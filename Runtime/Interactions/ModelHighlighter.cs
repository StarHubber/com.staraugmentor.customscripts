using System;
using StarCooperation.STAR;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    public class ModelHighlighter : MonoBehaviour
    {
        public int Priority = 0;
        // Lowlighting should look the same everywhere, so keep as public static for easy access.
        public static float lowlightAlpha = 0.1f;

        public static List<ModelHighlighter> activeHighOrLowlighers = new List<ModelHighlighter>();

        public delegate void ModelHighlighted(ModelHighlighter highlighter);
        public static event ModelHighlighted OnModelHighlightChanged;
        public static event ModelHighlighted OnModelSentTransparent;
        public BooleanCallback OnModelHighlighted = new BooleanCallback();
        public BooleanCallback OnModelUnhighlighted = new BooleanCallback();

        [Header("3D Model")]
        public List<GameObject> modelParts;

        [Header("Highlight Material")]
        [Tooltip("Material to highlight specific model parts.")]
        public Material highlightMat, transparentMat;

        [Header("Highlight Particles")]
        public GameObject highlightParticlePrefab;
        public bool useExistingParticles = false;
        public List<GameObject> highlightParticles;

        [Header("Other Settings")]
        public bool changeRotationCenter = true;

        [HideInInspector] public Bounds totalBounds;

        //private GameObject[] highlightParticles;

        /// <summary>
        /// Helper class to easily access front and back MeshRenderer off assigned model parts.
        /// </summary>
        private class MeshHighlighter
        {
            /// <summary>
            /// For debug purposes.
            /// </summary>
            private GameObject containingGameObject;

            private List<MeshRenderer> meshRendsToHighlight;
            private Dictionary<MeshRenderer, Material[]> defaultMaterials;
            private Dictionary<MeshRenderer, float[]> defaultTransparencies;

            public MeshHighlighter(List<GameObject> modelParts, GameObject gameObject)
            {
                containingGameObject = gameObject;

                meshRendsToHighlight = new List<MeshRenderer>();
                defaultMaterials = new Dictionary<MeshRenderer, Material[]>();
                defaultTransparencies = new Dictionary<MeshRenderer, float[]>();

                GetMaterialSettings(modelParts);
            }

            private void GetMaterialSettings(List<GameObject> modelParts)
            {
                // Get MeshRenderer to hightlight
                foreach (var modelPart in modelParts)
                {
                    var childrenMeshRends = modelPart.GetComponentsInChildren<MeshRenderer>(true);
                    if (childrenMeshRends != null)
                    {
                        foreach (var meshRend in childrenMeshRends)
                        {
                            // Workaround: Lowlighter modelPart List s created -not- via comparing transform or GameObject, but MeshRenderers.
                            // So, parent and child MeshRenderers can occur here, therefore double iterating through children that are already checked!
                            if (!meshRendsToHighlight.Contains(meshRend))
                            {
                                meshRendsToHighlight.Add(meshRend);
                            }
                        }
                        //meshRendsToHighlight.AddRange(childrenMeshRends);
                    }
                }

                // Assign material settings
                foreach (var meshRend in meshRendsToHighlight)
                {
                    defaultMaterials.Add(meshRend, meshRend.sharedMaterials);
                    float[] transparencies = new float[meshRend.sharedMaterials.Length];
                    for (int i = 0; i < meshRend.sharedMaterials.Length; i++)
                    {
                        transparencies[i] = meshRend.sharedMaterials[i].color.a;
                    }
                    defaultTransparencies.Add(meshRend, transparencies);
                }
            }

            /// <summary>
            /// Highlight models by setting shared highlight material.
            /// </summary>
            /// <param name="doHighlight"></param>
            /// <param name="highlightMat"></param>
            public void Highlight(bool doHighlight, Material highlightMat = null)
            {
                //if (doHighlight)
                //{
                //    defaultMaterials.Clear();
                //    defaultTransparencies.Clear();
                //}
                //foreach (var meshRend in meshRendsToHighlight)
                //{
                //    defaultMaterials.TryAdd(meshRend, meshRend.sharedMaterials);
                //    float[] transparencies = new float[meshRend.sharedMaterials.Length];
                //    for (int i = 0; i < meshRend.sharedMaterials.Length; i++)
                //    {
                //        transparencies[i] = meshRend.sharedMaterials[i].color.a;
                //    }
                //    defaultTransparencies.TryAdd(meshRend, transparencies);
                //}
                for (int i = 0; i < meshRendsToHighlight.Count; i++)
                {
                    if (doHighlight)
                    {
                        var newMatArray = new Material[meshRendsToHighlight[i].sharedMaterials.Length];
                        for (int j = 0; j < meshRendsToHighlight[i].sharedMaterials.Length; j++)
                        {
                            newMatArray[j] = highlightMat;
                        }
                        meshRendsToHighlight[i].sharedMaterials = newMatArray;
                    }
                    else
                    {
                        meshRendsToHighlight[i].sharedMaterials = defaultMaterials[meshRendsToHighlight[i]];
                    }
                }
            }

            /// <summary>
            /// Lowlight models by setting predefined alpha value.
            /// </summary>
            /// <param name="doLowlight"></param>
            public void Lowlight(bool doLowlight)
            {
                if (!doLowlight)
                {
                    // On un-Lowlight, simply un-Highlight to assign default SharedMaterial to all MeshRenderers!
                    Highlight(false);
                }
                else
                {
                    for (int i = 0; i < meshRendsToHighlight.Count; i++)
                    {
                        // Disable collider to disable clicking on lowlighted mesh
                        var collider = meshRendsToHighlight[i].gameObject.GetComponent<Collider>();
                        if (collider != null)
                        {
                            collider.enabled = !doLowlight;
                        }

                        // Lowlight all materials
                        for (int j = 0; j < meshRendsToHighlight[i].materials.Length; j++)
                        {
                            Material mat = meshRendsToHighlight[i].materials[j];
                            StandardShaderUtils.BlendMode newBlendMode = doLowlight ? StandardShaderUtils.BlendMode.Fade : StandardShaderUtils.BlendMode.Opaque;
                            StandardShaderUtils.ChangeRenderMode(mat, newBlendMode);    // todo: maybe fade routine. if, then put existing in utils class and re-use here
                            Color color = mat.color;
                            color.a = doLowlight ? lowlightAlpha : defaultTransparencies[meshRendsToHighlight[i]][j];
                            mat.color = color;
                         
                        }
                    }
                }
            }

        }

        private MeshHighlighter modelHighlighter;
        private MeshHighlighter modelLowlighter;

        private void OnDestroy()
        {
            modelHighlighter = null;
            modelLowlighter = null;
        }
        public bool isHighlighted
        {
            get;
            private set;
        }

        public bool isLowlightActive
        {
            get;
            private set;
        }

        // Start is called before the first frame update
        private void Start()
        {
            isHighlighted = false;
            isLowlightActive = false;

            modelHighlighter = new MeshHighlighter(modelParts, gameObject);

            // Get rest of model for lowlight access
            List<GameObject> modelPartsLowlight = new List<GameObject>();
            //foreach (Transform modelPart in SceneManagerBase.baseInstance.modelHolder.transform)

            var assignedMeshRends = new List<MeshRenderer>();
            foreach (var modelpart in modelParts)
            {
                assignedMeshRends.AddRange(modelpart.GetComponentsInChildren<MeshRenderer>(true));
            }

            foreach (var meshRendFromTotalModel in ModelControl.Instance?.modelMeshRends)
            {
                //if (!modelParts.Contains(meshRend.gameObject))
                if (!assignedMeshRends.Contains(meshRendFromTotalModel))
                {
                    if (meshRendFromTotalModel != null)
                        modelPartsLowlight.Add(meshRendFromTotalModel.gameObject);
                }
            }
            modelLowlighter = new MeshHighlighter(modelPartsLowlight, gameObject);

            // Calculate bounds of modelParts (not front and back - will be removed in the future)
            totalBounds = new Bounds();
            foreach (var part in modelParts)
            {
                var meshRends = part.GetComponentsInChildren<MeshRenderer>();
                if (meshRends != null)
                {
                    foreach (var rend in meshRends)
                    {
                        if (totalBounds.center == default)
                        {
                            totalBounds.center = rend.bounds.center;
                        }
                        totalBounds.Encapsulate(rend.bounds);
                    }
                }
            }

            // Highlight Particles
            // Auto-creates particle prefab instances and places them at bounds center of model parts,
            // or use existing Particles.
            // Todo: Could be combined with totalBounds calculation above, but would reduce readibility.
            if (highlightParticlePrefab != null || useExistingParticles)
            {
                if (highlightParticlePrefab != null && !useExistingParticles)
                {
                    highlightParticles = new List<GameObject>(new GameObject[modelParts.Count]);
                }

                for (int i = 0; i < highlightParticles.Count; i++)
                {
                    if (!useExistingParticles)
                    {
                        highlightParticles[i] = Instantiate(highlightParticlePrefab);
                        highlightParticles[i].transform.SetParent(modelParts[i].transform);

                        var bounds = new Bounds();
                        foreach (var childMeshRend in modelParts[i].GetComponentsInChildren<MeshRenderer>())
                        {
                            if (bounds.center == default)
                            {
                                bounds.center = childMeshRend.bounds.center;
                            }
                            bounds.Encapsulate(childMeshRend.bounds.center);
                        }
                        highlightParticles[i].transform.position = bounds.center;
                    }

                    highlightParticles[i].SetActive(false);
                }
            }
        }
        public void Highlight(string guid)
        {
            bool doHighlight = false;
            if (!isHighlighted && GetComponent<GUIDComponent>().GetGuid() == guid)
            {
                doHighlight = true;
            }
            Highlight(doHighlight);

        }
        /// <summary>
        /// Highlight all model parts that are assigned to this ModelHighlighter script.
        /// </summary>
        /// <param name="doHighlight"></param>
        public void Highlight(bool doHighlight)
        {
            // Assign to list
            if (doHighlight)
            {                
                activeHighOrLowlighers.Add(this);
            }
            else
            {
                activeHighOrLowlighers.Remove(this);
            }

            // Invoke Events
            foreach (var part in modelParts)
            {
                if (part.transform.childCount > 0)
                {
                    foreach (Transform child in part.GetComponentsInChildren<Transform>(true)) { child.gameObject.SetActive(true); }
                }
                part.gameObject.SetActive(true);
            }
            OnModelHighlighted?.Invoke(doHighlight);
            OnModelUnhighlighted?.Invoke(!doHighlight);

            // Actual highlighting
            isHighlighted = doHighlight;
            modelHighlighter?.Highlight(doHighlight, highlightMat);

            if (highlightParticles != null)
            {
                for (int i = 0; i < highlightParticles.Count; i++)
                {
                    highlightParticles[i].SetActive(doHighlight);
                }
            }

            // Last event
            OnModelHighlightChanged?.Invoke(this);  // Raise event after everyting is set
        }
        //public void HighlightLB11(bool doHighlight)
        //{
        //    //OnModelHighlighted?.Invoke(doHighlight);
        //    //OnModelUnhighlighted?.Invoke(!doHighlight);

        //    isHighlighted = doHighlight;
        //    modelHighlighter?.Highlight(doHighlight, highlightMat);

        //  //  OnModelHighlightChanged?.Invoke(this);  // Raise event after everyting is set
        //}

        /// <summary>
        /// Lowlighting makes all meshes transparent that are NOT assigned to this ModelHighlighter (via Model assignment in SceneManager).
        /// </summary>
        /// <param name="doLowlight"></param>
        public void Lowlight(bool doLowlight)
        {
            // Assign to list
            if (doLowlight)
            {
                activeHighOrLowlighers.Add(this);
            }
            else
            {
                activeHighOrLowlighers.Remove(this);
            }

            isLowlightActive = doLowlight;
            modelLowlighter?.Lowlight(doLowlight);

            OnModelHighlightChanged?.Invoke(this);  // Raise event after everyting is set
        }

        /// <summary>
        /// Reset highlighter to default state, that is, Lowlight off, Highlight off.
        /// </summary>
        public void ResetHighlighting()
        {
            Lowlight(false);
            Highlight(false);
        }

        public void ResetHighlightingAll()
        {   foreach(var highlighter in activeHighOrLowlighers.ToList())
            {
                highlighter.ResetHighlighting();
            }
        }

        public void SetTransparent(bool disable)
        {
            if (modelHighlighter == null) return;

            for (int i = 0; i < modelParts.Count; i++)
            {
                if (disable)
                {

                    modelHighlighter.Highlight(true, transparentMat);
                    //  modelParts[i].gameObject.SetActive(false);
                }
                else
                    if (isHighlighted)
                    modelHighlighter.Highlight(true, highlightMat);
                else
                    modelHighlighter.Highlight(false, highlightMat);

                //  modelParts[i].gameObject.SetActive(true);
            }
        }
        public void HighlightWithoutNotification(bool doHighlight)
        {
            // Assign to list
            if (doHighlight)
            {
                activeHighOrLowlighers.Add(this);
            }
            else
            {
                activeHighOrLowlighers.Remove(this);
            }

            // Invoke Events
            //   OnModelHighlighted?.Invoke(doHighlight);
            //    OnModelUnhighlighted?.Invoke(!doHighlight);

            // Actual highlighting
            isHighlighted = doHighlight;
            modelHighlighter?.Highlight(doHighlight, highlightMat);

            if (highlightParticles != null)
            {
                for (int i = 0; i < highlightParticles.Count; i++)
                {
                    highlightParticles[i].SetActive(doHighlight);
                }
            }

            // Last event
            //  OnModelHighlightChanged?.Invoke(this);  // Raise event after everyting is set
        }
    }
}