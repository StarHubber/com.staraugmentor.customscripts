using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequenceAnimationController : MonoBehaviour
{
    private bool cancelAnimation = false;

    private Dictionary<Renderer, Color> originalRendererColors =
    new Dictionary<Renderer, Color>();

    private Dictionary<GameObject, bool> originalStates =
    new Dictionary<GameObject, bool>();

    private Dictionary<Transform, TransformState> originalTransforms =
    new Dictionary<Transform, TransformState>();

    public Toggle startToggle;

    [Header("Loop Settings")]
    public float loopPause = 1f;

    [Header("Animation Steps")]
    public List<AnimationStep> steps = new List<AnimationStep>();

    private Coroutine animationRoutine;

    private void Start()
    {
        CacheOriginalStates();

        if (startToggle != null)
            startToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void CacheOriginalStates()
    {
        originalStates.Clear();
        originalTransforms.Clear();
        originalRendererColors.Clear();

        foreach (var step in steps)
        {
            foreach (var obj in step.targetObjects)
            {
                if (obj != null && !originalStates.ContainsKey(obj))
                    originalStates.Add(obj, obj.activeSelf);

                Renderer renderer = obj.GetComponentInChildren<Renderer>();

                if (renderer != null &&
                    renderer.material.HasProperty("_Color") &&
                    !originalRendererColors.ContainsKey(renderer))
                {
                    originalRendererColors.Add(
                        renderer,
                        renderer.material.color);
                }
            }

            foreach (var obj in step.transformObjects)
            {
                if (obj != null &&
                    !originalTransforms.ContainsKey(obj.transform))
                {
                    originalTransforms.Add(
                        obj.transform,
                        new TransformState(obj.transform)
                    );
                }
            }
        }
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            StartSequence();
        }
        else
        {
            StopSequenceAndReset();
        }
    }

    public void StartSequence()
    {
        StopSequenceAndReset();

        cancelAnimation = false;
        animationRoutine = StartCoroutine(SequenceLoop());
    }

    public void StopSequenceAndReset()
    {
        cancelAnimation = true;

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        ResetState();
    }

    IEnumerator SequenceLoop()
    {
        while (true)
        {
            foreach (var step in steps)
            {
                switch (step.stepType)
                {
                    case AnimationStepType.BlinkAndHide:
                        yield return StartCoroutine(BlinkAndHide(step));
                        break;

                    case AnimationStepType.Particle:
                        yield return StartCoroutine(PlayParticle(step));
                        break;

                    case AnimationStepType.Transform:
                        yield return StartCoroutine(AnimateTransform(step));
                        break;
                }

                yield return new WaitForSeconds(step.delayAfterStep);
            }

            // Erst Pause
            yield return new WaitForSeconds(loopPause);

            // Dann zurücksetzen
            ResetState();

            // Ein Frame warten, damit die Änderungen sichtbar werden
            yield return null;
        }
    }

    IEnumerator BlinkAndHide(AnimationStep step)
    {
        List<Renderer> renderers = new List<Renderer>();
        List<Color> originalColors = new List<Color>();

        foreach (GameObject obj in step.targetObjects)
        {
            if (obj == null)
                continue;

            Renderer renderer = obj.GetComponentInChildren<Renderer>();

            if (renderer == null)
                continue;

            renderers.Add(renderer);

            if (renderer.material.HasProperty("_Color"))
                originalColors.Add(renderer.material.color);
            else
                originalColors.Add(Color.white);
        }

        float timer = 0f;
        bool blinkState = false;

        while (timer < step.blinkDuration)
        {
            // Sofort abbrechen wenn anderes Panel gewählt wurde
            if (cancelAnimation)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (renderers[i] != null &&
                        renderers[i].material.HasProperty("_Color"))
                    {
                        renderers[i].material.color = originalColors[i];
                    }
                }

                yield break;
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null)
                    continue;

                if (renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color =
                        blinkState ? step.blinkColor : originalColors[i];
                }
            }

            blinkState = !blinkState;

            yield return new WaitForSeconds(step.blinkInterval);

            timer += step.blinkInterval;
        }

        // Nochmals prüfen bevor ausgeblendet wird
        if (cancelAnimation)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] != null &&
                    renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color = originalColors[i];
                }
            }

            yield break;
        }

        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null &&
                renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }

        foreach (GameObject obj in step.targetObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    IEnumerator PlayParticle(AnimationStep step)
    {
        if (step.particleSystem != null)
        {
            step.particleSystem.gameObject.SetActive(true);
            step.particleSystem.Play();

            yield return new WaitForSeconds(step.particleDuration);

            step.particleSystem.Stop(true,
                ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void ResetState()
    {
        foreach (var pair in originalStates)
        {
            if (pair.Key != null)
            {
                pair.Key.SetActive(pair.Value);

                Renderer[] renderers =
                    pair.Key.GetComponentsInChildren<Renderer>(true);

                foreach (var r in renderers)
                    r.enabled = true;              
            }            
        }

        foreach (var pair in originalTransforms)
        {
            if (pair.Key != null)
            {
                pair.Key.localPosition = pair.Value.position;
                pair.Key.localRotation = pair.Value.rotation;
                pair.Key.localScale = pair.Value.scale;
            }
        }

        foreach (var step in steps)
        {
            if (step.particleSystem != null)
            {
                step.particleSystem.Stop(
                    true,
                    ParticleSystemStopBehavior.StopEmittingAndClear);

                step.particleSystem.gameObject.SetActive(false);
            }
        }

        foreach (var pair in originalRendererColors)
        {
            if (pair.Key != null &&
                pair.Key.material.HasProperty("_Color"))
            {
                pair.Key.material.color = pair.Value;
            }
        }
    }

    IEnumerator AnimateTransform(AnimationStep step)
    {
        float timer = 0f;

        List<Vector3> startPos = new List<Vector3>();
        List<Quaternion> startRot = new List<Quaternion>();
        List<Vector3> startScale = new List<Vector3>();

        foreach (var obj in step.transformObjects)
        {
            if (obj == null)
                continue;

            startPos.Add(obj.transform.localPosition);
            startRot.Add(obj.transform.localRotation);
            startScale.Add(obj.transform.localScale);
        }

        while (timer < step.transformDuration)
        {
            // Sofort abbrechen
            if (cancelAnimation)
            {
                yield break;
            }

            float t = timer / step.transformDuration;

            for (int i = 0; i < step.transformObjects.Count; i++)
            {
                GameObject obj = step.transformObjects[i];

                if (obj == null)
                    continue;

                obj.transform.localPosition =
                    Vector3.Lerp(
                        startPos[i],
                        step.targetPosition,
                        t);

                obj.transform.localRotation =
                    Quaternion.Lerp(
                        startRot[i],
                        Quaternion.Euler(step.targetRotation),
                        t);

                obj.transform.localScale =
                    Vector3.Lerp(
                        startScale[i],
                        step.targetScale,
                        t);
            }

            timer += Time.deltaTime;

            yield return null;
        }

        // Falls während der letzten Frames abgebrochen wurde
        if (cancelAnimation)
        {
            yield break;
        }

        foreach (var obj in step.transformObjects)
        {
            if (obj == null)
                continue;

            obj.transform.localPosition =
                step.targetPosition;

            obj.transform.localRotation =
                Quaternion.Euler(step.targetRotation);

            obj.transform.localScale =
                step.targetScale;
        }
    }
}

public enum AnimationStepType
{
    BlinkAndHide,
    Particle,
    Transform
}

[System.Serializable]
public class AnimationStep
{
    public AnimationStepType stepType;

    [Header("Blink And Hide")]
    public List<GameObject> targetObjects = new List<GameObject>();

    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    [Header("Blink Color")]
    public Color blinkColor = Color.yellow;

    [Header("Particle")]
    public ParticleSystem particleSystem;

    public float particleDuration = 2f;

    [Header("General")]
    public float delayAfterStep = 0.5f;

    [Header("Transform")]
    public List<GameObject> transformObjects =
    new List<GameObject>();

    public Vector3 targetPosition;

    public Vector3 targetRotation;

    public Vector3 targetScale = Vector3.one;

    public float transformDuration = 1f;
}

[System.Serializable]
public class TransformState
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformState(Transform t)
    {
        position = t.localPosition;
        rotation = t.localRotation;
        scale = t.localScale;
    }
}