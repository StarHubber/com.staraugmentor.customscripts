using StarCooperation.LegacyLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StarCooperation
{
    public enum StepDetail
    {
        Default,
        Lupe,
        Explosion,
        Information,
        Animation
    }
    public abstract class UIElement : MonoBehaviour
    {
        public string Guid;
        public int listPosition;
        public UIElement Parent;
        public Dictionary<string, string> NamesDictionary = new Dictionary<string, string>();

        private void OnValidate()
        {
            //#if UNITY_EDITOR
            //      Guid = GUID.Generate().ToString();
            listPosition = this.transform.GetSiblingIndex();
            //#endif
        }
        public StepDetail StepShape = StepDetail.Default;
        protected void Awake()
        {
            GetNames();

        }
        public void GetNames()
        {
            var key = GetComponent<LocalizedTextAuto>().key;
            //NamesDictionary = Localizer.GetAllTexts(key, false);
        }
    }
}
