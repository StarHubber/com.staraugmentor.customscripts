using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class DocButtonHandler : MonoBehaviour
	{
		public string syncID;

		[Space(5)]
		public Button buttonOpenDoc;
		public Tooltip connectedTooltip;

		private Toggle toggle;

		private void Awake()
		{
			toggle = GetComponent<Toggle>();

			// Assign button ID before activating button
			buttonOpenDoc.GetComponent<SyncedButton>().id = syncID;

			// Enable PDF button and connect to PDF viewer of tooltip
			if (connectedTooltip != null)
			{
				connectedTooltip.GetComponent<Tooltip>().connectedButtonDoc = buttonOpenDoc;

				toggle.onValueChanged.AddListener(isOn => buttonOpenDoc.gameObject.SetActive(isOn));

				buttonOpenDoc.onClick.AddListener(delegate
				{
					SceneManager.instance.OpenPDF(connectedTooltip.pdfViewer);
				});
			}
		}

#if UNITY_EDITOR
		private void Reset()
		{
			if (string.IsNullOrEmpty(syncID))
			{
				UpdateNameFromGameObject();
			}
		}

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(syncID))
			{
				UpdateNameFromGameObject();
			}
		}

		private void UpdateNameFromGameObject()
		{
			syncID = "SyncedButtonPDF_" + gameObject.name;
		}
#endif
	}
}