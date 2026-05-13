using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace StarCooperation
{
    public class PedalActivator : MonoBehaviour, IPointerClickHandler

    {
        public ABS_Animation abs;
        private bool flag = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }



        public void OnPointerClick(PointerEventData eventData)
        {
            if (!flag)
            {
                abs.StartCar();
                abs.Brake();
                flag = true;
            }
            else
            {
                abs.Idle();
                flag = false;
            }
        }
    }
}
