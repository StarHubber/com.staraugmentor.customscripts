using NaughtyAttributes;
using StarCooperation.Export;
using StarCooperation.ExportCCP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    [DefaultExecutionOrder(0)]
    /// <summary>
    /// Controls hull assigned in prefab.
    /// </summary>
    public class ModelControl : MonoBehaviour
    {
        [Serializable]
        public struct MaterialSet
        {
            public List<GameObject> Geometrie;
            public Material Material;
        }

        //[SerializeField] private Transparency transSlider;
        public static ModelControl Instance;

        public GameObject modelHolder;
        public GameObject fadeAndLowlightException;
        public GameObject[] fadeAndLowlightExceptions;
        public GameObject modelHull;
        public GameObject modelBadge;
        public GameObject modelShadow;
        [HideInInspector] public List<MeshRenderer> modelMeshRends = new List<MeshRenderer>();

        [Header("Model Color Define")]
        public List<MaterialSet> MaterialSets = new List<MaterialSet>();
        public Material changeableMaterial;

        private bool materialsColored = true;
        private Coroutine fadeModelCoroutine;
        private float initialHullTransparency;

        private Bounds modelBounds;

        private MeshRenderer sharedMeshRend;
        private Color defaultModelPartColor;
        private float initialTransValue;

        private void Awake()
        {
            Instance = this;
            MessageHandler messageHandler = FindObjectOfType<MessageHandler>();
            Debug.Log(messageHandler);
            messageHandler.SetBadgeAndHull(modelBadge, modelHull);
            sharedMeshRend = GetComponent<MeshRenderer>();
            defaultModelPartColor = sharedMeshRend.sharedMaterial.color;

            // Get model bounds for camera calculation (and maybe other stuff)
            modelBounds = new Bounds();
            modelBounds.center = Vector3.zero;  // Always within car model

            // Get mesh renderer in awake, might be accessed from other scripts during Start
            //modelMeshRends = new List<MeshRenderer>();

            var exceptionMeshRends = new List<MeshRenderer>();
            if (fadeAndLowlightException != null)
            {
                exceptionMeshRends.AddRange(fadeAndLowlightException.GetComponentsInChildren<MeshRenderer>());
            }
            if (fadeAndLowlightExceptions != null && fadeAndLowlightExceptions.Length > 0)
            {
                foreach (var ex in fadeAndLowlightExceptions)
                {
                    exceptionMeshRends.AddRange(ex.GetComponentsInChildren<MeshRenderer>());
                }
            }

            foreach (var meshRend in modelHolder.GetComponentsInChildren<MeshRenderer>())
            {
                if (exceptionMeshRends.Count > 0)
                {
                    if (exceptionMeshRends.Contains(meshRend))
                    {
                        continue;
                    }
                }
                modelMeshRends.Add(meshRend);
                modelBounds.Encapsulate(meshRend.bounds);
            }
        }

        private void OnDestroy()
        {
            sharedMeshRend.sharedMaterial.color = defaultModelPartColor;
        }

        [Button("Assign Materials")]
        public void SwitchMaterial()
        {
            if (materialsColored)
            {
                foreach (MaterialSet set in MaterialSets)
                {
                    foreach (GameObject geo in set.Geometrie)
                    {
                        AssignMaterialToAllChildren(geo, changeableMaterial);
                    }
                }
                materialsColored = false;
            }
            else
            {
                foreach (MaterialSet set in MaterialSets)
                {
                    foreach (GameObject geo in set.Geometrie)
                    {
                        AssignMaterialToAllChildren(geo, set.Material);
                    }
                }
                materialsColored = true;
            }
        }

        /// <summary>
        /// Assign Materials.
        /// </summary>
        private void AssignMaterialToAllChildren(GameObject geo, Material mat)
        {
            foreach (var meshRend in geo.GetComponentsInChildren<MeshRenderer>(true))
            {
                Material[] newMatArray = new Material[meshRend.materials.Length];
                for (int i = 0; i < meshRend.materials.Length; i++)
                {
                    newMatArray[i] = mat;
                }
                List<Material> matList = new List<Material>(meshRend.materials);

                meshRend.materials = newMatArray; // shared materials due to edit mode
            }
        }

        /// <summary>
        /// Change ModelPart color to adjust "opacity" on Hololens
        /// </summary>
        /// <param name="opacity"></param>
        public void ChangeModelPartMaterialOpacity(float opacity)
        {
            foreach (var mat in sharedMeshRend.sharedMaterials)
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, opacity);
            }
            //foreach (var meshRend in modelMeshRends)
            //{
            //	if (meshRend.material.Equals(modelPartMaterial))
            //	{
            //		meshRend.sharedMaterial.color = Color.Lerp(new Color(0.1f, 0.1f, 0.1f), defaultModelPartColor, opacity);
            //	}
            //}
        }

        /// <summary>
        /// Fade whole 3D model in or out via transparency.
        /// </summary>
        /// <param name="fadeOut"></param>
        public void FadeModelOut(bool fadeOut)
        {

            if (modelHull == null)
            {
                Debug.LogError("ModelHull not assigned. Please do now.");
                return;
            }
            //initialTransValue = transSlider.slider.normalizedValue;

            if (fadeModelCoroutine != null)
            {
                StopCoroutine(fadeModelCoroutine);
            }
            fadeModelCoroutine = StartCoroutine(DoFadeModel(fadeOut));
        }

        private IEnumerator DoFadeModel(bool fadeOut)
        {

            float alphaStartModel = fadeOut ? initialTransValue : 0;
            MeshRenderer shadowRend = null;
            MeshRenderer[] meshRendsHull = modelHull.GetComponentsInChildren<MeshRenderer>();
            try
            {
                if (fadeOut)
                {
                    initialHullTransparency = meshRendsHull[0].sharedMaterial.color.a;
                }
                float alphaStartHull = fadeOut ? initialHullTransparency : 0;
                if (modelShadow != null)
                    modelShadow?.TryGetComponent<MeshRenderer>(out shadowRend);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }



            //SpriteRenderer shadowRend = modelShadow.GetComponent<SpriteRenderer>();

            float t = 0;
            float alphaModel;
            float alphaHull;
            float alphaShadow;

            if (!fadeOut)
            {
                modelHolder.SetActive(true);
                modelHull.SetActive(true);
                modelShadow.SetActive(true);
            }
            else
            {
                //foreach (var meshRend in modelMeshRends)
                //{
                //    foreach (var mat in meshRend.materials)
                //    {
                //        StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Fade);
                //    }
                //}
            }

            while (t < 1)
            {
                if (DeviceSwitcher.Instance.device == AppType.TA)
                {
                    t += Time.deltaTime / (fadeOut ? MoveCamera.instance.settings.timeFocusDetail : MoveCamera.instance.settings.timeCameraReset);
                }
                else if (DeviceSwitcher.Instance.device == AppType.HL)
                {
                    t += Time.deltaTime;
                }

                if (t > 1)
                {
                    t = 1;
                }

                // Set alpha model
                alphaModel = Mathf.Lerp(alphaStartModel, initialTransValue, t);
                foreach (var meshRend in modelMeshRends)
                {
                    foreach (var mat in meshRend.sharedMaterials)
                    {
                        //if (mat.color == highlightMat.color)
                        //{
                        //	// Don't fade highlighted objects
                        //	continue;
                        //}
                        Color color = mat.color;
                        color.a = alphaModel;
                        mat.color = color;
                    }
                }

                // Set alpha hull
                alphaHull = Mathf.Lerp(fadeOut ? initialHullTransparency : 0, fadeOut ? 0 : initialHullTransparency, t);
                foreach (var meshRend in meshRendsHull)
                {
                    foreach (var mat in meshRend.sharedMaterials)
                    {
                        Color color = mat.color;
                        color.a = alphaHull;
                        mat.color = color;
                    }
                }
                if (shadowRend != null)
                {
                    // Set alpha shadow
                    alphaShadow = Mathf.Lerp(alphaStartModel, 1 - alphaStartModel, t);
                    Color colorShadow = shadowRend.material.color;
                    colorShadow.a = alphaShadow;
                    shadowRend.material.color = colorShadow;
                }
                yield return null;
            }

            if (fadeOut)
            {
                modelHolder?.SetActive(false);
                modelHull?.SetActive(false);
                if (modelShadow != null)
                    modelShadow?.SetActive(false);
            }
            else
            {
                //foreach (var meshRend in modelMeshRends)
                //{
                //    foreach (var mat in meshRend.materials)
                //    {
                //        StandardShaderUtils.ChangeRenderMode(mat, StandardShaderUtils.BlendMode.Opaque);
                //    }
                //}
            }
        }

        //public void ToggleHull()
        //{
        //	modelHull.SetActive(!modelHull.gameObject.activeSelf);
        //	modelShadow.SetActive(!modelShadow.activeSelf);
        //}
    }
}
