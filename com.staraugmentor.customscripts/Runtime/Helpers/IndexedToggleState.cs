using System;
using UnityEngine;

namespace StarCooperation
{
    [Serializable]
    public class IndexedToggleState: ISerializableMessagePayload
    {
        public int index;
        public bool state;

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public void Deserialize(string data)
        {
            JsonUtility.FromJsonOverwrite(data, this);
        }

        public ISerializableMessagePayload CreateUninitializedInstance()
        {
            return new IndexedToggleState();
        }
    }
}