using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    public enum Clicktype
    {
        Menu,
        Endstecker,
        Tab
    }
    public class SteckerHandler : MonoBehaviour
    {
        public static SteckerHandler Instance;
        public delegate void ActiveSteckerChange(Stecker activeStecker, bool active, Clicktype clicktype);
        public static event ActiveSteckerChange OnActiveSteckerChange;

        public static List<Stecker> SteckerList;
        public Stecker ActiveStecker { get; set; }

        private void Awake() => Instance = this;
        private void OnEnable()
        {
            SteckerList = new List<Stecker>();
        }

        public void RegisterMenuClick(Stecker stecker, bool isOn)
        {
            if (!isOn) DisableStecker(stecker);
            else EnableStecker(stecker, Clicktype.Menu);

        }
        public void RegisterTabClick(Stecker stecker)
        {
            SetToggleSilently(stecker);
            EnableStecker(stecker, Clicktype.Tab);
        }

        public void RegisterEndsteckerClick(ToggleEar toggleEar)
        {
            SetToggleSilently(toggleEar.Stecker);
            EnableStecker(toggleEar?.Stecker, Clicktype.Endstecker);
        }

        public void AddStecker(Stecker stecker)
        {
            if (SteckerList.Contains(stecker)) return;

            AddSteckerToList(stecker);
        }

        private void AddSteckerToList(Stecker stecker)
        {
            SteckerList.Add(stecker);
        }

        //Sets Toggle Silently to prevent registering the same toggle for menu click in a loop
        private static void SetToggleSilently(Stecker temp)
        {
            temp.Toggle.Toggle.Set(true, false);
            temp.Toggle.GetComponent<ToggleInteractionDesign>().ToggleListener();
        }

        private void EnableStecker(Stecker stecker, Clicktype type)
        {
            SetActiveStecker(stecker, Clicktype.Menu);
            OnActiveSteckerChange(stecker, true, type);
            Invoke("SetScrollViewTarget", .1f);
        }

        private void DisableStecker(Stecker stecker)
        {
            if (ActiveStecker != null && ActiveStecker != stecker)
                return;
            else
            {
                TabController.Instance.ClearAllTabs();
                SteckerViewer.Instance.ToggleSteckerViewState(false);
                SetToActiveStecker(stecker, false);
                OnActiveSteckerChange(stecker, false, Clicktype.Menu);

            }
        }

        private void SetScrollViewTarget() => GameObject.Find("ScrollView").GetComponent<ScrollRect>().ScrollToItem(SteckerHandler.Instance.ActiveStecker.Toggle.GetComponent<RectTransform>());

        public bool SetActiveStecker(Stecker newStecker, Clicktype clicktype)
        {
            if (newStecker == ActiveStecker) return false;
            CheckAndSetOldStecker();
            SetToActiveStecker(newStecker, true);
            return true;
        }

        private void CheckAndSetOldStecker()
        {
            if (ActiveStecker != null) SetToActiveStecker(ActiveStecker, false);

        }

        private void SetToActiveStecker(Stecker newStecker, bool toActivate)
        {
            if (toActivate)
            {
                ActiveStecker = newStecker;
                ActiveStecker.Highlighter.Highlight(true);
                LineDrawer.Instance.CreateParticleEffectOnActiveTooltip(newStecker);

            }
            else
            {
                ActiveStecker.Highlighter.Highlight(false);
                LineDrawer.Instance.StopActivePS();
                ActiveStecker.Toggle.Toggle.Set(false, false);
                ActiveStecker.Toggle.GetComponent<ToggleInteractionDesign>().ToggleListener();
                ActiveStecker = null;
                //   ToggleActiveStecker(ActiveStecker, newStecker);

            }
        }


        public Stecker RemoveActiveTooltip()
        {
            if (ActiveStecker == null) return null;
            ActiveStecker.Highlighter.Highlight(false);
            OnActiveSteckerChange(ActiveStecker, false, Clicktype.Menu);
            ActiveStecker = null;
            return ActiveStecker;
        }

    }
}