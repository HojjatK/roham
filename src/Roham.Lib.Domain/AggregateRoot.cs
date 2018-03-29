using System;

namespace Roham.Lib.Domain
{
    public abstract class AggregateRoot : Identifiable
    {
        private Guid _uid = default(Guid);

        public virtual Guid Uid
        {
            get
            {
                if (_uid == default(Guid))
                {
                    _uid = Guid.NewGuid();
                }
                return _uid;
            }
            protected set
            {
                _uid = value;
            }
        }
    }
}
