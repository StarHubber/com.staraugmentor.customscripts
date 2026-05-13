using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparentToRootOnPlay : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(DoDelayedReparenting());
	}

	private IEnumerator DoDelayedReparenting()
	{
		yield return null;
		yield return null;
		yield return null;
		transform.parent = null;
	}
}
