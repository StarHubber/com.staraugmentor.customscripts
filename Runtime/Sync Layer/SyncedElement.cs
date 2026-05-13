using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	[ExecuteInEditMode]
	public abstract class SyncedElementBase<TUIElement> : MonoBehaviour where TUIElement : Selectable
	{
		public string id;
		public bool disableInteractionByTrainer = true;

		protected virtual void OnEnable()
		{
			if (disableInteractionByTrainer)
			{
				InteractionControl.UserInteractionsDisabled += OnUserInteractionDisabled;
			}
			GetComponent<TUIElement>().interactable = !InteractionControl.InteractionsDisabled;
		}

		protected virtual void OnDisable()
		{
			if (disableInteractionByTrainer)
			{
				InteractionControl.UserInteractionsDisabled -= OnUserInteractionDisabled;
			}
		}

		private void OnUserInteractionDisabled(bool disabled)
		{
			GetComponent<TUIElement>().interactable = !disabled;
		}

#if UNITY_EDITOR
		private void Reset()
		{
			if (string.IsNullOrEmpty(id))
			{
				SetNewGuid();
			}
		}

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(id))
			{
				SetNewGuid();
			}
		}

		private void SetNewGuid()
		{
			id = "SyncedUIElement_" + System.Guid.NewGuid();
		}
#endif
	}
}