using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Roham.Persistence.NHibernate.UserTypes;
using Roham.Lib.Domain.DataAnnotation;

namespace Roham.Persistence.NHibernate.Conventions
{
    public class PropertyConvention : IPropertyConvention
    {
        public void Apply(IPropertyInstance instance)
        {
            if (instance == null)
            {
                return;
            }

            // Bracket every property to ensure resevered work problems go away.
            instance.Column($"[{instance.Name}]");

            ProcessUniqueKeyAttribute(instance);
            ProcessRequiredAttribute(instance);

            if (ProcessStringTypes(instance) || ProcessBooleanTypes(instance) || ProcessDateTypes(instance) || ProcessEnumTypes(instance))
            {
                return;
            }
        }

        private bool ProcessUniqueKeyAttribute(IPropertyInstance instance)
        {
            if (instance.Property.MemberInfo.IsDefined(typeof(KeyAttribute), false))
            {
                instance.Unique();
                return true;
            }
            else
            {
                var uniqueAttribute =
                    (from attribute
                     in instance.Property.MemberInfo.GetCustomAttributes(typeof(UniqueAttribute), false)
                     select (UniqueAttribute)attribute).FirstOrDefault();

                if (uniqueAttribute != null)
                {
                    if (!string.IsNullOrWhiteSpace(uniqueAttribute.KeyName))
                    {
                        instance.UniqueKey(uniqueAttribute.KeyName);
                    }
                    else
                    {
                        instance.Unique();
                    }
                    return true;
                }
            }
            return false;
        }

        private bool ProcessRequiredAttribute(IPropertyInstance instance)
        {
            if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false))
            {
                instance.Not.Nullable();
                return true;
            }
            return false;
        }

        private bool ProcessStringTypes(IPropertyInstance instance)
        {
            if (instance.Type == typeof(string) || instance.Type == typeof(PageNameUserType))
            {
                int length;
                if (HasMaxLengthAttribute(instance, out length) ||
                    HasStringLengthAttribute(instance, out length))
                {
                    // TODO: based on sqlserver, sqlite?
                    if (length <= 4000)
                    {
                        instance.CustomSqlType(string.Format("nvarchar({0})", length));
                        instance.Length(length);
                    }
                    else
                    {
                        instance.CustomSqlType("ntext");
                        instance.Length(4001);
                    }
                    return true;
                }
                ConventionBasedLengthAssignment(instance);
                return true;
            }

            return false;
        }

        private static bool ProcessBooleanTypes(IPropertyInstance instance)
        {
            if (instance.Type == typeof(bool))
            {
                return true;
            }
            return false;
        }

        private static bool ProcessDateTypes(IPropertyInstance instance)
        {
            if (instance.Type == typeof(DateTime))
            {
                if (instance.Name.ToUpper().Contains("START") || instance.Name.ToUpper().Contains("CREATEDON"))
                {
                    instance.Default("(getdate())");
                    instance.Not.Nullable();
                }
                return true;
            }
            return false;
        }

        private static bool ProcessEnumTypes(IPropertyInstance instance)
        {
            if (instance.Type.IsEnum)
            {
                if (instance.Property.PropertyType.IsGenericType &&
                    instance.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    instance.Nullable();
                }
                else
                {
                    instance.Not.Nullable();
                }
                instance.CustomType(typeof(GenericPersistentEnumType<>)
                    .CreateGenericType(instance.Type.GetGenericArguments()[0]));
                return true;
            }

            return false;
        }

        private static bool HasMaxLengthAttribute(IPropertyInstance instance, out int maxLength)
        {
            maxLength = (from attribute
                          in instance.Property.MemberInfo.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                         select (MaxLengthAttribute)attribute into result
                         select result.Length)
                      .FirstOrDefault();
            return maxLength > 0;
        }

        private static bool HasStringLengthAttribute(IPropertyInstance instance, out int length)
        {
            length = (from attribute
                        in instance.Property.MemberInfo.GetCustomAttributes(typeof(StringLengthAttribute), false)
                      select (StringLengthAttribute)attribute into result
                      select result.MaximumLength)
                    .FirstOrDefault();
            return length > 0;
        }

        private static void ConventionBasedLengthAssignment(IPropertyInstance instance)
        {
            if (instance == null) return;

            if (instance.Name.ToUpper().Contains("SHORT"))
            {
                instance.CustomSqlType("nvarchar(10)");
                return;
            }

            switch (instance.Name)
            {
                case "Name":
                    instance.CustomSqlType("nvarchar(255)");
                    break;
                case "Description":
                    instance.CustomSqlType("nvarchar(1024)");
                    break;
                default:
                    instance.CustomSqlType("nvarchar(150)");
                    break;
            }
        }
    }
}