using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class HighlightElements : MonoBehaviour
	{

		public Transform[] theObjects;
		public Material highlightColor;
		private Material[] tempOldColor;
		private bool isObjectsHighlighted = false;
		private Transform ttPanel;
		private string ttString = "";

		private void Start()
		{
			tempOldColor = new Material[theObjects.Length];
			for (int i = 0; i < theObjects.Length; i++)
			{
				tempOldColor[i] = theObjects[i].GetComponent<Renderer>().material;
			}
			ttPanel = Camera.main.transform.Find("PanelTooltip");
			ttString = this.transform.Find("Label").GetComponent<Text>().text;
		}

		public void HighlightObjects()
		{
			for (int i = 0; i < theObjects.Length; i++)
			{
				theObjects[i].GetComponent<Renderer>().material = new Material(highlightColor);
			}
			isObjectsHighlighted = true;
			ttPanel.transform.Find("Text").GetComponent<TextMesh>().text = ttString;
			ttPanel.gameObject.SetActive(true);
		}

		public void HighlightObjects_Off()
		{
			for (int i = 0; i < theObjects.Length; i++)
			{
				theObjects[i].GetComponent<Renderer>().material = new Material(tempOldColor[i]);
			}
			isObjectsHighlighted = false;
			ttPanel.gameObject.SetActive(false);
		}

		public void ToggleObjects()
		{
			if (isObjectsHighlighted == true)
			{
				HighlightObjects_Off();
			}
			else if (isObjectsHighlighted == false)
			{
				HighlightObjects();
			}
		}
	}
}