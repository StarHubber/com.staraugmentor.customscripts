using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace StarCooperation
{
    public class DisableCameraMovement : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera.instance.allowMovement = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            MoveCamera.instance.allowMovement = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            MoveCamera.instance.allowMovement = false;
        }
    }
}