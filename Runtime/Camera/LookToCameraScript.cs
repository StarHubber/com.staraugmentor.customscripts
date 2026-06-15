using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation.STAR
{
	public class LookToCameraScript : MonoBehaviour
	{

		public Transform Cam;
		public Transform rotationObject;
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (Cam != null)
			{
				transform.LookAt(Cam.position);
				transform.position = rotationObject.position;
			}
		}
	}
}