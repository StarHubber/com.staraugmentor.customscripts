using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StarCooperation
{
    public class TabController : MonoBehaviour
    {
        public static TabController Instance;
        [SerializeField]
        private GameObject tabPrefab;
        [SerializeField]
        private Color activeTabColor, passiveTabColor;
        public Color ActiveColor { get { return activeTabColor; } }
        public Color PassiveColor
        {
            get { return passiveTabColor; }
        }
        [SerializeField]
        public List<Tab> tablist;
        private int activeIndex;
        public int ActiveIndex
        {
            get { return activeIndex; }
            set
            {
                activeIndex = value;
            }
        }

        private void Start()
        {

        }
        private void OnEnable()
        {
            SteckerHandler.OnActiveSteckerChange += RespondToTooltipChange;
        }
        private void OnDisable()
        {
            SteckerHandler.OnActiveSteckerChange -= RespondToTooltipChange;
        }
        private void Awake()
        {
            Instance = this;
        }

        private void RespondToTooltipChange(Stecker tooltipToSet, bool active, Clicktype clicktype)
        {
            if (active)
            {
                if (clicktype == Clicktype.Menu)
                {
                    ClearAllTabs();
                }
                SetTooltipToTab();
                for (int i = 0; i < tablist.Count; i++)
                {
                    if (tablist[i].correspStecker == tooltipToSet)
                    {
                        continue;
                    }
                    tablist[i].SetAsActiveTab(false);
                }

            }
            else
            {
                if (clicktype == Clicktype.Menu)
                {
                    ClearAllTabs();
                }
                SetPassiveDesign(tooltipToSet);
            }
        }








        public void ClearAllTabs()
        {
            foreach (var item in tablist)
            {
                item.ClearTab();
            }
            tablist.Clear();


        }
        public void SetTooltipToTab()
        {
            if (IsTooltipAlreadyInList(SteckerHandler.Instance.ActiveStecker)) return;
            DeleteFollowingTabs();

            //if the tab before is not empty, it means this is the connection it needs to make.

            Tab Tab = CreateTab();

            tablist.Add(Tab);
            activeIndex = tablist.Count - 1;

        }

        private void DeleteFollowingTabs()
        {
            if (ActiveIndex <= tablist.Count - 1)
            {
                int lastIndex = tablist.Count - 1;
                for (int i = lastIndex; i > ActiveIndex; i--)
                {
                    tablist[i].ClearTab();
                    tablist.Remove(tablist[i]);

                }
            }

        }


        private Tab CreateTab()
        {
            GameObject lineR = null;
            Tab TabToAdd = Instantiate(tabPrefab, this.transform).GetComponent<Tab>();
            TabToAdd.name = "Tab_" + tablist.Count;
            if (tablist.Count > 0)
            {
                //Create LineR
                lineR = LineDrawer.Instance.DrawLineAndReturnGameObject(SteckerHandler.Instance.ActiveStecker, tablist[tablist.Count - 1].correspStecker);

            }
            TabToAdd.FillTabWithTooltip(SteckerHandler.Instance.ActiveStecker, lineR, true);
            if (tablist.Count <= 0) TabToAdd.SetAsFirstTab(true);
            if (tablist.Count > 0) TabToAdd.SetAsFirstTab(false);

            return TabToAdd;

        }


        private bool IsTooltipAlreadyInList(Stecker stecker)
        {
            foreach (var item in tablist)
            {
                if (item.correspStecker == stecker)
                {
                    item.SetAsActiveTab(true);
                    return true;
                }
            }
            return false;
        }

        public void SetPassiveDesign(Stecker correspStecker)
        {
            for (int i = 0; i < tablist.Count; i++)
            {
                if (tablist[i].correspStecker == correspStecker)
                {
                    tablist[i].SetAsActiveTab(false);
                }
            }

        }


    }

}