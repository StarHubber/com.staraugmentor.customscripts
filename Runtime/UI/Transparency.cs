using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StarCooperation;
using System.Linq;
using System;
using UnityEngine.Events;

namespace StarCooperation.Helpers
{
    public class Transparency : MonoBehaviour
    {
        public Slider slider;
        public TextMeshProUGUI text;
        public Material trans;
        public Material opaque;
        public GameObject parts;
        public GameObject model;
        public HighlightHandler highlighterQueue;
        private bool hulltoggled = true;

        private List<MeshRenderer> meshR = new List<MeshRenderer>();
        private List<MeshRenderer> meshRC = new List<MeshRenderer>();
        private List<MeshRenderer> mask = new List<MeshRenderer>();
        private bool isOpaque = true;
        private List<ModelHighlighter> selectedHighlighter = null;

        private bool stopCamera = false;
        // Start is called before the first frame update
        void Start()
        {
            ChangeTransparency();
            meshR.AddRange(model.GetComponentsInChildren<MeshRenderer>());
            slider.onValueChanged.AddListener(delegate {
                ChangeTransparency();
            });

            foreach (ComponentView sg in parts.GetComponentsInChildren<ComponentView>(true))
            {
                //go.onValueChanged.AddListener(delegate { ResetMaterials(); });
                sg.ToggleShowItems.onValueChanged.AddListener(delegate { 
                    ChangeMask();
                    UpdateToggleHull();
                });
            }
            ChangeTransparency();
        }

        public void ToggleHull()
        {
            ChangeMask();
            hulltoggled ^= true;
            foreach(MeshRenderer obj in mask)
            {
                obj.gameObject.SetActive(hulltoggled);
            }
        }

        public void UpdateToggleHull()
        {
            if (hulltoggled == false) {
                foreach (MeshRenderer obj in mask)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (stopCamera)
                StopCamera();
        }

        private void StopCamera()
        {
        }

        public void OnPointerDown()
        {
            FindObjectOfType<MoveCamera>().allowMovement = false;

        }
        public void OnPointerUp()
        {
            FindObjectOfType<MoveCamera>().allowMovement = true;

        }

        private void ChangeMask()
        {
            //Change Text
            text.text = ((int)slider.value).ToString();
            //selectedHighlighter = ReturnSelectedHighlighter();
            if (highlighterQueue.ActiveHighlighter.Count > 0)
            {
                selectedHighlighter = highlighterQueue.ActiveHighlighter;
                //fetch mask from highlighter inverse
                GetMask(selectedHighlighter);

                //Swap materials below 100%
                SwapMats();
            }

            Resources.UnloadUnusedAssets();
        }

        private void ChangeTransparency()
        {
            if (isOpaque == true || slider.value == 100)
                ChangeMask();

            //change transparency
            if (((int)slider.value) < 100)
            {
                ChangeTrans();
            }
        }

        private ModelHighlighter ReturnSelectedHighlighter()
        {
            ModelHighlighter highlighter = null;
            Toggle toggle = null;
            foreach (ToggleGroup go in parts.GetComponentsInChildren<ToggleGroup>())
            {
                toggle = go.ActiveToggles().FirstOrDefault();
                if (toggle != null)
                    break;
            }

            if (toggle != null)
                highlighter = toggle.transform.GetComponent<ToggleListener>().highlighter;

            return highlighter;
        }

        private void GetMask(List<ModelHighlighter> highList)
        {
            meshRC = new List<MeshRenderer>();
            mask = new List<MeshRenderer>(meshR);
            foreach (var high in highList)
            {
                if (high.modelParts != null)
                {
                    foreach (var parts in high.modelParts)
                    {
                        meshRC.Add(parts.GetComponent<MeshRenderer>());
                        meshRC.AddRange(parts.GetComponentsInChildren<MeshRenderer>());
                    }
                }
                if (meshRC.Count != 0)
                {
                    foreach (MeshRenderer mesh in meshRC)
                    {
                        mask.Remove(mesh);
                    }
                }
            }
        }

        private void SwapMats()
        {
            if (slider.value < 100 /*&& isOpaque*/)
            {
                foreach (var mesh in mask)
                {
                    if (mesh != null)
                    {
                        Material[] mats = mesh.materials;
                        for (int i = 0; i < mats.Length; i++)
                            mats[i] = trans;

                        mesh.materials = mats;
                    }
                    isOpaque = false;
                }
            }
            if (slider.value == 100 /*&& !isOpaque*/)
            {
                foreach (var mesh in mask)
                {
                    if (mesh != null)
                    {
                        Material[] mats = mesh.materials;
                        for (int i = 0; i < mats.Length; i++)
                            mats[i] = opaque;

                        mesh.materials = mats;
                    }
                    isOpaque = true;
                }
                
            }
        }

        private void ChangeTrans()
        {
            //Change Text
            text.text = ((int)slider.value).ToString();

            trans.color = new Color(trans.color.r, trans.color.g, trans.color.b, slider.value / 100);
        }

        public void ResetMaterials()
        {
            if (selectedHighlighter != null)
            {
                if (slider.value == 100)
                {
                    foreach (var mesh in mask)
                    {
                        if (mesh != null) mesh.material = opaque;
                    }
                }
                else
                {
                    foreach (var mesh in mask)
                    {
                        if (mesh != null) mesh.material = trans;
                    }
                }
            }

        }
    }
}