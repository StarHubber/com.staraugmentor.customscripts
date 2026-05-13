using UnityEngine;

namespace StarCooperation
{
	public class OrientationChecker : MonoBehaviour
	{
		private void Awake()
		{
			EnablePortraitOrientation(false);
		}

		public void EnablePortraitOrientation(bool enable)
		{
			Screen.orientation = enable ? ScreenOrientation.AutoRotation : ScreenOrientation.LandscapeLeft;
		}
	}
}