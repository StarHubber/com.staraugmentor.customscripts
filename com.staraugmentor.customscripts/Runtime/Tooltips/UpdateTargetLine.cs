using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTargetLine : MonoBehaviour
{
	public Transform corners;

	private LineRenderer lineRend;
	private float startWidth;

	private void Awake()
	{
		lineRend = GetComponent<LineRenderer>();
		startWidth = lineRend.startWidth;
		lineRend.positionCount = 2;
	}

	// Update is called once per frame
	private void LateUpdate()
	{
		lineRend.startWidth = startWidth * transform.lossyScale.magnitude;
		if (transform.lossyScale != Vector3.zero)
		{
			// Run in update, because target's positions are set in Update
			DrawLineToClosestCorner();
		}
	}

	private void DrawLineToClosestCorner()
	{
		Vector3 screenTargetPos = Camera.main.WorldToScreenPoint(this.transform.position);

		Vector3 worldMinCornerPos = corners.GetChild(0).position;
		Vector3 screenMinCornerPos = Camera.main.WorldToScreenPoint(worldMinCornerPos);

		foreach (Transform corner in corners)
		{
			Vector3 screenCornerPos = Camera.main.WorldToScreenPoint(corner.position);

			if (Vector3.Distance(screenTargetPos, screenCornerPos) < Vector3.Distance(screenTargetPos, screenMinCornerPos))
			{
				worldMinCornerPos = corner.position;
			}
			screenMinCornerPos = Camera.main.WorldToScreenPoint(worldMinCornerPos);
		}

		lineRend.SetPosition(0, this.transform.position);
		lineRend.SetPosition(1, worldMinCornerPos);
	}
}
