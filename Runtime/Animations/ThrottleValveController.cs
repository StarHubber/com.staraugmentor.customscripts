using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantThrottleController : MonoBehaviour
{
    // =========================
    // LINEARE VENTILE
    // =========================
    [System.Serializable]
    public class Valve
    {
        public Transform transform;
        public Vector3 localMoveDirection = Vector3.down;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 worldMoveDir;
    }

    // =========================
    // ROTIERENDE KLAPPE
    // =========================
    [System.Serializable]
    public class RotatingValve
    {
        public Transform transform; // Drosselklappe
        public Transform pivot; // frei platzierbarer Punkt (Empty GameObject)

        public Vector3 rotationAxis = Vector3.up;
        public float maxAngle = 70f;

        [HideInInspector] public Vector3 startOffset;
        [HideInInspector] public Quaternion startRotation;
    }

    // =========================
    // ARRAYS
    // =========================
    public Valve[] throttleValves;
    public RotatingValve[] rotatingValves;

    // =========================
    // SETTINGS
    // =========================
    public float liftAmount = 0.08f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f; // Grad pro Sekunde

    // =========================
    // STATES
    // =========================
    private enum ThrottleState
    {
        State1, // Konstantdrossel zu, Abgasdrosselklappe offen
        State2, // Konstantdrossel offen, Abgasdrosselklappe offen
        State3  // Konstantdrossel offen, Abgasdrosselklappe zu
    }
  
    private ThrottleState currentState = ThrottleState.State1;
    public int CurrentState { get; private set; } // Zustände für ParticleController

    // State-Methoden für Button-Toggles
    public void EngineBrakeSetState1()
    {
        currentState = ThrottleState.State1;
    }

    public void EngineBrakeSetState2()
    {
        currentState = ThrottleState.State2;
    }
    public void EngineBrakeSetState3()
    {
        currentState = ThrottleState.State3;
    }

    // Animation values (0–1)
    private float linearValue = 0f;
    private float rotationValue = 0f;

    // =========================
    // INIT
    // =========================
    void Start()
    {
        // lineare Ventile
        foreach (var v in throttleValves)
        {
            v.startPos = v.transform.localPosition;
            v.worldMoveDir = v.localMoveDirection;
        }

        // rotierende Ventile
        foreach (var rv in rotatingValves)
        {
            //rv.startOffset = rv.transform.localPosition - rv.pivot.localPosition;
            rv.startRotation = rv.transform.localRotation;
        }
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        CurrentState = (int)currentState + 1; // 1,2,3 für ParticleController

        /*
        // Zustände per Tasten
        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentState = ThrottleState.State1;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            currentState = ThrottleState.State2;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            currentState = ThrottleState.State3;
        */

        // Zielwerte je Zustand
        float targetLinear = 0f;
        float targetRotation = 0f;

        switch (currentState)
        {
            case ThrottleState.State1:
                // Konstantdrossel zu, Abgasdrosselklappe offen
                targetLinear = 0f;
                targetRotation = 0f;
                break;

            case ThrottleState.State2:
                // Konstantdrossel offen, Abgasdrosselklappe offen
                targetLinear = 1f;
                targetRotation = 0f;
                break;

            case ThrottleState.State3:
                // Konstantdrossel offen, Abgasdrosselklappe zu
                targetLinear = 1f;
                targetRotation = 1f;
                break;
        }

        // weiche Bewegung
        linearValue = Mathf.MoveTowards(linearValue, targetLinear, Time.deltaTime * moveSpeed);
        rotationValue = Mathf.MoveTowards(rotationValue, targetRotation, Time.deltaTime * (rotationSpeed / 90f));

        // lineare Ventile bewegen
        foreach (var v in throttleValves)
        {
            v.transform.localPosition =
                v.startPos + v.worldMoveDir * (linearValue * liftAmount);
        }

        // rotierende Klappen bewegen
        foreach (var rv in rotatingValves)
        {
            RotateValve(rv, rotationValue);
        }
    }

    // =========================
    // ROTATION
    // =========================
    void RotateValve(RotatingValve rv, float normalizedValue)
    {
        float angle = normalizedValue * rv.maxAngle;

        // WICHTIG: Achse relativ zum Pivot!
        Vector3 worldAxis = rv.rotationAxis.normalized;
        Quaternion rot = Quaternion.AngleAxis(angle, worldAxis);

        // Position um Pivot drehen
        //rv.transform.localPosition =
        //   rv.pivot.localPosition + rot * rv.startOffset;

        // Rotation der Klappe
        rv.transform.localRotation =
            rot * rv.startRotation;
    }
}