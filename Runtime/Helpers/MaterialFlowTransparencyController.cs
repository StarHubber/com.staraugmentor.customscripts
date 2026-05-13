using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways] // wichtig, damit es auch im Editor bei Timeline funktioniert
public class MaterialFlowTransparencyController : MonoBehaviour
{
    public List<Renderer> targetMaterial;
    public int RenderQueue = 3021;
    public bool animate = false;
    public bool animateColor = false;
    public bool activateWaveManipulator = false;
    public int WaveManipulator = 6;

    public Color ColorFade;
    public Color ColorBFade = new Color(0.22f, 0.57f, 0.95f, 0.0f);
    public Color ColorCFade = new Color(0.85f, 0.98f, 1.0f, 0.0f);

    [Range(0f, 1f)]
    public float alpha = 1f;
    [Range(0f, 10f)]
    public float MainColorAlphaMultiplier = 3.0f;
    public float SekundaerColorAlphaMultiplier = 0.5f;
    public float BaseColorAlphaMultiplier = 0.7f;
    public float HighlightColorAlphaMultiplier = 0.1f;

    private float duration = 1f;

    /*void Awake()
    {
        if (targetMaterial != null)
            targetMaterial = new Material(targetMaterial); // instance
    }*/

    private void OnDisable()
    {
        StopCoroutine(LerpColor());
        StopCoroutine(LerpColorBack());

        foreach (Renderer ren in targetMaterial)
        {

            Material mat = ren.material;

            Color colorA = mat.GetColor("_ColorA");
            //colorA = Color.Lerp(colorA, ColorAFade, Mathf.PingPong(Time.time, 1));
            colorA.a = Mathf.Clamp(0, 0.0f, 1.0f);
            mat.SetColor("_ColorA", colorA);

            Color colorB = mat.GetColor("_ColorB");
            //colorB = Color.Lerp(colorB, ColorBFade, Mathf.PingPong(Time.time, 1));
            colorB.a = Mathf.Clamp(0, 0.0f, 1.0f);
            mat.SetColor("_ColorB", colorB);

            Color colorC = mat.GetColor("_ColorC");
            //colorC = Color.Lerp(colorC, ColorCFade, Mathf.PingPong(Time.time, 1));
            colorC.a = Mathf.Clamp(0, 0.0f, 1.0f);
            mat.SetColor("_ColorC", colorC);

            mat.renderQueue = RenderQueue;
        }
    }

    public void StartFadeCoroutine()
    {
        StartCoroutine(LerpColor());
    }

    public void StartFadeBackCoroutine()
    {
        StartCoroutine(LerpColorBack());
    }

    private void Update()
    {
        if (targetMaterial != null && animate)
        {
            /*Color color = targetMaterial.GetColor("_MainColor");
            color.a = alpha;
            targetMaterial.SetColor("_MainColor", color);

            Color colorReveal = targetMaterial.GetColor("_RevealColor");
            colorReveal.a = alpha;
            targetMaterial.SetColor("_RevealColor", colorReveal);*/

            foreach (Renderer ren in targetMaterial)
            {

                Material mat = ren.material;

                if(activateWaveManipulator)
                    mat.SetInt("_WaveCount", WaveManipulator);

                Color colorA = mat.GetColor("_ColorA");
                colorA.a = Mathf.Clamp(alpha * SekundaerColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_ColorA", colorA);

                Color colorB = mat.GetColor("_ColorB");
                colorB.a = Mathf.Clamp(alpha * MainColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_ColorB", colorB);

                Color baseColor = mat.GetColor("_BaseColor");
                baseColor.a = Mathf.Clamp(alpha * BaseColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_BaseColor", baseColor);

                Color highlightColor = mat.GetColor("_HighlightColor");
                highlightColor.a = Mathf.Clamp(alpha * HighlightColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_HighlightColor", highlightColor);

                Color colorC = mat.GetColor("_ColorC");
                colorC.a = Mathf.Clamp(alpha * SekundaerColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_ColorC", colorC);

                mat.renderQueue = RenderQueue;
            }
        }
    }

    IEnumerator LerpColor()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            foreach (Renderer ren in targetMaterial)
            {
                Material mat = ren.material;

                Color colorB = mat.GetColor("_BaseColor");
                colorB = Color.Lerp(colorB, ColorFade, t);
                colorB.a = Mathf.Clamp(alpha * MainColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_BaseColor", colorB);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator LerpColorBack()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            foreach (Renderer ren in targetMaterial)
            {
                Material mat = ren.material;

                Color colorB = mat.GetColor("_BaseColor");
                colorB = Color.Lerp(ColorFade, ColorBFade, t);
                colorB.a = Mathf.Clamp(alpha * MainColorAlphaMultiplier, 0.0f, 1.0f);
                mat.SetColor("_BaseColor", colorB);
            }

            time += Time.deltaTime;
            yield return null;
        }
    }
}
