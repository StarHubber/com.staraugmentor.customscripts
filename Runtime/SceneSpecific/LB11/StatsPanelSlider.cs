using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
    public class StatsPanelSlider : MonoBehaviour
    {
        public static StatsPanelSlider Instance;

        [SerializeField]
        private Animation SlideInAnim;
        [SerializeField]
        private Image rowImage, bgImage;
        [SerializeField]
        private bool isVisible;
        [SerializeField]
        private Transform aderStatsContent;
        [SerializeField]
        private Animator StatsPanelAnimator;

        private Toggle toggle;
        [SerializeField]
        private Color activeRowColor, passiveRowColor, bgColor;

        void Start()
        {
            toggle = GetComponentInChildren<Toggle>(true);

        }
        private void OnEnable()
        {

            SteckerHandler.OnActiveSteckerChange += SlideStatsPanel;
        }
        private void OnDisable()
        {

            SteckerHandler.OnActiveSteckerChange -= SlideStatsPanel;
        }

        private void Awake()
        {
            Instance = this;
            SetActiveRowDesign(false);
        }


        public void SlideStatsPanel(Stecker activeTooltip, bool active, Clicktype clicktype)
        {
            SetActiveRowDesign(active);

            if (!active)
            {
                if (!isVisible) return;
                isVisible = false;

                StatsPanelAnimator.SetTrigger("SlideOut");
                ClearStatsTable();
                return;
            }

            else
            {
                aderStatsContent.gameObject.SetActive(true);

                if (!isVisible)
                {

                    StatsPanelAnimator.SetTrigger("SlideIn");



                    isVisible = true;
                    return;
                }
                if (isVisible)
                {


                    StatsPanelAnimator.SetTrigger("Loop");
                }




            }
            isVisible = true;

        }

        private void SetActiveRowDesign(bool v)
        {
            if (v)
            {
                rowImage.color = activeRowColor;
            }
            else
            {
                rowImage.color = passiveRowColor;
                bgImage.color = bgColor;
            }
        }

        public void ClearStatsTable()
        {
            aderStatsContent.gameObject.SetActive(false);
            //Clear the Ader table before filling it up with new entries
        }

    }
}