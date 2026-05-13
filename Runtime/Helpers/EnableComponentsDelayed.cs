using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableComponentsDelayed : MonoBehaviour
{
	public MonoBehaviour[] components;
	public float delayInSecods = 1;

	private void OnEnable()
	{
		foreach (var comp in components)
		{
			comp.enabled = false;
		}
		StartCoroutine(EnableDelayed());
	}

	// Start is called before the first frame update
	void Start()
    {

    }

	private IEnumerator EnableDelayed()
	{
		yield return new WaitForSeconds(delayInSecods);
		foreach (var comp in components)
		{
			comp.enabled = true;
		}
	}
}
