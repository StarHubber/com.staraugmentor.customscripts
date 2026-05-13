using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace STAR.Utils
{
	/// <summary>
	/// This class is a base class for the actual VariableValues.
	/// Derived classes need to be in dedicated scripts to use them as ScriptableObjects.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class VariableObjectBase<T> : ScriptableObject
	{
		/// <summary>
		/// Event that is raised when the Value is changed.
		/// </summary>
		public event Action<T> onValueChanged;

		public bool debugOutput = false;

		private void OnValidate()
		{
			// To change propery when changing inspector value, assign manually in OnValidate
			Value = value;
		}

		// Serialize private field value, so JsonUtility can FromJsonOverwrite the value (see IHaveSettings interface)
		// SerializeField-attribute for visibility in inspector
		[Newtonsoft.Json.JsonProperty]
		[SerializeField]				
		private T value;
		public T Value
		{
			get => value;
			set
			{
				this.value = value;
				onValueChanged?.Invoke(value);

				if (debugOutput)
					Debug.Log($"VariableObject {name} changed to {value}");
			}
		}

		/// <summary>
		/// Setter function to make Value accessible via inspector, e.g. slider callback.
		/// </summary>
		/// <param name="input"></param>
		public void SetValue(T input)
		{
			Value = input;
		}
	}
}
