using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class OffsetScroll : MonoBehaviour
{
    [SerializeField]
    [Range(.01f, 1f)]
    private float lineSpeed;
    [SerializeField]
    private LineRenderer lineR;
    private int offsetCounter;

    void FixedUpdate()
    {
        offsetCounter++;
        AddOffset();
    }

    private void AddOffset()
    {
        if (lineR)
            lineR.material.mainTextureOffset = new Vector2(offsetCounter * lineSpeed, 0);

    }
}
