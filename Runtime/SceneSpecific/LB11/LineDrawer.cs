using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class LineDrawer : MonoBehaviour
    {
        private GameObject upperSteckerParticle;
        public static LineDrawer Instance;
        [SerializeField] private LineDrawerSettings settings;

        private LineRenderer LineRenderer;

        private ParticleSystem UpperSteckerPS, LowerSteckerPS;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            if (LineRenderer)
                GameObject.Destroy(LineRenderer);

        }

        public void StopActivePS()
        {
            if (!UpperSteckerPS)
                return;

            UpperSteckerPS.Stop();
        }
        public GameObject DrawLineAndReturnGameObject(Stecker upperStecker, Stecker lowerStecker)
        {
            if (upperStecker == null || lowerStecker == null) return null;

            LineRenderer = Instantiate(settings.lineRendererPrefab, this.transform).GetComponent<LineRenderer>();

            SetupLineRenderer(upperStecker, lowerStecker);

            return LineRenderer.gameObject;
        }
        public void SetActiveMat(LineRenderer liner, bool value)
        {
            if (!liner) return;
            if (value)
                liner.material = settings.ActiveMaterial;
            else
                liner.material = settings.PassiveMaterial;

            SetMaterialTiling(liner);

        }

        private void SetMaterialTiling(LineRenderer liner)
        {
            float _distance = Vector3.Distance(liner.GetPosition(0), liner.GetPosition(1));
            liner.material.mainTextureScale = new Vector2(_distance * settings.TilingFactor, 1);
        }

        private void SetupLineRenderer(Stecker upperStecker, Stecker lowerStecker)
        {
            LineRenderer.startWidth = 0.01f;
            LineRenderer.endWidth = 0.01f;

            LineRenderer.positionCount = 2;
            LineRenderer.useWorldSpace = true;

            LineRenderer.SetPosition(0, upperStecker.Highlighter.modelParts[0].GetComponent<Renderer>().bounds.center);
            LineRenderer.SetPosition(1, lowerStecker.Highlighter.modelParts[0].GetComponent<Renderer>().bounds.center);
            LineRenderer.sharedMaterial = settings.ActiveMaterial;
            SetMaterialTiling(LineRenderer);

        }

        public void CreateParticleEffectOnActiveTooltip(Stecker upperTooltipModel)
        {
            if (!upperSteckerParticle)
            {
                upperSteckerParticle = Instantiate(settings.particlePrefab, upperTooltipModel.Highlighter.modelParts[0].transform);

            }
            else
            {
                upperSteckerParticle.transform.SetParent(upperTooltipModel.Highlighter.modelParts[0].transform);
            }

            UpperSteckerPS = upperSteckerParticle.GetComponent<ParticleSystem>();



            UpperSteckerPS.gameObject.SetActive(true);
            UpperSteckerPS.transform.position =
            upperTooltipModel.Highlighter.modelParts[0].GetComponent<Renderer>().bounds.center;
            UpperSteckerPS.startColor = new Color(0, 0.6784314f, 1);
            UpperSteckerPS.Play();
        }
    }
}