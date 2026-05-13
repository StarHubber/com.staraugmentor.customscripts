using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StarCooperation
{
    public class Tab : MonoBehaviour
    {
        [SerializeField]
        private Image activeImage, startImage, followingImage;
        public GameObject lineRenderer;
        [SerializeField]
        public Stecker correspStecker;
        public bool IsActiveTab;
        [SerializeField]
        private TMPro.TextMeshProUGUI textContainer;
        private bool isOccupied = false;
        public static bool clickDelayer = false;

        public bool IsOccupied
        {
            get { return isOccupied; }
            set { isOccupied = value; }
        }

        public void DoMagic(Tab tab)
        {
            Stecker temp;
            for (int i = 0; i < TabController.Instance.tablist.Count; i++)
            {
                if (TabController.Instance.tablist[i] == tab)
                {
                    temp = tab.correspStecker;

                    SteckerHandler.Instance.RegisterTabClick(temp);
                    SetAsActiveTab(true);
                }

            }
        }

        public void SetAsFirstTab(bool value)
        {
            if (value)
            {
                activeImage = startImage;
                textContainer.alignment = TMPro.TextAlignmentOptions.Left;

            }
            else
            {

                activeImage = followingImage;
                startImage.enabled = false;
                activeImage.color = TabController.Instance.ActiveColor;
            }
        }
        public void FillTabWithTooltip(Stecker tooltipToSet, GameObject lineR, bool isActiveTab)
        {
            SetAsActiveTab(isActiveTab);
            lineRenderer = lineR;
            correspStecker = tooltipToSet;
            SetTabText(tooltipToSet.SteckerInfo.id);
            IsOccupied = true;
        }
        public void ClearTab()
        {
            if (this.gameObject)
                GameObject.Destroy(lineRenderer, 0);
            if (correspStecker != null)
                //correspStecker.Highlighter.Highlight(false);
                correspStecker = null;
            textContainer.SetText(string.Empty);
            IsOccupied = false;
            SetAsActiveTab(false);
            GameObject.Destroy(this.gameObject);
        }
        private void SetTabText(string iD)
        {
            textContainer.SetText(iD);
        }
        public void SetAsActiveTab(bool value)
        {
            if (value)
            {
                textContainer.fontStyle = TMPro.FontStyles.Bold;

                IsActiveTab = true;
                TabController.Instance.ActiveIndex = TabController.Instance.tablist.IndexOf(this);
                activeImage.color = TabController.Instance.ActiveColor;
                if (lineRenderer)
                    LineDrawer.Instance.SetActiveMat(lineRenderer.GetComponent<LineRenderer>(), value);
            }
            else
            {
                textContainer.fontStyle = TMPro.FontStyles.Normal;

                IsActiveTab = false;
                activeImage.color = TabController.Instance.PassiveColor;
                if (lineRenderer)
                    LineDrawer.Instance.SetActiveMat(lineRenderer.GetComponent<LineRenderer>(), value);
            }
        }
    }
}