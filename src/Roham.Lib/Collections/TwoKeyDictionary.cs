using System;
using System.Collections;
using System.Collections.Generic;

namespace Roham.Lib.Collections
{
    public class TwoKeyDictionary<Key1, Key2, Value> : IEnumerable<KeyValuePair<Tuple<Key1, Key2>, Value>>
    {
        protected readonly Dictionary<Tuple<Key1, Key2>, Value> _underlyingDictionary = new Dictionary<Tuple<Key1, Key2>, Value>();

        public TwoKeyDictionary() { }

        public TwoKeyDictionary(Dictionary<Tuple<Key1, Key2>, Value> dictionary)
        {
            _underlyingDictionary = dictionary;
        }

        public void Add(Key1 firstKey, Key2 secondKey, Value value)
        {
            var key = Tuple.Create(firstKey, secondKey);
            _underlyingDictionary.Add(key, value);
        }

        public bool Remove(Key1 firstKey, Key2 secondKey)
        {
            var key = Tuple.Create(firstKey, secondKey);
            return _underlyingDictionary.Remove(key);
        }

        public void Clear()
        {
            _underlyingDictionary.Clear();
        }

        public bool Contains(Key1 firstKey, Key2 secondKey)
        {
            var key = Tuple.Create(firstKey, secondKey);
            return Contains(key);
        }

        protected bool Contains(Tuple<Key1, Key2> key)
        {
            return _underlyingDictionary.ContainsKey(key);
        }

        public bool TryGetValue(Key1 firstKey, Key2 secondKey, out Value value)
        {
            var key = Tuple.Create(firstKey, secondKey);
            return _underlyingDictionary.TryGetValue(key, out value);
        }

        public Value this[Key1 firstKey, Key2 secondKey]
        {
            get
            {
                var key = Tuple.Create(firstKey, secondKey);
                return _underlyingDictionary[key];
            }
            set
            {
                var key = Tuple.Create(firstKey, secondKey);
                _underlyingDictionary[key] = value;
            }
        }

        public IEnumerator<KeyValuePair<Tuple<Key1, Key2>, Value>> GetEnumerator()
        {
            return _underlyingDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}