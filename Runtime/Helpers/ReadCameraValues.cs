using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class ReadCameraValues : MonoBehaviour
	{
		public Text textPosX;
		public Text textPosY;
		public Text textPosZ;

		public Text textRotX;
		public Text textRotY;
		public Text textRotZ;

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			textPosX.text = MoveCamera.instance.transform.position.x.ToString();
			textPosY.text = MoveCamera.instance.transform.position.y.ToString();
			textPosZ.text = MoveCamera.instance.transform.position.z.ToString();

			textRotX.text = MoveCamera.instance.transform.eulerAngles.x.ToString();
			textRotY.text = MoveCamera.instance.transform.eulerAngles.y.ToString();
			textRotZ.text = MoveCamera.instance.transform.eulerAngles.z.ToString();
		}
	}
}