using StarCooperation.Helpers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StarCooperation;

public class LB9DifferentialLock : MonoBehaviour
{
	private const float MaxDuration = 10f;

	[SerializeField] private Image pendingLED;
	[SerializeField] private Image activeLED;
	[SerializeField] private ScriptableEventBool lockStateChangedEvent;
	[SerializeField] private bool raiseChangedAfterTransition;

	[SerializeField, Range(0, MaxDuration)] private float transitionDuration = 1f;

	//[SerializeField, Range(0.1f, 2f)] float blinkInterval = 0.25f;

	private Coroutine blinkRoutine;

	public bool Locked
	{
		get; private set;
	}

	public bool Transitioning
	{
		get; private set;
	}

	public event Action<LB9DifferentialLock> LockStateChangeRequested;

	private void OnEnable()
    {
        LB9DifferentialLockSystemVisController.instance?.SetFadeTime(2, this.gameObject.name);
        LB9DifferentialLockSystemVisController.instance?.SetFadeTime(1, this.gameObject.name);

        SetLockedState(false, true);
	}

    private void OnDisable()
	{
		if (blinkRoutine != null)
		{
			SetLockedState(Locked, true);
		}
	}

	public void Toggle()
	{
		if (Transitioning)
		{
			return;
		}

		LockStateChangeRequested?.Invoke(this);
	}



    public void SetLockedState(bool locked)
	{
		SetLockedState(locked, false);
	}

	private void SetLockedState(bool locked, bool forced)
	{
		if (!forced)
		{
			if (Locked == locked || Transitioning)
			{
				return;
			}
		}
		else
		{
			SetLockedValue(locked, false);
			if (blinkRoutine != null)
			{
				StopCoroutine(blinkRoutine);
			}

			Transitioning = false;
			pendingLED.enabled = false;
			activeLED.enabled = locked;
			return;
		}

		IEnumerator CountDownAndAnimateTransition(float duration)
		{
			//var t = 0f;
			duration = Mathf.Min(duration, MaxDuration);

            pendingLED.enabled = Locked;
            LB9DifferentialLockSystemVisController.instance.SetFadeTime(2, this.gameObject.name);

            yield return new WaitForSeconds(duration);

			//while (t < duration)
			//{
			//	t += Time.deltaTime;
			//	pendingLED.enabled = Mathf.PingPong(t / interval, 1f) < 0.5f;
			//	yield return null;
			//}
			Transitioning = false;
			//pendingLED.enabled = false;
			activeLED.enabled = Locked;
			blinkRoutine = null;
			SetLockedValue(Locked, raiseChangedAfterTransition);
            LB9DifferentialLockSystemVisController.instance.SetFadeTime(1, this.gameObject.name);

        }

        Transitioning = isActiveAndEnabled;
		pendingLED.enabled = Transitioning;
		activeLED.enabled = Locked;

		SetLockedValue(locked, !Transitioning || !raiseChangedAfterTransition);

		if (Transitioning)
		{
			if (blinkRoutine != null)
				StopCoroutine(blinkRoutine);
			blinkRoutine = StartCoroutine(CountDownAndAnimateTransition(transitionDuration));
		}
		else
		{
			activeLED.enabled = Locked;
		}
	}

	private void SetLockedValue(bool value, bool raiseEvent)
	{
		Locked = value;
		if (raiseEvent && lockStateChangedEvent != null)
		{
			lockStateChangedEvent.Raise(Locked);
		}
	}
}