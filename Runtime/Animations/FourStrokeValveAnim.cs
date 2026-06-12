using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourStrokeValveAnim : MonoBehaviour
{
    [System.Serializable]
    public class Valve
    {
        public Transform transform;

        // Bewegungsrichtung im lokalen Raum
        public Vector3 localMoveDirection = Vector3.up;

        [Range(0f, 1f)]
        public float phaseOffset;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 worldMoveDir;
    }

    public Valve[] intakeValves;   // 2 Einlassventile
    public Valve[] exhaustValves;  // 2 Auslassventile

    [System.Serializable]
    public class Piston
    {
        public Transform transform;

        public Vector3 localMoveDirection = Vector3.up;

        public float strokeLength = 0.1f; // Hubhöhe

        [Range(0f, 1f)]
        public float phaseOffset;

        [HideInInspector] public Vector3 startPos;
        [HideInInspector] public Vector3 worldMoveDir;
    }

    public Piston[] pistons;

    public float cycleDuration = 2f; // Dauer für 4‑Takt-Zyklus
    public float valveLift = 0.05f;  // Ventilhub
    public float CurrentCycle { get; private set; } // Für Partikel

    private float timer;

    public ConstantThrottleController throttleController;

    void Start()
    {
        foreach (var v in intakeValves)
            InitValve(v);

        foreach (var v in exhaustValves)
            InitValve(v);

        foreach (var p in pistons)
            InitPiston(p);
    }

    void InitValve(Valve v)
    {
        v.startPos = v.transform.position;

        // lokale Richtung → Welt-Richtung umrechnen
        v.worldMoveDir = v.transform.TransformDirection(v.localMoveDirection).normalized;
    }

    void InitPiston(Piston p)
    {
        p.startPos = p.transform.position;
        p.worldMoveDir = p.transform.TransformDirection(p.localMoveDirection).normalized;
    }

    void Update()
    {
        // aktuellen Takt bestimmen (vor Update!)
        float rawCycle = (timer % cycleDuration) / cycleDuration; // Normierter Zyklus (0–1, wiederholt sich endlos)

        int state = throttleController.CurrentState;
        float speedFactor = GetSpeedFactor(state);

        // Timer langsamer laufen lassen
        timer += Time.deltaTime * speedFactor;

        // neuen Cycle berechnen
        float cycle = (timer % cycleDuration) / cycleDuration;
        CurrentCycle = cycle; // Für Partikel

        AnimateValves(cycle);
        AnimatePistons(cycle);
    }

    int GetStroke(float cycle)
    {
        if (cycle < 0.25f) return 1;
        if (cycle < 0.5f) return 2;
        if (cycle < 0.75f) return 3;
        return 4;
    }

    float GetSpeedFactor(int state)
    {
        switch (state)
        {
            case 1: return 1.0f; // normal
            case 2: return 0.6f; // langsamer (Drossel aktiv)
            case 3: return 0.3f; // 2am langsamsten (max. Bremswirkung)
        }
        return 1f;
    }


    void AnimateValves(float cycle)
    {
        foreach (var v in intakeValves)
        {
            float shiftedCycle = (cycle + v.phaseOffset) % 1f;

            float lift = 0f;

            if (shiftedCycle < 0.25f)
            {
                float t = shiftedCycle / 0.25f;
                lift = SmoothLift(t);
            }

            v.transform.position = v.startPos + v.worldMoveDir * (lift * valveLift);
        }

        foreach (var v in exhaustValves)
        {
            float shiftedCycle = (cycle + v.phaseOffset) % 1f;

            float lift = 0f;

            if (shiftedCycle >= 0.75f)
            {
                float t = (shiftedCycle - 0.75f) / 0.25f;
                lift = SmoothLift(t);
            }

            v.transform.position = v.startPos + v.worldMoveDir * (lift * valveLift);
        }
    }

    void SetValves(Valve[] valves, float lift)
    {
        foreach (var v in valves)
        {
            v.transform.position = v.startPos + v.worldMoveDir * (lift * valveLift);
        }
    }

    float SmoothLift(float t)
    {
        // sanftes Öffnen/Schließen (Ventilkurve)
        return Mathf.Sin(t * Mathf.PI);
    }


    void AnimatePistons(float cycle)
    {
        foreach (var p in pistons)
        {
            float shiftedCycle = (cycle + p.phaseOffset) % 1f;

            float value = (1f - Mathf.Cos(shiftedCycle * 4f * Mathf.PI)) / 2f;

            p.transform.position =
                p.startPos + p.worldMoveDir * (value * p.strokeLength);
        }
    }


    void SetPistons(Piston[] pistons, float value)
    {
        foreach (var p in pistons)
        {
            p.transform.position =
                p.startPos + p.worldMoveDir * (value * p.strokeLength);
        }
    }

}

