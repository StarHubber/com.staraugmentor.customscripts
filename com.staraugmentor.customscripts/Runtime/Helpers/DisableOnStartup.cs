using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StarCooperation.Helpers
{
	public class DisableOnStartup : MonoBehaviour
	{
		public enum StartMethod
		{
			OnEnable,
			Awake,
			Start
		}

		public StartMethod disableOn = StartMethod.Start;

		private void OnEnable()
		{
			if (disableOn == StartMethod.OnEnable)
			{
				gameObject.SetActive(false);
			}
		}

		private void Awake()
		{
			if (disableOn == StartMethod.Awake)
			{
				gameObject.SetActive(false);
			}
		}

		private void Start()
		{
			if (disableOn == StartMethod.Start)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
