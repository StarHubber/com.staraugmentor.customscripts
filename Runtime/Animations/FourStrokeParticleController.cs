using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourStrokeParticleController : MonoBehaviour
{
    public FourStrokeValveAnim engine; // Referenz auf Hauptskript
    public ConstantThrottleController throttleController;

    [System.Serializable]
    public class CylinderParticles
    {
        [Range(0f, 1f)]
        public float phaseOffset; // gleich wie Kolben!

        public ParticleSystem[] intakeParticles;
        public ParticleSystem[] exhaustParticles;
        public ParticleSystem[] compressionParticles;
        public ParticleSystem[] throttleParticles;

        [HideInInspector] public int lastStroke = -1;
    }

    public CylinderParticles[] cylinders;

    void Update()
    {
        float baseCycle = engine.CurrentCycle;
        int state = throttleController.CurrentState;

        foreach (var cyl in cylinders)
        {
            // eigener Zyklus pro Zylinder
            float cycle = (baseCycle + cyl.phaseOffset) % 1f;
            int stroke = GetStroke(cycle);

            if (stroke != cyl.lastStroke)
            {
                cyl.lastStroke = stroke;
                UpdateParticlesForCylinder(cyl, stroke, state);
            }
        }
    }

    int GetStroke(float cycle)
    {
        if (cycle < 0.25f) return 1; // Ansaugen
        if (cycle < 0.5f) return 2;  // Verdichten
        if (cycle < 0.75f) return 3; // Arbeiten
        return 4;                    // Ausstoßen
    }

    void UpdateParticlesForCylinder(CylinderParticles cyl, int stroke, int state)
    {
        // alles stoppen
        StopArray(cyl.intakeParticles);
        StopArray(cyl.exhaustParticles);
        StopArray(cyl.compressionParticles);
        StopArray(cyl.throttleParticles);

        switch (state)
        {
            // =========================
            // ZUSTAND 1
            // Einlass + Auslass
            // =========================
            case 1:

                if (stroke == 1)
                    PlayArray(cyl.intakeParticles);

                if (stroke == 4)
                    PlayArray(cyl.exhaustParticles);

                break;

            // =========================
            // ZUSTAND 2
            // Einlass + Konstantdrossel
            // =========================
            case 2:

                if (stroke == 1)
                    PlayArray(cyl.intakeParticles);

                if (stroke == 2)
                {
                    PlayArray(cyl.compressionParticles);
                    PlayArray(cyl.throttleParticles);
                }

                break;

            // =========================
            // ZUSTAND 3
            // gleich wie Zustand 2
            // =========================
            case 3:

                if (stroke == 1)
                    PlayArray(cyl.intakeParticles);

                if (stroke == 2)
                {
                    PlayArray(cyl.compressionParticles);
                    PlayArray(cyl.throttleParticles);
                }

                break;
        }
    }

    void PlayArray(ParticleSystem[] systems)
    {
        if (systems == null) return;

        foreach (var ps in systems)
        {
            if (ps != null)
                ps.Play();
        }
    }

    void StopArray(ParticleSystem[] systems)
    {
        if (systems == null) return;

        foreach (var ps in systems)
        {
            if (ps != null)
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }


}

