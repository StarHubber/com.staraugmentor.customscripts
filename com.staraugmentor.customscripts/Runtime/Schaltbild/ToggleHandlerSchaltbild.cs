using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class ToggleHandlerSchaltbild : MonoBehaviour
	{
		public GameObject imagePanel;
		public GameObject legendePanel;

		private Toggle[] imageToggles;
		private Toggle[] legendeToggles;

		private bool isUpdating = false;

		private void OnEnable()
		{
			// Manually deactivate image toggles on enabling image
			if (imageToggles != null && imageToggles.Length > 0)
			{
				for (int i = 0; i < imageToggles.Length; i++)
				{
					imageToggles[i].isOn = false;
				}
			}
		}

		private void Start()
		{
			if (imagePanel == null || legendePanel == null)
			{
				return;
			}

			imageToggles = imagePanel.GetComponentsInChildren<Toggle>();
			legendeToggles = legendePanel.GetComponentsInChildren<Toggle>();

			foreach (Toggle imageToggle in imageToggles)
			{
				imageToggle.onValueChanged.AddListener(delegate { SyncLegende(); });
			}
			foreach (Toggle legendeToggle in legendeToggles)
			{
				legendeToggle.onValueChanged.AddListener(delegate { SyncImage(); });
			}
		}

		public void SyncImage()
		{
			if (isUpdating == false)
			{
				isUpdating = true;
				for (int i = 0; i < imageToggles.Length; i++)
				{
					imageToggles[i].isOn = legendeToggles[i].isOn;
				}
				isUpdating = false;
			}
		}

		public void SyncLegende()
		{
			if (isUpdating == false)
			{
				isUpdating = true;
				for (int i = 0; i < legendeToggles.Length; i++)
				{
					legendeToggles[i].isOn = imageToggles[i].isOn;
				}
				isUpdating = false;
			}
		}

	}
}