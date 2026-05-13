using Microsoft.MixedReality.Toolkit.UI;
using NaughtyAttributes;
using StarCooperation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StarCooperation.Localization.explode_auto;

namespace StarCooperation.Localization
{
    public class explode_auto : MonoBehaviour
    {
        [Serializable]
        public class TooltipRef
        {
            public Tooltip tooltip;
            public GameObject target;
            public GameObject go;
        }

        //tooltip
        public int rows = 3;             // Anzahl der Reihen im Rechteck
        public int columns = 3;          // Anzahl der Spalten im Rechteck
        public float spacing = 2f;       // Abstand zwischen den Tooltips
        public Vector3 areaCenter = new Vector3(0, 0, 0);  // Zentrum des Rechtecks
        public Vector3 areaSize = new Vector3(10, 10, 10);  // Größe des Rechtecks
        public float animationSpeed = 2f; // Geschwindigkeit der Animation

        private GameObject[] tooltips;    // Array zum Speichern der Tooltips
        private Vector3[] targetPositions; // Zielpositionen für jedes Tooltip
        private Vector3[] initialPositions; // Ausgangspositionen für jedes Tooltip
        private bool isAdjusting = false;

        //gridVariables
        private float gridSpacing = 1f;  // Abstand zwischen den Meshes im Grid
        private int gridSize = 4;       // Größe des Grids (10x10, 20x20, etc.)
        //public float animationSpeed = 2f; // Geschwindigkeit der Animation


        // Öffentliche Variablen für das Skript
        public List<TooltipRef> TooltipsRef;
        public Tooltip tooltip;
        public float explosionDistance = 0.3f; // Maximale Distanz für die Explosion
        public float explosionSpeed = 2f;    // Geschwindigkeit, mit der sich die Teile bewegen
        public float explosionDuration = 1.0f; // Dauer der Explosion in Sekunden

        public Slider distanceSlider;        // Slider für die Distanzsteuerung

        private List<Transform> allChildren = new List<Transform>();  // Liste für alle Kinder und Unterkinder
        private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>();  // Ursprüngliche Positionen der Teile

        private bool isExploding = false;     // Flag, um zu kontrollieren, ob die Explosion läuft
        private float timeElapsed = 0f;       // Zeit, die seit Beginn der Explosion vergangen ist
        private string guid;
        private float offset = 1;

        [SerializeField] float separationStrength = 8.0f;

        Vector3 explosionCenter;

        [SerializeField] float resetDuration = 1.2f;

        [SerializeField] AnimationCurve resetCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        bool isResetting;
        float resetElapsed;

        void Start()
        {
            isExploding = false;
            //SetUpTooltips(TooltipsRef);
            //gridAnimation(this.gameObject);

            guid = tooltip.Guid;

            // Falls ein Slider zugewiesen wurde, füge eine Listener-Methode hinzu
            if (distanceSlider != null)
            {
                distanceSlider.onValueChanged.AddListener(OnDistanceChanged);
            }
        }

        private void GetAllChildren(List<TooltipRef> tooltipsRef)
        {
            foreach (TooltipRef tooltip in tooltipsRef)
            {
                //allChildren.Add(child);
                allChildren.AddRange(tooltip.go.GetComponentsInChildren<Transform>().ToList());
                // Rekursiver Aufruf für jedes Kind, um auch Unterkinder zu finden
                //GetAllChildren(child);
            }
        }

        void Update()
        {
            //if(isAdjusting)
            //SlideTooltips();

            if (isExploding)
            {
                AnimateExplosion();
            }
            else if (isResetting)
                AnimateReset();
        }

        private void AnimateReset()
        {
            resetElapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(resetElapsed / resetDuration);
            //float smooth = Mathf.SmoothStep(0f, 1f, progress);

            float t = resetCurve.Evaluate(progress);

            foreach (var child in allChildren)
            {
                child.position = Vector3.Lerp(
                    child.position,
                    originalPositions[child],
                    t
                );
            }

            if (progress >= 1f)
            {
                isResetting = false;

                // Exakt zurücksetzen (verhindert Drift)
                foreach (var child in allChildren)
                    child.position = originalPositions[child];
            }
        }


        public void StartReset()
        {
            resetElapsed = 0f;
            isResetting = true;
        }

