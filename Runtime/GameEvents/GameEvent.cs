using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NaughtyAttributes;

namespace STAR.Utils
{
	[CreateAssetMenu(fileName = "NewGameEvent", menuName = "STAR Cooperation/STAR/Events/Game Event")]
	[DefaultExecutionOrder(100)] // -> see OnEnable()
	public class GameEvent : ScriptableObject
	{
		// Events to enable listening via script
		public event Action onVoidRaised;
		public event Action<int> onIntRaised;
		public event Action<float> onFloatRaised;
		public event Action<bool> onBoolRaised;
		public event Action<string> onStringRaised;

		public bool eventRaisingEnabled = true;
		public bool debugOutput = false;

		private List<GameEventListener> listeners = new List<GameEventListener>();

		public int LastInt { get; private set; }
		public float LastFloat { get; private set; }
		public bool LastBool { get; private set; }
		public string LastString { get; private set; }

		private bool voidRaised = false;
		private bool intRaised = false;
		private bool floatRaised = false;
		private bool boolRaised = false;
		private bool stringRaised = false;

		[ResizableTextArea]
		[SerializeField] private string comment;

		private void OnEnable()
		{
			// GameEvents are instanced ScriptableObjects. To avoid writing raised-flags permamently to "true",
			// set to "false" on startup. Awake() won't be called, because they exist already on disk.
			// ExecutionOrder is set to +100 to run this OnEnable() before GameEventListener's OnEnable()s (which are set to 200).

			voidRaised = false;
			intRaised = false;
			floatRaised = false;
			boolRaised = false;
			stringRaised = false;
		}

		public void RegisterListener(GameEventListener listener, bool raisePreviousEvents)
		{
			listeners.Add(listener);

			// Apply last called raise event to new listener
			if (raisePreviousEvents)
			{
				if (voidRaised)
					listener.RaiseEventVoid();
				if (intRaised)
					listener.RaiseEventInt(LastInt);
				if (floatRaised)
					listener.RaiseEventFloat(LastFloat);
				if (boolRaised)
					listener.RaiseEventBool(LastBool);
				if (stringRaised)
					listener.RaiseEventString(LastString);
			}
		}

		public void UnregisterListener(GameEventListener listener)
		{
			listeners.Remove(listener);
		}

		public void EnableRaising(bool enable)
		{
			eventRaisingEnabled = enable;
		}

		[Button]
		public void RaiseVoid()
		{
			if (!eventRaisingEnabled)
				return;

			if (debugOutput)
				Debug.Log($"{this.name}: Raise void");

			// Note: Order is important here.
			// First raise the flag, then the actual events. This allows event listeners that are activated/instantiated in the event
			// itself to listen to the event raising via the 'raisePreviousEvents' flag upon registering.
			voidRaised = true;

			onVoidRaised?.Invoke();

			for (int i = listeners.Count - 1; i >= 0; i--)
			{
				listeners[i].RaiseEventVoid();
			}
		}

		public void RaiseInt(int value)
		{
			if (!eventRaisingEnabled)
				return;

			if (debugOutput)
				Debug.Log($"{this.name}: Raise {value.GetType()}: {value}");

			LastInt = value;
			intRaised = true;

			onIntRaised?.Invoke(value);

			for (int i = listeners.Count - 1; i >= 0; i--)
			{
				listeners[i].RaiseEventInt(value);
			}
		}

		public void RaiseFloat(float value)
		{
			if (!eventRaisingEnabled)
				return;

			if (debugOutput)
				Debug.Log($"{this.name}: Raise {value.GetType()}: {value}");

			LastFloat = value;
			floatRaised = true;

			onFloatRaised?.Invoke(value);

			for (int i = listeners.Count - 1; i >= 0; i--)
			{
				listeners[i].RaiseEventFloat(value);
			}
		}

		public void RaiseBool(bool value)
		{
			if (!eventRaisingEnabled)
				return;

			if (debugOutput)
				Debug.Log($"{this.name}: Raise {value.GetType()}: {value}");

			LastBool = value;
			boolRaised = true;

			onBoolRaised?.Invoke(value);

			for (int i = listeners.Count - 1; i >= 0; i--)
			{
				listeners[i]?.RaiseEventBool(value);
			}
		}

		public void RaiseString(string value)
		{
			if (!eventRaisingEnabled)
				return;

			if (debugOutput)
				Debug.Log($"{this.name}: Raise {value.GetType()}: {value}");

			LastString = value;
			stringRaised = true;

			onStringRaised?.Invoke(value);

			for (int i = listeners.Count - 1; i >= 0; i--)
			{
				listeners[i].RaiseEventString(value);
			}
		}
	}
}