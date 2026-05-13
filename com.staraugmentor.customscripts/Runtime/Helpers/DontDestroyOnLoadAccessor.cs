using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	[DefaultExecutionOrder(-1001)]
	public class DontDestroyOnLoadAccessor : MonoBehaviour
	{
		// For development only
		public bool dontDontDestroyOnLoad = false;

		public static DontDestroyOnLoadAccessor instance;
		public DeviceSwitcher deviceSwitcher;
		[HideInInspector] public AppType appType = AppType.None;
		[HideInInspector] public string startSceneName;

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;

				appType = deviceSwitcher.device;
				startSceneName = gameObject.scene.name;

				if (!dontDontDestroyOnLoad)
				{
					transform.SetParent(null);
					DontDestroyOnLoad(gameObject);
				}
				else
				{
					// Aways push on DontDestryOnLoad when built
#if !UNITY_EDITOR
					transform.SetParent(null);
					DontDestroyOnLoad(gameObject);
#endif
					Debug.LogWarning("Attention: DontDestroyOnLoadAccessor in development mode for scene savings! Disable flag when finished.");
				}
			}
			else
			{
				Destroy(gameObject);
			}
		}

		// Start is called before the first frame update
		private void Start()
		{

		}

		public GameObject[] GetDontDestroyOnLoadObjects()
		{
			return gameObject.scene.GetRootGameObjects();
		}

		public List<string> GetDontDestroyOnLoadNames()
		{
			var objs = gameObject.scene.GetRootGameObjects();
			var names = new string[objs.Length];
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = objs[i].name;
			}
			return new List<string>(names);
		}
	}
}