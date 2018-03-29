using System;

namespace Roham.Lib.Domain
{
    public abstract class Entity<TId>
    {
        public virtual TId Id { get; protected set; }

        public virtual bool IsTransient
        {
            get { return IsTransientEntity(this); }
            protected set { }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity<TId>);
        }

        public virtual bool Equals(Entity<TId> other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetUnproxiedType() != other.GetUnproxiedType())
            {
                return false;
            }

            if (!IsTransientEntity(this) && !IsTransientEntity(other) && Equals(Id, other.Id))
            {
                var thisType = GetUnproxiedType();
                var otherType = other.GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) || otherType.IsAssignableFrom(thisType);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(TId)))
            {
                return base.GetHashCode();
            }

            return Id.GetHashCode();
        }

        public static bool IsTransientEntity(Entity<TId> obj)
        {
            return obj != null && Equals(obj.Id, default(TId));
        }

        protected Type GetUnproxiedType()
        {
            return GetType();
        }
    }
}
