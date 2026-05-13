using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloRepair.Core;
using StarCooperation.Export;

namespace StarCooperation
{
    public class HololensControl : MonoBehaviour
    {
        [Header("Calibration")]
        public List<GameObject> objectsToDeactivate;
        public ModelControl control;

        [Header("Hologram Placement")]
        public GameObject movedModel;
        public float flyTime = 1;

        //private bool anchoringBool = false;

        private void Awake()
        {
            control = FindObjectOfType<ModelControl>();
            //// Get MRTK objects from scene and add to list for deactivation
            //if (autoDeactivateMRTK)
            //{
            //	objectsToDeactivate.Add(FindObjectOfType<MixedRealityInputModule>().transform.parent.gameObject);
            //}
            ContentAppInterface.OnCalibrationFinished += OnCalibrationFinished;
            HoloRepairInterface.anchoringStarted += StartCalibration;
        }

        public void anchoringKnown(bool obj)
        {
            HoloRepairInterface.anchoringBool = obj;
        }
        //#if !UNITY_EDITOR
        private IEnumerator Start()
        {
            // Wait one frame to give ARAnchor manager time to run OnLegacyContentLoaded callback,
            // then run positioning routine.
            ActivateModelObjects(false);
            yield return null;
            PlaceModelAtCarPosition();
        }
        //#endif
        private void OnDestroy()
        {
            HoloRepair.Core.ContentAppInterface.OnCalibrationFinished -= OnCalibrationFinished;
            HoloRepair.Core.HoloRepairInterface.anchoringStarted -= StartCalibration;
        }

        /// <summary>
        /// Place model at real world car's position and rotation, only valid for Hololens application
        /// </summary>
        public void PlaceModelAtCarPosition()
        {
            StartCoroutine(DoPlaceModelAtCarPosition());
        }

        /// <summary>
        /// Coroutine to make model fly back.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DoPlaceModelAtCarPosition()
        {
            Vector3 startPos = movedModel.transform.position;
            Quaternion startRot = movedModel.transform.rotation;
            Vector3 startScale = movedModel.transform.localScale;

            var targetPos = HoloRepair.Core.ContentAppInterface.ContentPosition;
            if (targetPos == Vector3.zero)
            {
                StartCalibration();
                yield break;
                //targetPos = new Vector3(0, -1, 3);
            }

            // Compensate target rotation for legacy rotation corrector (e.g. correct rotation corrector ^^)
            Quaternion targetRot = HoloRepair.Core.ContentAppInterface.ContentRotation;
            if (!HoloRepairInterface.anchoringBool)
                targetRot = targetRot * Quaternion.Euler(control.modelHull.transform.rotation.eulerAngles);

            ActivateModelObjects(true);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / flyTime;
                t = Mathf.Clamp01(t);
                movedModel.transform.position = Vector3.Slerp(startPos, targetPos, t);
                movedModel.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                movedModel.transform.localScale = Vector3.Slerp(startScale, Vector3.one, t);
                yield return null;
            }
        }

        /// <summary>
        /// Call calibration routine from HoloRepaire.Core plugin.
        /// </summary>
        public void StartCalibration()
        {
            // Deactivate own content
            ActivateModelObjects(false);
            //foreach (var obj in objectsToDeactivate)
            //{
            //	obj.SetActive(false);
            //}

            // Start calibration and assign to finished event
            if (!HoloRepairInterface.anchoringBool)
                HoloRepair.Core.ContentAppInterface.StartCalibration();
            //HoloRepair.Core.ContentAppInterface.OnCalibrationFinished += CalibrationFinished;
        }

        /// <summary>
        /// Called when calibration finished.
        /// </summary>
        /// <param name="args"></param>
        private void OnCalibrationFinished(HoloRepair.Core.CalibrationEventArgs args)
        {
            // Re-activate own content
            ActivateModelObjects(true);
            //foreach (var obj in objectsToDeactivate)
            //{
            //	obj.SetActive(true);
            //}

            // Place model at new position
            PlaceModelAtCarPosition();

            // Un-assign event
            //HoloRepair.Core.ContentAppInterface.OnCalibrationFinished -= CalibrationFinished;
        }

        private void ActivateModelObjects(bool activate)
        {
            foreach (var obj in objectsToDeactivate)
            {
                obj.SetActive(activate);
            }
        }
    }
}