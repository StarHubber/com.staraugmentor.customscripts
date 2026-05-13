using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
	[Header("Scene Reference")]
	public Transform transformToFollow;

	[Header("What to do")]
	public bool followPosition = true;
	public bool followRotation = true;
	public bool followScaling = true;

	[Header("Position Follower")]
	public bool keepPosOffsetX = true;
	public bool keepPosOffsetY = true;
	public bool keepPosOffsetZ = true;

	[Header("Rotation Follower")]
	public bool keepRotOffsetX = true;
	public bool keepRotOffsetY = true;
	public bool keepRotOffsetZ = true;

	[Header("Scale")]
	public float scaleMin = 0.1f;

	private Vector3 posOffset;
	private Vector3 rotOffset;

	// Start is called before the first frame update
	private void Awake()
	{
		// Init position offset
		posOffset = this.transform.position - transformToFollow.position;
		posOffset.x = keepPosOffsetX ? posOffset.x : 0;
		posOffset.y = keepPosOffsetY ? posOffset.y : 0;
		posOffset.z = keepPosOffsetZ ? posOffset.z : 0;

		// Init rotation offset
		rotOffset = this.transform.eulerAngles - transformToFollow.rotation.eulerAngles;
		rotOffset.x = keepRotOffsetX ? rotOffset.x : 0;
		rotOffset.y = keepRotOffsetY ? rotOffset.y : 0;
		rotOffset.z = keepRotOffsetZ ? rotOffset.z : 0;
	}

	private void LateUpdate()
	{
		// Position
		if (followPosition)
		{
			transform.position = transformToFollow.position;
			transform.Translate(posOffset, Space.Self);
		}

		// Rotation
		if (followRotation)
		{
			transform.rotation = transformToFollow.rotation;
			transform.Rotate(rotOffset, Space.Self);
		}

		// Scale
		if (followScaling)
		{
			transform.localScale = transformToFollow.localScale;
			if (transform.localScale.x < scaleMin)  // Todo: maybe check for magnitude, not just x value. But parent should be (1,1,1).
			{
				transform.localScale = Vector3.one * scaleMin;
			}
		}
	}
}
