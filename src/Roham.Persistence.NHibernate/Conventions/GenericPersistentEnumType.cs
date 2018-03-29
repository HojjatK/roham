using NHibernate.Type;

namespace Roham.Persistence.NHibernate.Conventions
{
    public class GenericPersistentEnumType<T> : PersistentEnumType
    {
        public GenericPersistentEnumType() : base(typeof(T)) {}
    }
}