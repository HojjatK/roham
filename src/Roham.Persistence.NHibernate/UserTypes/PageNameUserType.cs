using System;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NHibernate;
using Roham.Lib.Strings;

namespace Roham.Persistence.NHibernate.UserTypes
{
    public class PageNameUserType : IUserType
    {
        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Disassemble(object value)
        {
            return value;
        }

        public new bool Equals(object x, object y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        public bool IsMutable => false;

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var resultAsString = (string)NHibernateUtil.String.NullSafeGet(rs, names[0]);
            var result = (PageName)(resultAsString ?? string.Empty);
            return result;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            value = (PageName)(value ?? string.Empty);
            ((IDataParameter)cmd.Parameters[index]).Value = value.ToString();
        }

        public object Replace(object original, object target, object owner)
        {
            return target;
        }

        public Type ReturnedType => typeof(PageName); 

        public global::NHibernate.SqlTypes.SqlType[] SqlTypes
        {
            get { return new[] { new StringSqlType() }; }
        }
    }
}
