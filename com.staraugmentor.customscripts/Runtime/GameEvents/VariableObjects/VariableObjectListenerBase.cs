using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace STAR.Utils
{

    public class VariableObjectListenerBase<T> : MonoBehaviour
    {
        public VariableObjectBase<T> variableObject;
		public UnityEvent<T> onValueChanged;

		private void Awake()
		{
			// Cast VariableObject event to UnityEvent, to assign callbacks in inspector
			variableObject.onValueChanged += value => onValueChanged?.Invoke(value);
		}
	}
}