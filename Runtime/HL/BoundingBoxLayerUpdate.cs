using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(BoundingBox))]
public class BoundingBoxLayerUpdate : MonoBehaviour
{
	private BoundingBox box;

	private void Awake()
	{
		box = GetComponent<BoundingBox>();
	}

	// Start is called before the first frame update
	void Start()
    {
		box.Target.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

	public void EnableRaycast(bool enable)
	{
		box.Target.layer = enable ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("Ignore Raycast");
	}
}
