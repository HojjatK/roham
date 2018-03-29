using System.Collections.Generic;
using System.Threading;

namespace Roham.Lib.Collections
{
    public class ThreadLocalLookup<Key, Value>
    {
        private readonly ThreadLocal<Dictionary<Key, Value>> _localLookup = new ThreadLocal<Dictionary<Key, Value>>();

        private Dictionary<Key, Value> Lookup
        {
            get
            {
                if (_localLookup.Value == null)
                {
                    _localLookup.Value = new Dictionary<Key, Value>();
                }
                return _localLookup.Value;
            }
        }

        public Value this[Key key]
        {
            get
            {
                Value value;
                Lookup.TryGetValue(key, out value);
                return value;
            }
            set
            {
                Lookup[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return Lookup.Count;
            }
        }
    }
}