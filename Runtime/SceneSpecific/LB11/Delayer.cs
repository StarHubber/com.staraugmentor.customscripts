using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarCooperation
{
    public class Delayer : MonoBehaviour, IInteractionDelay
    {
        public static Delayer Instance;

        public float DelayTime = 1f;
        private DelayInteraction[] objs;
        private IEnumerator activeRoutine;
        public void DisableInteraction()
        {
            GetObjs();
            foreach (var item in objs)
            {
                if (item == this) continue;
                item.DisableInteraction();
            }
        }

        public void EnableInteraction()
        {
            GetObjs();
            foreach (var item in objs)
            {
                if (item == this) continue;
                item.EnableInteraction();
            }
        }

        private void GetObjs()
        {
            objs = GameObject.FindObjectsOfType<DelayInteraction>();
        }
        public void Notify()
        {
            if (activeRoutine != null) return;
            DisableInteraction();
            activeRoutine = DelayRoutine();
            StartCoroutine(DelayRoutine());
        }
        private IEnumerator DelayRoutine()
        {
            yield return new WaitForSeconds(DelayTime);
            EnableInteraction();
            activeRoutine = null;
        }

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }
        private void Start()
        {

        }
    }
}