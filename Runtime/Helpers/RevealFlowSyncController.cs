using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteAlways]
public class RevealFlowSyncController : MonoBehaviour
{
    public bool start = true;

    [SerializeField] private float delayBetweenCycles = 1f;
    [SerializeField] private float SpeedMultiplier = 1f;

    private List<RevealFlowController> children = new List<RevealFlowController>();
    private bool isRunning = false;

    private void Update()
    {
        if (start && !isRunning)
        {
            StartCoroutine(LoopReveal());
            isRunning = true;
        }
        else if(!start)
        {
            StopCoroutine(LoopReveal());
            isRunning = false;
        }
    }

    private void OnEnable()
    {
        start = true;
    }

    private void OnDisable()
    {
        start = false;
        StopCoroutine(LoopReveal());
        isRunning = false;
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            var controller = child.GetComponent<RevealFlowController>();
            if (controller != null)
            {
                controller.SpeedMultiplier = SpeedMultiplier;
                children.Add(controller);
            }
        }
    }

    private IEnumerator LoopReveal()
    {
        while (true)
        {
            // Alle Reveals starten
            foreach (var child in children)
            {
                child.BeginReveal();
                //if (resetProgress)
                //    child.progress = 0.0f;
                if (!start) child.StopReveal();
            }

            // Warten bis alle fertig
            yield return new WaitUntil(() => AllFinished());

            // Kurze Pause vor dem Neustart
            yield return new WaitForSeconds(delayBetweenCycles);
        }
    }

    private bool AllFinished()
    {
        foreach (var child in children)
        {
            if (!child.IsDone) return false;
        }
        return true;
    }

}
