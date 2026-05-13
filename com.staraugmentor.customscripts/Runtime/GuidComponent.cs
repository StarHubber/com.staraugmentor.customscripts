using System;
using System.Collections.Generic;
using UnityEngine;


    [ExecuteInEditMode]
    public class GUIDComponent : MonoBehaviour
    {
        private static List<GUIDComponent> allComponents;

        // Private to protect from access, serialized to display in editor. Therefore no prop, but field.
        [SerializeField] private string guid;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
                guid = Guid.NewGuid().ToString();
        }
        private void Awake()
        {
            if (allComponents == null)
                allComponents = new List<GUIDComponent>();

            allComponents.Add(this);
        }

        private void OnDestroy()
        {
            allComponents.Remove(this);
        }

        public static GUIDComponent FindByGuid(string guid)
        {
            return allComponents.Find(c => c.GetGuid() == guid);
        }

        public string GetGuid()
        {
            return guid;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }
    }

