using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class DetailAutoExploder: MonoBehaviour
    {
        class RendererData
        {
            public Renderer Target { get; private set; }
            public Material[] Materials { get; private set; }

            public static RendererData CopyFrom(Renderer target)
            {
                var rs = new RendererData
                {
                    Target = target,
                    Materials = target.sharedMaterials
                };
                return rs;
            }

            public void Revert()
            {
                for (int i = 0; i < Materials.Length; i++)
                {
                    Target.SetPropertyBlock(null, i);
                }
            }
        }

        class TooltipData
        {
            Tooltip target;
            public Vector3 Position { get; private set; }
            public bool EnabledState { get; private set; }
            public List<Vector3> TargetLinePositions { get; private set; } = new List<Vector3>();

            public static TooltipData CopyFrom(Tooltip target)
            {
                var tts = new TooltipData
                {
                    target = target,
                    Position = target.transform.position,
                    EnabledState = target.gameObject.activeInHierarchy,
                };
                for (var i = 0; i < target.targetLines.transform.childCount; i++)
                {
                    tts.TargetLinePositions.Add(target.targetLines.transform.GetChild(0).position);
                }
                return tts;
            }

            public void Revert()
            {
                target.transform.position = Position;
                target.gameObject.SetActive(EnabledState);
                for (var i = 0; i < target.targetLines.transform.childCount; i++)
                {
                    target.targetLines.transform.GetChild(0).position = TargetLinePositions[i];
                }
            }

            public void ApplyCustom(Vector3 position, bool enabledState)
            {
                target.transform.position = position;
                target.gameObject.SetActive(enabledState);
                for (var i = 0; i < target.targetLines.transform.childCount; i++)
                {
                    target.targetLines.transform.GetChild(0).position = TargetLinePositions[i];
                }
            }

        }

        static readonly int ColorPropKey = Shader.PropertyToID("_Color");

        [SerializeField] List<MeshRenderer> renderers = new List<MeshRenderer>();
        [SerializeField] List<Tooltip> tooltips = new List<Tooltip>();

        [SerializeField] Transform center;
        [SerializeField] float duration = 2f;
        [SerializeField] float delay = 1f;
        [SerializeField, Range(0, 1f)] float tooltipActivationTime = 0.75f;
        [SerializeField] Material fadeMaterial;

        List<RendererData> rendererStates;
        List<TooltipData> tooltipStates;

        Coroutine fadeRoutine;

        public void Run()
        {
            rendererStates = CollectInitialStates(renderers);
            tooltipStates = CollectInitialStates(tooltips);
            fadeRoutine = StartCoroutine(FadeIn(rendererStates, tooltipStates, center, duration, delay, tooltipActivationTime, fadeMaterial));
        }

        public void Cleanup()
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = null;
            foreach (var rendererState in rendererStates)
            {
                rendererState.Revert();
            }
            foreach (var tooltipState in tooltipStates)
            {
                tooltipState.Revert();
            }
        }

        [ContextMenu("Load Items")]
        void AutoLoad()
        {
            GetComponentsInChildren(tooltips);
            GetComponentsInChildren(renderers);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        List<TooltipData> CollectInitialStates(List<Tooltip> targets)
        {
            var result = new List<TooltipData>();
            foreach (var target in targets)
            {
                var s = TooltipData.CopyFrom(target);
                result.Add(s);
            }
            return result;
        }

        List<RendererData> CollectInitialStates(List<MeshRenderer> targets)
        {
            var result = new List<RendererData>();
            foreach (var target in targets)
            {
                var s = RendererData.CopyFrom(target);
                result.Add(s);
            }
            return result;
        }

        IEnumerator FadeIn(List<RendererData> rendererTargets, List<TooltipData> tooltipTargets, Transform center, float duration, float delay, float tooltipActivation, Material materialOverride)
        {
            var t = 0f;
            duration = Mathf.Clamp(duration, 0, 10f);
            foreach (var target in rendererTargets)
            {
                for (var i = 0; i < target.Materials.Length; i++)
                {
                    var propBlock = new MaterialPropertyBlock();
                    propBlock.SetColor(ColorPropKey, Color.white);
                    target.Target.SetPropertyBlock(propBlock, i);
                }
            }
            foreach (var target in tooltipTargets)
            {
                target.ApplyCustom(center.position, false);
            }
            yield return new WaitForSeconds(delay);
            while (t < duration)
            {
                t += Time.deltaTime;
                var st = Mathf.SmoothStep(0, 1f, Mathf.Clamp01(t / duration));
                foreach (var target in rendererTargets)
                {
                    for (var i = 0; i < target.Materials.Length; i++)
                    {
                        var propBlock = new MaterialPropertyBlock();
                        var col = target.Materials[i].GetColor(ColorPropKey);
                        col = Color.Lerp(Color.white, col, st);
                        propBlock.SetColor(ColorPropKey, col);
                        target.Target.SetPropertyBlock(propBlock, i);
                    }
                }
                foreach (var target in tooltipTargets)
                {
                    target.ApplyCustom(Vector3.Lerp(center.position, target.Position, st), st > tooltipActivation);
                }
                yield return null;
            }
            foreach (var target in rendererTargets)
            {
                target.Revert();
            }
            foreach (var target in tooltipTargets)
            {
                target.Revert();
            }
            fadeRoutine = null;
        }
    }
}