        private void AnimateTooltips()
        {
            // Animieren der Tooltips
            for (int i = 0; i < TooltipsRef.Count; i++)
            {
                // Berechne eine Animation (Lerp zwischen der Ausgangs- und Zielposition)
                Vector3 newPos = Vector3.Lerp(initialPositions[i], targetPositions[i], Mathf.PingPong(Time.time * animationSpeed, 1f));
                tooltips[i].transform.position = newPos;
            }
        }

        [Button]
        // Methode, um die Explosion zu starten
        public void StartExplosion(string guidEvent)
        {
            if (guidEvent == guid)
            {
                isExploding = true;
                timeElapsed = 0f;
            }
        }

        void CalculateExplosionCenter()
        {
            explosionCenter = Vector3.zero;

            foreach (var child in allChildren)
                explosionCenter += originalPositions[child];

            explosionCenter /= allChildren.Count;
        }

        // Methode für die Explosion-Animation
        private void AnimateExplosion()
        {
            SetTransformations();
            timeElapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(timeElapsed / explosionDuration);

            float t = resetCurve.Evaluate(progress);

            float currentDistance = Mathf.Lerp(0f, explosionDistance, t);

            // === 1. Radiale Grundbewegung ===
            foreach (var child in allChildren)
            {
                Vector3 direction = (originalPositions[child] - explosionCenter).normalized;
                child.position = originalPositions[child] + direction * currentDistance;
            }

            // === 2. Abstoßung zwischen Meshes ===
            if (progress > 0.9f)
            {
                for (int i = 0; i < allChildren.Count; i++)
                {
                    Transform a = allChildren[i];
                    Renderer ra = a.GetComponent<Renderer>();
                    if (ra == null) continue;

                    float radiusA = ra.bounds.extents.magnitude;

                    for (int j = i + 1; j < allChildren.Count; j++)
                    {
                        Transform b = allChildren[j];
                        Renderer rb = b.GetComponent<Renderer>();
                        if (rb == null) continue;

                        float radiusB = rb.bounds.extents.magnitude;

                        Vector3 delta = a.position - b.position;
                        float distance = delta.magnitude;
                        float minDistance = radiusA + radiusB;

                        if (distance < minDistance && distance > 0.0001f)
                        {
                            Vector3 push = delta.normalized * (minDistance - distance);
                            //push *= separationStrength * Time.deltaTime * 0.5f;

                            a.position += push * separationStrength * Time.deltaTime;
                            b.position -= push * separationStrength * Time.deltaTime;
                        }
                    }
                }
            }

            // Stoppe die Explosion nach der festgelegten Dauer
            if (progress >= 1f)
            {
                isExploding = false;
                isAdjusting = true;

                InverseTooltips();
            }

        }

        private void SetTransformations()
        {
            if (allChildren.Count > 0)
                return;
            // Finde alle Kinder und Unterkinder des GameObjects
            // Finde alle Kinder und Unterkinder des GameObjects
            //GetAllChildren(transform);
            GetAllChildren(TooltipsRef);
            //GetAllChildren(transform);

            // Speichere die ursprünglichen Positionen der Teile
            foreach (var child in allChildren)
            {
                originalPositions[child] = child.position;
            }

            CalculateExplosionCenter();
        }

        void ApplySeparation()
        {
            for (int i = 0; i < allChildren.Count; i++)
            {
                Transform a = allChildren[i];
                Renderer ra = a.GetComponent<Renderer>();
                if (ra == null) continue;

                float radiusA = ra.bounds.extents.magnitude;

                for (int j = i + 1; j < allChildren.Count; j++)
                {
                    Transform b = allChildren[j];
                    Renderer rb = b.GetComponent<Renderer>();
                    if (rb == null) continue;

                    float radiusB = rb.bounds.extents.magnitude;

                    Vector3 delta = a.position - b.position;
                    float distance = delta.magnitude;
                    float minDistance = radiusA + radiusB;

                    if (distance < minDistance && distance > 0.0001f)
                    {
                        Vector3 push = delta.normalized * (minDistance - distance);

                        a.position += push * separationStrength * Time.deltaTime;
                        b.position -= push * separationStrength * Time.deltaTime;
                    }
                }
            }
        }


