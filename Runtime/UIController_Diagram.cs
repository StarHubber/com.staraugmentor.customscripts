using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using HoloRepair.Core;
using System.Linq;
using System;
using System.Threading.Tasks;
using STAR.Utils;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace StarCooperation
{
    public class UIController_Diagram : MonoBehaviour
    {
        public Image DiagramImage;


        public UnityEvent triggerAssetUpdate;

        private void OnEnable()
        {
            triggerAssetUpdate.Invoke();

        }


        private float normalizedContainerPosY;
        private float maximizedContainerPosY;
        private bool coroutineRunning = false;

        private LocalizedString currentString;
        private LocalizedAsset<Sprite> currentSprite;

        private void Awake()
        {
        }
        private void Start()
        {
            //currentString = null;
            //currentSprite = null;
            //UpdateAsset(null);
        }

        public void OnLanguageChange()
        {
            var localizedAsset = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(currentSprite.TableReference, currentSprite.TableEntryReference);
            SetDiagramPicture(localizedAsset);
        }
        public void UpdateInfobox(LocalizedString textInfoString, LocalizedAsset<Sprite> asset)
        {
            UpdateAsset(asset);
        }

        private void UpdateAsset(LocalizedAsset<Sprite> asset)
        {
            if (currentSprite is not null)
                currentSprite.AssetChanged -= SetDiagramPicture;

            if (asset == null)
            {
                SetDiagramPicture(null);
                return;
            }

            if (asset.IsEmpty || asset == null)
            {
                SetDiagramPicture(null);
                return;
            }

            var localizedAsset = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(asset.TableReference, asset.TableEntryReference);


            SetDiagramPicture(localizedAsset);
            currentSprite = asset;
            currentSprite.AssetChanged += SetDiagramPicture;
        }

        private void SetDiagramPicture(Sprite tex)
        {
            DiagramImage.sprite = tex;
        }
    }
}