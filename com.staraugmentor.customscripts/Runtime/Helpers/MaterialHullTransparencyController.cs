using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways] // wichtig, damit es auch im Editor bei Timeline funktioniert
public class MaterialHullTransparencyController : MonoBehaviour
{
    public List<Material> targetMaterial;
    public int RenderQueue = 3021;
    public bool animate = false;


    [Range(0f, 1f)]
    public float alpha = 1f;

    /*void Awake()
    {
        if (targetMaterial != null)
            targetMaterial = new Material(targetMaterial); // instance
    }*/

    private void Update()
    {
        if (targetMaterial != null && animate)
        {
            foreach (Material ren in targetMaterial)
            {

                //Material mat = ren.material;

                Color color = ren.color;
                color.a = Mathf.Clamp(alpha, 0.0f, 1.0f);
                ren.color = color;

                //ren.renderQueue = RenderQueue;
            }
        }
    }
}