        private void InverseTooltips()
        {
            foreach (var tool in TooltipsRef)
            {
                SetTooltips(tool.go, offset);
            }
        }

        private void SlideTooltips()
        {
            timeElapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(timeElapsed / offset);
            float current = Mathf.Lerp(0f, offset, progress);

            foreach (var tool in TooltipsRef)
            {
                tool.tooltip.gameObject.transform.GetChild(0).transform.Translate(new Vector3(current, 0, 0));
                offset = returnInverse(offset);
            }
            if (progress >= 1f)
                isAdjusting = false;
        }

        /*            timeElapsed += Time.deltaTime;

            // Berechne die Positionsverschiebung basierend auf der Zeit und der maximalen Distanz
            float progress = Mathf.Clamp01(timeElapsed / explosionDuration);
            float currentDistance = Mathf.Lerp(0f, explosionDistance, progress);

            // Bewege alle Kinder (Teile des Bauteils) nach au�en, aber ber�cksichtige die Gr��e der Meshes
            foreach (var child in allChildren)
            {
                // Hole den BoxCollider (oder MeshRenderer, wenn kein Collider vorhanden ist) des aktuellen Meshes, um die Gr��e zu ermitteln
                var meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // Berechne den Abstand basierend auf der Gr��e des Meshes
                    float meshSize = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);

                    // Berechne die Richtung und Zielposition f�r das Mesh
                    Vector3 direction = (child.position - transform.position).normalized;
                    Vector3 targetPosition = originalPositions[child] + direction * (currentDistance + meshSize * 0.5f); // Abstand ber�cksichtigen

                    // Setze die neue Position des Kindes
                    child.position = targetPosition;

                    // Optional: Stelle Tooltips oder andere visuelle Effekte ein, basierend auf der Position des Meshes
                    SetTooltips(child.gameObject, offset);
                }
            }*/

        public Vector3 GetGeometricCenterOfMass(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<MeshRenderer>();

            if (renderers.Length == 0)
                return root.transform.position;

            Vector3 sum = Vector3.zero;

            foreach (var r in renderers)
                sum += r.bounds.center;

            return sum / renderers.Length;
        }

        private Vector3 calcYBounds(GameObject go)
        {
            //var maxY = go.GetComponentsInChildren<MeshRenderer>().Max(x => x.bounds.max.y-0.05f);
            var position = GetGeometricCenterOfMass(go);
            return new Vector3(position.x, position.y, position.z);
        }
        private void SetTooltips(GameObject goRef, float offsetPar)
        {
            var tooltipRef = TooltipsRef.Where(x => x.go?.name == goRef.name).FirstOrDefault();

            if (tooltipRef != null)
            {
                var massYBounds = calcYBounds(tooltipRef.go);

                tooltipRef.tooltip.gameObject.transform.GetChild(0).transform.position = new Vector3(massYBounds.x + offsetPar, massYBounds.y + 0.5f, massYBounds.z);
                tooltipRef.target.gameObject.transform.position = massYBounds;
                offset = returnInverse(offsetPar);

                tooltipRef.target.SetActive(true);
                tooltipRef.tooltip.gameObject.SetActive(true);
            }
        }

        private float returnInverse(float i)
        {
            return (i * (-1));
        }

        [Button]
        // Methode, um die Explosion zurückzusetzen
        public void ResetExplosion()
        {
            // Setze alle Bauteile auf ihre ursprünglichen Positionen zurück
            foreach (var child in allChildren)
            {
                child.position = originalPositions[child];
            }
        }

        // Diese Methode wird aufgerufen, wenn der Slider-Wert geändert wird
        private void OnDistanceChanged(float value)
        {
            explosionDistance = value; // Setze die Explosion-Distanz basierend auf dem Slider-Wert
        }

        // Rekursive Methode, um alle Kinder und Unterkinder zu finden
        private void GetAllChildren(Transform parent)
        {
            //foreach (Transform child in parent)
            //{
            //allChildren.Add(child);
            allChildren = parent.GetComponentsInChildren<Transform>().ToList();
            // Rekursiver Aufruf für jedes Kind, um auch Unterkinder zu finden
            //GetAllChildren(child);
            //}
        }
    }

}