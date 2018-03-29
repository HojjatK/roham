using System.Linq;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Roham.Lib.Domain.DataAnnotation;

namespace Roham.Persistence.NHibernate.Conventions
{
    public class ReferenceConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            if (instance == null) return;

            // Bracket every property to ensure resevered work problems go away.
            instance.Column($"[{instance.Name}]");
            instance.ForeignKey($"FK_{instance.Name}_{instance.EntityType.Name}");
            if (instance.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false))
            {
                instance.Not.Nullable();
            }

            ProcessUniqueKeyAttribute(instance);
        }

        private bool ProcessUniqueKeyAttribute(IManyToOneInstance instance)
        {
            if (instance.Property.MemberInfo.IsDefined(typeof(KeyAttribute), false))
            {
                instance.Index($"IX_{instance.Name}_{instance.EntityType.Name}");
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
                        instance.UniqueKey($"UQ_{instance.Name}_{instance.EntityType.Name}");
                    }
                    return true;
                }
            }
            return false;
        }
    }

    public class HasManyForeignKeyConvention : IHasManyConvention
    {
        public void Apply(IOneToManyCollectionInstance instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.Key.ForeignKey($"FK_{instance.Member.Name}_{instance.EntityType.Name}");
            if (instance.Member.IsDefined(typeof(RequiredAttribute), false))
            {
                instance.Not.KeyNullable();
            }
        }
    }

    public class ManyToManyForeignKeyConvention : IHasManyToManyConvention
    {
        public void Apply(IManyToManyCollectionInstance instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.Key.ForeignKey($"FK_{instance.TableName}_{instance.EntityType.Name}");
            if (instance.Member.IsDefined(typeof(RequiredAttribute), false))
            {
                instance.Not.KeyNullable();
            }
        }
    }

    public class JoinedSubclassForeignKeyConvention : IJoinedSubclassConvention
    {
        public void Apply(IJoinedSubclassInstance instance)
        {
            if (instance.Type.BaseType != null)
            {
                instance.Key.ForeignKey($"FK_{instance.Type.BaseType.Name}_{instance.EntityType.Name}");                
            }
        }
    }
}