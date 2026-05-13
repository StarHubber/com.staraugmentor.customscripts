using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StarCooperation
{
    public class DelayInteraction : MonoBehaviour, IInteractionDelay
    {
        private Button _button;
        private Toggle _toggle;


        public void DisableInteraction()
        {
            if (_button) _button.interactable = false;
            if (_toggle) _toggle.interactable = false;
        }

        public void EnableInteraction()
        {
            if (_button) _button.interactable = true;
            if (_toggle) _toggle.interactable = true;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _toggle = GetComponent<Toggle>();
            _button?.onClick.AddListener(delegate { Notify(); });
            _toggle?.onValueChanged.AddListener(delegate { Notify(); });



        }

        public void Notify()
        {
            Delayer.Instance.Notify();
        }
    }
}