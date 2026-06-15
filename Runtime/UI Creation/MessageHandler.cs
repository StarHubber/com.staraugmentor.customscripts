using NaughtyAttributes;
using STAR.Utils;
using StarCooperation.Export;
using UnityEngine;
using UnityEngine.UI;
using static StarCooperation.MoveCamera;

namespace StarCooperation.ExportCCP
{
    [DefaultExecutionOrder(-9000)]
    public class MessageHandler : MonoBehaviour
    {
        public bool setGlobalCamera = false;
        public bool allowAnimation = false;
        public IMessageReceiver Interface { get; set; }
        public GameEvent StepClickedEvent, ExtensionClickedEvent;
        private void Awake()
        {
            Interface = FindObjectOfType<MessageReceiver>();
            Interface.UIItemClicked += UI_ItemClicked;
            Interface.UIExtensionClicked += UI_ItemExtensionClicked;
            Interface.UIBackButtonClicked += UI_BackButtonClicked;
            Interface.AllowCameraAnimationChanged += OnAllowCameraAnimationChange;
            Interface.CameraSettingsChanged += HandleCameraSettingsChange;
        }
        [Button]
        public void SetGlobalCamera()
        {
            HandleCameraSettingsChange("globalcamera", setGlobalCamera);
        }
        [Button]
        public void SetAllowCameraAnimation()
        {
            HandleCameraSettingsChange("allowanimation", allowAnimation);

        }
        public void SetBadgeAndHull(GameObject badge, GameObject hull)
        {
            Interface?.SetBadge(badge);
            Interface?.Sethull(hull);
        }
        private void HandleCameraSettingsChange(string key, bool boolean)
        {
            switch (key)
            {
                case "allowanimation":
                    FindObjectOfType<MoveCamera>().allowAnimation = boolean;

                    break;
                case "globalcamera":
                    FindObjectOfType<MoveCamera>().SetCameraMode(boolean ? CameraMode.GlobalCamera : CameraMode.Default);
                    break;
                default:
                    break;
            }
        }

        private void OnAllowCameraAnimationChange(bool obj)
        {
            FindObjectOfType<MoveCamera>().allowAnimation = obj;
        }

        private void UI_BackButtonClicked(string tabGuid)
        {
            var go = GameObject.Find("ButtonBack");
            go.GetComponent<Button>().onClick.Invoke();
        }

        private void UI_ItemExtensionClicked(string guid)
        {
            ExtensionClickedEvent?.RaiseString(guid);


            //var component = FindObjectsOfType<UIComponent>().Where(x => x.Guid == guid).First();
            //HandleExtensionClick(component, component.StepShape);

            //now do the extension move
        }

        private void HandleExtensionClick(UIComponent component, StepDetail stepShape)
        {
            switch (stepShape)
            {
                case StepDetail.Default:
                    component.GetComponent<Toggle>().isOn = true;
                    break;
                case StepDetail.Lupe:
                    component.GetComponent<LupenHandler>().buttonLupe.onClick.Invoke();
                    break;
                case StepDetail.Explosion:
                    break;
                case StepDetail.Information:
                    component.GetComponent<DocButtonHandler>().buttonOpenDoc.onClick.Invoke();
                    break;
                case StepDetail.Animation:
                    break;
                default:
                    break;
            }
        }

        public void HandleTooltipClick(string guid)
        {
            //We need to Tell the Viewer here that we want to activate the UI element with this guid
            // Debug.Log("Clicked: " + guid);
            //DataInterface.DATAObject.ToggleState(DataInterface.DATAObject.ReturnsCorrespondingInteractor(guid), "Toggles");
            Interface.OnHotSpotItemClicked(guid);
            /*if (guid == GetComponent<UIComponent>().Guid)
            {
                //this is me
                GetComponent<Toggle>().isOn = true;
            }*/
        }

        private void UI_ItemClicked(string guid)
        {
            StepClickedEvent?.RaiseString(guid);
        }


    }
}