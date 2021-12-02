using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedDictionary<T, U> : Dictionary<T, U>, ISerializationCallbackReceiver
{
    [SerializeField] private List<T> _keys;
    [SerializeField] private List<U> _values;
    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < _keys.Count && i < _values.Count; i++)
            if (!ContainsKey(_keys[i]))
                Add(_keys[i], _values[i]);
    }
}
