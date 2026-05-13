using System;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class PinPongRotationAnimation: MonoBehaviour
    {
        [Serializable]
        class AnimatedElement
        {
            [SerializeField] Mesh mesh;
            [SerializeField] Material material;
            [SerializeField] bool overrideColor = true;
            [SerializeField] Color colorOverride = Color.green;

            [SerializeField] Vector3 minAngle = new Vector3(0, 0, 0);
            [SerializeField] Vector3 maxAngle = new Vector3(20f, 0, 0);
            [SerializeField] Vector3 reverseRotationOffset = new Vector3(-180f, 0, 0);

            [SerializeField, Range(0.1f, 10f)] float durationPerDirection = 1f;

            [SerializeField] bool smoothed = true;

            MaterialPropertyBlock propBlock;

            public void Draw(float time, Vector3 pos, Quaternion rot, Vector3 scale, int layer)
            {
                if (mesh == null || material == null) return;
                if (propBlock == null) propBlock = new MaterialPropertyBlock();
                else propBlock.Clear();
                var animTime = GetAnimTime(time);
                var animATime = Mathf.Clamp01(animTime * 2f);
                var animBTime = Mathf.Clamp01(animTime * 2f - 1f);
                var fadeATime = Smooth(Mathf.Clamp01(Mathf.PingPong(animATime * 2f, 1f)));
                var fadeBTime = Smooth(Mathf.Clamp01(Mathf.PingPong(animBTime * 2f, 1f)));
                var col = overrideColor ? colorOverride : material.color;
                col.a = fadeATime;
                propBlock.SetColor(ColorPropKey, col);
                var aRot = Vector3.Lerp(minAngle, maxAngle, animATime);
                var bRot = Vector3.Lerp(maxAngle, minAngle, animBTime);
                Graphics.DrawMesh(mesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(aRot), scale), material, layer, null, 0, propBlock);
                col.a = fadeBTime;
                propBlock.SetColor(ColorPropKey, col);
                Graphics.DrawMesh(mesh, Matrix4x4.TRS(pos, rot * Quaternion.Euler(bRot) * Quaternion.Euler(reverseRotationOffset), scale), material, layer, null, 0, propBlock);
            }

            float GetAnimTime(float time)
            {
                return Mathf.Clamp(time % durationPerDirection / durationPerDirection, 0, 1f);
            }

            float Smooth(float t)
            {
                if (!smoothed) return t;
                return Mathf.SmoothStep(0, 1, t);
            }
        }

        [SerializeField] List<AnimatedElement> elements = new List<AnimatedElement>();

        float time;
        static readonly int ColorPropKey = Shader.PropertyToID("_Color");

        void OnEnable()
        {
            time = 0;
        }

        void Update()
        {
            time += Time.deltaTime;
        }

        void OnRenderObject()
        {
            var pos = transform.position;
            var rot = transform.rotation;
            var scale = transform.localScale;
            foreach (var element in elements)
            {
                element.Draw(time, pos, rot, scale, gameObject.layer);
            }
        }

        
    }
}
