using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjectVisibility : MonoBehaviour
{
    [Header("Drag & Drop dein Bauteil hier rein")]
    public GameObject targetObject;

    public KeyCode toggleKey = KeyCode.T;

    private bool isVisible = true;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
            targetObject.SetActive(isVisible);
        }
    }
}