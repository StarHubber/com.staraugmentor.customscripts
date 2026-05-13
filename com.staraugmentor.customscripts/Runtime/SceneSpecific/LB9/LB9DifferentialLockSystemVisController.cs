using StarCooperation.Helpers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
	public class LB9DifferentialLockSystemVisController : MonoBehaviour
    {
        public static LB9DifferentialLockSystemVisController instance;
        public int FadeTime;

        [Serializable]
		private class ObjectSetup
		{
			[SerializeField] private ScriptableEventBool scriptableEvent;
			[SerializeField] private Renderer pathHighlight;
			[SerializeField] private ParticleSystem poiHighlighter;
            public int fadeTime;

            private float valueChangeTime;
			private MaterialPropertyBlock propBlock;
			private static readonly int FadePropKey = Shader.PropertyToID("_Fade");

			public void Initialize()
			{
				if (propBlock == null)
				{
					propBlock = new MaterialPropertyBlock();
				}
				else
				{
					propBlock.Clear();
				}

				propBlock.SetFloat(FadePropKey, 0);
				pathHighlight.SetPropertyBlock(propBlock);
				valueChangeTime = -10f;
				scriptableEvent.Raised += SetActiveState;
			}

			public void Cleanup()
			{
				scriptableEvent.Raised -= SetActiveState;
				poiHighlighter.Stop();
			}

			private void SetActiveState(bool state)
			{
				//Debug.Log("Value changed");
				valueChangeTime = Time.time;
				if (state)
				{
					poiHighlighter.Play();
				}
				else
				{
					poiHighlighter.Stop();
				}
			}

            public void Update()
            {
                var t = Mathf.Clamp01(Time.time - valueChangeTime);
				propBlock.SetFloat(FadePropKey, Mathf.PingPong(t * 2f, fadeTime));
				pathHighlight.SetPropertyBlock(propBlock);
			}
		}

        private void Awake()
        {
            instance = this;
        }

		[SerializeField] private List<ObjectSetup> elements = new List<ObjectSetup>();

		private void OnEnable()
		{
			foreach (var element in elements)
			{
				element.Initialize();
			}
		}

		private void OnDisable()
		{
			foreach (var element in elements)
			{
				element.Cleanup();
			}
		}

		private void Update()
		{
			foreach (var element in elements)
			{
				element.Update();
			}
		}

		public void SetFadeTime(int value, string lockValue)
		{
			switch (lockValue)
			{
				case "ButtonVTG":
					elements[0].fadeTime = value;
					break;

				case "ButtonVA":
					elements[1].fadeTime = value;
					break;

				case "ButtonHA":
					elements[2].fadeTime = value;
					break;
			}
		}
	}
}