using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI

{
    public class TestMessage : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("TestMessage");
            var go = GameObject.Find("ViewerInterface");
            Debug.Log(go.name);

            var handler = go.GetComponent<IMessageHandler>();
            handler.UiClicked += Test;
        }

        private void Test(string obj)
        {
            Debug.Log(obj + " clicked");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}