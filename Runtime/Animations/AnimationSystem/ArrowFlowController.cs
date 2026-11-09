using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ArrowFlowController : MonoBehaviour
{
    [Header("Zugehöriger Toggle")]
    public Toggle targetToggle;

    [Header("Animationsschritte")]
    public ArrowStep[] animationSteps;

    [Header("Farben")]
    public Color normalColor = Color.white;

    [Header("Farbabschnitte")]
    public ColorSection[] colorSections;

    [Header("Animation")]
    public float highlightTime = 0.2f;

    private Coroutine animationRoutine;
    private bool isRunning;

    [System.Serializable]
    public class ColorSection
    {
        public int startIndex;
        public int endIndex;
        public Color highlightColor = Color.cyan;
    }

    [System.Serializable]
    public class ArrowStep
    {
        public Renderer[] arrows;
    }

    private void Start()
    {
        SetArrowsVisible(false);

        if (targetToggle != null)
        {
            targetToggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void OnDestroy()
    {
        if (targetToggle != null)
        {
            targetToggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
            ActivateFlow();
        else
            DeactivateFlow();
    }

    public void ActivateFlow()
    {
        if (isRunning)
            return;

        isRunning = true;

        // Sicherheitshalber zunächst alle ausblenden
        SetArrowsVisible(false);

        animationRoutine = StartCoroutine(FlowAnimation());
    }

    public void DeactivateFlow()
    {
        isRunning = false;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        ResetArrowColors();
        SetArrowsVisible(false);
    }

    IEnumerator FlowAnimation()
    {
        while (isRunning)
        {
            for (int i = 0; i < animationSteps.Length; i++)
            {
                HideAllArrows();

                ArrowStep step = animationSteps[i];

                Color currentColor = GetColorForArrow(i);

                foreach (Renderer arrow in step.arrows)
                {
                    if (arrow == null)
                        continue;

                    arrow.gameObject.SetActive(true);
                    arrow.material.color = currentColor;
                }

                yield return new WaitForSeconds(highlightTime);
            }
        }
    }

    private void HideAllArrows()
    {
        foreach (ArrowStep step in animationSteps)
        {
            foreach (Renderer arrow in step.arrows)
            {
                if (arrow == null)
                    continue;

                arrow.gameObject.SetActive(false);
                arrow.material.color = normalColor;
            }
        }
    }

    private Color GetColorForArrow(int stepIndex)
    {
        foreach (ColorSection section in colorSections)
        {
            if (stepIndex >= section.startIndex &&
                stepIndex <= section.endIndex)
            {
                return section.highlightColor;
            }
        }

        return Color.cyan;
    }

    private void ResetArrowColors()
    {
        foreach (ArrowStep step in animationSteps)
        {
            foreach (Renderer arrow in step.arrows)
            {
                if (arrow == null)
                    continue;

                arrow.material.color = normalColor;
            }
        }
    }

    private void SetArrowsVisible(bool visible)
    {
        foreach (ArrowStep step in animationSteps)
        {
            foreach (Renderer arrow in step.arrows)
            {
                if (arrow == null)
                    continue;

                arrow.gameObject.SetActive(visible);
            }
        }
    }
}