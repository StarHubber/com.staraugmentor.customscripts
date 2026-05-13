using System;
using System.Collections.Generic;
using StarCooperation.Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace StarCooperation
{
    public class LB9DifferentialLockVisDetailController : MonoBehaviour
    {

        [Serializable] class HighlighterLockAssociation
        {
            [SerializeField] ScriptableEventBool scriptableEvent;
            [SerializeField] Renderer target;
            Material highlightMaterial;
            Material[] defaultMaterials;

            public void Initialize(Material highlightMat)
            {
                highlightMaterial = highlightMat;
                scriptableEvent.Raised += OnEventRaised;
                defaultMaterials = target.materials;
            }

            public void Cleanup()
            {
                scriptableEvent.Raised -= OnEventRaised;
                target.materials = defaultMaterials;
            }

            void OnEventRaised(bool value)
            {
                if (value)
                {
                    var mats = new Material[defaultMaterials.Length];
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats[i] = highlightMaterial;
                    }
                    target.materials = mats;
                }
                else
                {
                    target.materials = defaultMaterials;
                }
            }
        }

        [SerializeField] Material highlightMaterial;
        [SerializeField] List<HighlighterLockAssociation> lockHighlighters = new List<HighlighterLockAssociation>();
        [SerializeField] DetailAutoExploder exploder;

        void OnEnable()
        {
            foreach (var lockHighlighter in lockHighlighters)
            {
                lockHighlighter.Initialize(highlightMaterial);
            }
            if (exploder != null) exploder.Run();
        }

        void OnDisable()
        {
            if (exploder != null) exploder.Cleanup();
            foreach (var lockHighlighter in lockHighlighters)
            {
                lockHighlighter.Cleanup();
            }
        }
    }
}