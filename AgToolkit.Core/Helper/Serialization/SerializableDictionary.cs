using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace AgToolkit.Core.Helper.Serialization
{
    public abstract class SerializableDictionaryBase<TKey, TValue, TValueStorage> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private TKey[] _Keys;
        [SerializeField]
        private TValueStorage[] _Values;

        protected SerializableDictionaryBase()
        {
        }

        protected SerializableDictionaryBase(IDictionary<TKey, TValue> dict) : base(dict.Count)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        protected SerializableDictionaryBase(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected abstract void SetValue(TValueStorage[] storage, int i, TValue value);
        protected abstract TValue GetValue(TValueStorage[] storage, int i);

        public void CopyFrom(IDictionary<TKey, TValue> dict)
        {
            Clear();
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
            {
                this[kvp.Key] = kvp.Value;
            }
        }

        public void OnAfterDeserialize()
        {
            if (_Keys == null) return;
            if (_Values == null) return;
            if (_Keys.Length != _Values.Length) return;

            Clear();
            int n = _Keys.Length;
            for (int i = 0; i < n; ++i)
            {
                this[_Keys[i]] = GetValue(_Values, i);
            }

            _Keys = null;
            _Values = null;

        }

        public void OnBeforeSerialize()
        {
            int n = Count;
            _Keys = new TKey[n];
            _Values = new TValueStorage[n];

            int i = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                _Keys[i] = kvp.Key;
                SetValue(_Values, i, kvp.Value);
                ++i;
            }
        }
    }

    public class SerializableDictionary<TKey, TValue> : SerializableDictionaryBase<TKey, TValue, TValue>
    {
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override TValue GetValue(TValue[] storage, int i)
        {
            return storage[i];
        }

        protected override void SetValue(TValue[] storage, int i, TValue value)
        {
            storage[i] = value;
        }
    }

    public static class SerializableDictionaryStorage
    {
        public class Storage<T>
        {
            public T data;
        }
    }

    public class SerializableDictionary<TKey, TValue, TValueStorage> : SerializableDictionaryBase<TKey, TValue, TValueStorage> where TValueStorage : SerializableDictionaryStorage.Storage<TValue>, new()
    {
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dict) : base(dict)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override TValue GetValue(TValueStorage[] storage, int i)
        {
            return storage[i].data;
        }

        protected override void SetValue(TValueStorage[] storage, int i, TValue value)
        {
            storage[i] = new TValueStorage
            {
                data = value
            };
        }
    }
}