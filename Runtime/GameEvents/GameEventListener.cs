using UnityEngine;
using System.Collections;
using UnityEngine.Events;
//using Pixeye.Unity;
using NaughtyAttributes;

namespace STAR.Utils
{
	[DefaultExecutionOrder(200)] // -> see OnEnable()
	public class GameEventListener : MonoBehaviour
	{
		// The game event instance to register to
		public GameEvent gameEvent;

		[Foldout("Foldout")]public bool listeningEnabled = true;
		[Foldout("Foldout")]public bool catchPreviousEventOnRegister = false;
		[Foldout("Foldout")]public bool unregisterOnDisable = true;

		private bool startHasFinished = false;
		private bool registered;

		#if UNITY_EDITOR
		[Foldout("Events")]public bool listenToVoid;
		[Foldout("Events")]public bool listenToInt;
		[Foldout("Events")]public bool listenToFloat;
		[Foldout("Events")]public bool listenToBool;
		[Foldout("Events")]public bool listenToBoolInv;
		[Foldout("Events")]public bool listenToBoolTrue;
		[Foldout("Events")]public bool listenToBoolFalse;
		[Foldout("Events")]public bool listenToString;
		#endif
		// The unity event response created for the event
		[Foldout("Foldout")] [ShowIf("listenToVoid")]public UnityEvent response_void;
		[Foldout("Foldout")] [ShowIf("listenToInt")]public UnityEvent_Int response_int;
		[Foldout("Foldout")] [ShowIf("listenToFloat")]public UnityEvent_Float response_float;
		[Foldout("Foldout")] [ShowIf("listenToBool")]public UnityEvent_Bool response_bool;
		[Foldout("Foldout")] [ShowIf("listenToBoolInv")]public UnityEvent_Bool response_bool_inv;
		[Foldout("Foldout")] [ShowIf("listenToBoolTrue")]public UnityEvent_Bool response_bool_true;
		[Foldout("Foldout")] [ShowIf("listenToBoolFalse")]public UnityEvent_Bool response_bool_false;
		[Foldout("Foldout")] [ShowIf("listenToString")]public UnityEvent_String response_string;


		#if UNITY_EDITOR
		private void OnValidate()
		{
			try
			{
				if (response_void.GetPersistentEventCount() > 0 && listenToVoid == false)
				{
					listenToVoid = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for void.");
				}
				if (response_int.GetPersistentEventCount() > 0 && listenToInt == false)
				{
					listenToInt = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for int.");
				}
				if (response_float.GetPersistentEventCount() > 0 && listenToFloat == false)
				{
					listenToFloat = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for float.");
				}
				if (response_bool.GetPersistentEventCount() > 0 && listenToBool == false)
				{
					listenToBool = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for bool.");
				}
				if (response_bool_inv.GetPersistentEventCount() > 0 && listenToBoolInv == false)
				{
					listenToBoolInv = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for bool inverse.");
				}
				if (response_bool_true.GetPersistentEventCount() > 0 && listenToBoolTrue == false)
				{
					listenToBoolTrue = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for bool true.");
				}
				if (response_bool_false.GetPersistentEventCount() > 0 && listenToBoolFalse == false)
				{
					listenToBoolFalse = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for bool false.");
				}
				if (response_string.GetPersistentEventCount() > 0 && listenToString == false)
				{
					listenToString = true;
					Debug.LogWarning($"Can't switch toggle to false, because {gameObject.name} has persistent event listeners for string.");
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError($"GameEvent error in GameObject {gameObject.name}: {e.Message}");
			}
		}
		#endif

		private void OnEnable()
		{
			// RegisterListener(this, catchPreviousEventOnRegister) might result in calling event responses -before-
			// the script who's function is called has already initialized via Awake().
			// To ensure all Awake's have finished before GameEventListener's OnEnable, execution order is set to + 200.

			if (gameEvent == null)
			{
				Debug.LogError("No GameEvent attached to listener: " + gameObject.name);
				return;
			}

			// Register to GameEvent, double-check if already done (due to unregister logic in OnDisable())
			if (!registered)
			{
				gameEvent.RegisterListener(this, catchPreviousEventOnRegister);
				registered = true;
			}
		}

		private void Start()
		{
			startHasFinished = true;
		}

		private void OnDisable()
		{
			if (gameEvent == null)
			{
				Debug.LogError("No GameEvent attached to listener: " + gameObject.name);
				return;
			}

			if (unregisterOnDisable)
			{
				registered = false;
				gameEvent.UnregisterListener(this);
			}
		}

		public void EnableListening(bool enable)
		{
			listeningEnabled = enable;
		}

		public void StopListening()
		{
			listeningEnabled = false;
		}

		public void StartListening()
		{
			listeningEnabled = true;
		}

		public void RaiseEventVoid()
		{
			if (!listeningEnabled)
				return;

			response_void.Invoke();
		}

		public void RaiseEventInt(int value)
		{
			if (!listeningEnabled)
				return;

			response_int.Invoke(value);
		}

		public void RaiseEventFloat(float value)
		{
			if (!listeningEnabled)
				return;

			response_float.Invoke(value);
		}

		public void RaiseEventBool(bool value)
		{
			if (!listeningEnabled)
				return;

			response_bool.Invoke(value);
			response_bool_inv.Invoke(!value);
			if (value)
				response_bool_true.Invoke(true);
			else
				response_bool_false.Invoke(false);
		}

		public void RaiseEventString(string value)
		{
			if (!listeningEnabled)
				return;

			response_string.Invoke(value);
		}
	}
}