/* Copyright - Roham 
 * This software may be modified and distributed under the terms of the MIT license.  See the LICENSE file for details.*/
using System;
using NHibernate.Proxy;
using Roham.Lib.Domain;

namespace Roham.Domain.Entities
{
    /// <summary>
    /// This static class provides common extension methods for <see cref="IEntity"/> types.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Gets the real, underlying Entity-type - as opposed to the standard GetType() method,
        /// this method takes into account the possibility that the object may in fact be an
        /// NHibernate Proxy object, and not a real object. This method will return the real
        /// Entity-type, doing a full initialization if necessary.
        /// </summary>
        public static Type GetEntityType(this Identifiable entity)
        {
            if (entity is INHibernateProxy)
            {
                var lazyInitialiser = ((INHibernateProxy)entity).HibernateLazyInitializer;
                var type = lazyInitialiser.PersistentClass;

                if (type.IsAbstract || type.GetNestedTypes().Length > 0)
                {
                    return Unproxy<Identifiable>(entity).GetType();
                }
                else 
                {
                    // we don't need to "unbox" the Proxy-object to get the type
                    return lazyInitialiser.PersistentClass;
                }
            }

            return entity.GetType();
        }

        /// <summary>
        /// Based on the real, underlying Entity-type, this method returns true if the specified
        /// type matches (or is assignable from) the specified Type.
        /// </summary>
        public static bool Is<TEntity>(this Identifiable entity)
            where TEntity : Identifiable
        {
            var entityType = entity.GetEntityType();
            var type = typeof(TEntity);

            return entityType == type || type.IsAssignableFrom(entityType);
        }

        /// <summary>
        /// In some cases, you may need the actual object, not just the type - for example, if
        /// you're going to cast to a type deeper in the hierarchy, you may need to Unproxy
        /// the object first.
        /// </summary>
        public static TEntity Unproxy<TEntity>(this object entity)
            where TEntity : Identifiable
        {
            var proxy = entity as INHibernateProxy;
            return proxy != null
                ? proxy.HibernateLazyInitializer.GetImplementation() as TEntity
                : entity as TEntity;
        }
    }
}