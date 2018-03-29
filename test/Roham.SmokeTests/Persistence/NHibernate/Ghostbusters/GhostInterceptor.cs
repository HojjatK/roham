using System.Collections.Generic;
using NHibernate;
using NHibernate.Proxy;

namespace Roham.Persistence.NHibernate.Ghostbusters
{
    internal class GhostInterceptor : EmptyInterceptor
    {
        private ISession _session;
        private readonly IList<string> _ghosts;

        public GhostInterceptor(IList<string> ghosts)
        {
            _ghosts = ghosts;
        }

        public override void SetSession(ISession session)
        {
            _session = session;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, global::NHibernate.Type.IType[] types)
        {
            var msg = string.Format("Flush Dirty {0}", entity.GetType().FullName);
            _ghosts.Add(msg);
            ListDirtyProperties(entity);
            return false;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, global::NHibernate.Type.IType[] types)
        {
            var msg = string.Format("Save {0}", entity.GetType().FullName);
            _ghosts.Add(msg);
            return false;
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, global::NHibernate.Type.IType[] types)
        {
            var msg = string.Format("Delete {0}", entity.GetType().FullName);
            _ghosts.Add(msg);
        }

        private void ListDirtyProperties(object entity)
        {
            string className = NHibernateProxyHelper.GuessClass(entity).FullName;
            var sessionImpl = _session.GetSessionImplementation();
            var persister = sessionImpl.Factory.GetEntityPersister(className);
            var oldEntry = sessionImpl.PersistenceContext.GetEntry(entity);
            if ((oldEntry == null) && (entity is INHibernateProxy))
            {
                var proxy = entity as INHibernateProxy;
                object obj = sessionImpl.PersistenceContext.Unproxy(proxy);
                oldEntry = sessionImpl.PersistenceContext.GetEntry(obj);
            }
            object[] oldState = oldEntry.LoadedState;
            object[] currentState = persister.GetPropertyValues(entity, sessionImpl.EntityMode);

            int[] dirtyProperties = persister.FindDirty(currentState, oldState, entity, sessionImpl);
            foreach (int index in dirtyProperties)
            {
                var msg = string.Format("Dirty property {0}.{1} was {2}, is {3}.",
                    className, persister.PropertyNames[index], oldState[index] ?? "null", currentState[index] ?? "null");
                _ghosts.Add(msg);
            }
        }
    }
}
