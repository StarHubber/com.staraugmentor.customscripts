using System;
using UnityEngine;

public class LB9DifferentialLockVisController : MonoBehaviour
{
	[SerializeField] private LB9DifferentialLock[] orderedLocks = new LB9DifferentialLock[3];

	private void OnEnable()
	{
		foreach (var diffLock in orderedLocks)
		{
			diffLock.LockStateChangeRequested += OnLockStateChangeRequest;
		}
	}

	private void OnDisable()
	{
		foreach (var diffLock in orderedLocks)
		{
			diffLock.LockStateChangeRequested -= OnLockStateChangeRequest;
		}
	}

	private void OnLockStateChangeRequest(LB9DifferentialLock diffLock)
	{
		if (diffLock.Transitioning)
		{
			return;
		}

		var lockIndex = Array.IndexOf(orderedLocks, diffLock);

		if (lockIndex < 0)
		{
			Debug.LogError($"lock target ({diffLock.name}) not registered with controller", this);
			return;
		}

		var desiredState = !diffLock.Locked;
		var nextLock =	lockIndex + 1 >= orderedLocks.Length ? null : orderedLocks[lockIndex + 1];
		var prevLock = lockIndex - 1 < 0 ? null : orderedLocks[lockIndex - 1];

		int idx = lockIndex + 1;
		while (idx < orderedLocks.Length)
		{
			if (orderedLocks[idx].Transitioning)
			{
				return;
			}
			idx++;
		}

		if (desiredState)
		{
			if (prevLock != null && !prevLock.Locked)
			{
				return;
			}
		}
		else
		{
			if (nextLock != null && nextLock.Locked)
			{
				//return;
				nextLock.Toggle();
			}
		}
		diffLock.SetLockedState(desiredState);
	}
}