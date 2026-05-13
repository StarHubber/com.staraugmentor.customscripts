using System;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    [DefaultExecutionOrder(-100)]
    public class InteractionControl : MonoBehaviour
    {
        public static event Action<bool> UserInteractionsDisabled;
        public static bool InteractionsDisabled { get; private set; }

        [Header("Interaction Control")]
        public CanvasGroup canvasGroupMainTA;
        public CanvasGroup canvasGroupMenuTA;
        public Button buttonMenuBack;
        public static float menuDisabledAlpha = 0.3f;
        public CanvasGroup canvasGroupBlockDiagramTA;
        public CanvasGroup canvasGroupBlockDiagramHL;

        [Header("Debug Trainermode")]
        public bool trainerMode = false;
        public bool isTrainer = false;
        private bool lastTrainerMode;
        private bool lastIsTrainer;

        private class TrainerSettings
        {
            public TrainerSettings(bool trainerMode, bool isTrainer)
            {
                this.trainerMode = trainerMode;
                this.isTrainer = isTrainer;
            }

            public bool trainerMode;
            public bool isTrainer;
        }

        private TrainerSettings trainerSettings;

        private void Awake()
        {
#if UNITY_EDITOR
            trainerSettings = new TrainerSettings(trainerMode, isTrainer);
#else
			trainerSettings = new TrainerSettings(
				trainerMode = HoloRepair.Core.ContentAppInterface.InteractionMode == HoloRepair.Core.InteractionMode.Trainer,
				isTrainer = HoloRepair.Core.ContentAppInterface.IsTrainer);
#endif
        }

        private void OnEnable()
        {
            HoloRepair.Core.ContentAppInterface.OnInteractionModeChanged += OnInteractionModeChanged;
            HoloRepair.Core.ContentAppInterface.OnIsTrainerChanged += OnIsTrainerChanged;
        }

        private void OnDisable()
        {
            HoloRepair.Core.ContentAppInterface.OnInteractionModeChanged -= OnInteractionModeChanged;
            HoloRepair.Core.ContentAppInterface.OnIsTrainerChanged -= OnIsTrainerChanged;
        }

        private void Start()
        {
            SetInteractionAndTransmissionMode();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (trainerMode != lastTrainerMode || isTrainer != lastIsTrainer)
            {
                trainerSettings = new TrainerSettings(trainerMode, isTrainer);
                SetInteractionAndTransmissionMode();
                lastTrainerMode = trainerMode;
                lastIsTrainer = isTrainer;
            }
#endif
        }

        /// <summary>
        /// Event listener for interaction mode change. Enables or disables Hololens user interaction and transmission.
        /// </summary>
        /// <param name="args"></param>
        private void OnInteractionModeChanged(HoloRepair.Core.InteractionModeEventArgs args)
        {
            trainerSettings.trainerMode = args.InteractionMode == HoloRepair.Core.InteractionMode.Trainer;
            SetInteractionAndTransmissionMode();
        }

        /// <summary>
        /// Event listener for isTrainer changed. Enables or disables Hololens user interaction and transmission.
        /// </summary>
        /// <param name="args"></param>
        private void OnIsTrainerChanged(HoloRepair.Core.IsTrainerEventArgs args)
        {
            trainerSettings.isTrainer = args.IsTrainer;
            SetInteractionAndTransmissionMode();
        }

        /// <summary>
        /// Enable or disable user interactions and transmission/reception based on Trainer settings.
        /// </summary>
        /// <param name="trainerParams">[0]: isTrainer, [1]: trainerMode</param>
        private void SetInteractionAndTransmissionMode()
        {
            // Those (legacy) flags are just to debug/simulate trainer mode in Editor, and for readibility in this function
            isTrainer = trainerSettings.isTrainer;
            trainerMode = trainerSettings.trainerMode;

            InteractionsDisabled = trainerMode && !isTrainer;
            DisableUserInteractions(InteractionsDisabled);


        }

        /// <summary>
        /// Disable user interactions, used for Trainer mode.
        /// </summary>
        /// <param name="disable"></param>
        private void DisableUserInteractions(bool disable)
        {
            canvasGroupMainTA.interactable = !disable;

            canvasGroupMenuTA.alpha = disable ? menuDisabledAlpha : 1;
            canvasGroupMenuTA.interactable = !disable;

            buttonMenuBack.interactable = !disable;

            canvasGroupBlockDiagramTA.interactable = !disable;
            canvasGroupBlockDiagramHL.interactable = !disable;

            UserInteractionsDisabled?.Invoke(disable);
        }
    }
}