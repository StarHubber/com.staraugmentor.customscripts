using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StarCooperation
{
    public class ESP_Anim : MonoBehaviour
    {
        private Material tempMat;
        public Material redMat, defaultMat, greenMat;
        public MeshRenderer tireArrowLeft, tireArrowRight;
        public ABS_Animation abs;
        public ParticleSystem hydraulicPS, signalPS, schlauchPS;
        public ParticleSystem hydraulicPSright, signalPSright, schlauchPSright;
        public ParticleSystem hydraulicPSfrontLeft, signalPSfrontLeft, schlauchPSfrontLeft;

        public ParticleSystem[] restSignalPS, restSchlauchPS;
        public Toggle espToggle, panelToggle;
        public GameObject dirArrow, wishArrow, espIcon;
        public Color signalDefaultColor, signalBrakeColor;

        // Start is called before the first frame update
        private void Start()
        {
            defaultMat = tireArrowLeft.material;

            espToggle.onValueChanged.AddListener(delegate
            {
                StartESPAnimation(espToggle.isOn);
            });

            panelToggle.onValueChanged.AddListener(delegate
            {
                SetDefaultState(panelToggle.isOn);
            });
        }

        // Update is called once per frame
        private void Update()
        {

        }

        private void SetDefaultState(bool start)
        {
            if (start)
            {
                //Activation happens already in ABS
            }
            else
            {
                //Debug.Log("default state jetzt");
                dirArrow.gameObject.SetActive(false);
                wishArrow.gameObject.SetActive(false);
                this.GetComponent<Animator>().SetBool("ActivateESP", false);

                hydraulicPS.gameObject.SetActive(true);
                hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                hydraulicPS.Play();
                hydraulicPSright.gameObject.SetActive(true);
                hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                hydraulicPSright.Play();
            }

        }
        public void StartESPAnimation(bool start)
        {
            if (start)
            {
                dirArrow.gameObject.SetActive(true);
                wishArrow.gameObject.SetActive(true);

                this.GetComponent<Animator>().SetBool("ActivateESP", true);
            }
            else
            {
                var animator = GetComponent<Animator>();
                animator.SetBool("ActivateESP", false);
                ReleaseBrake();
                DefaultArrowColor();
                dirArrow.gameObject.SetActive(false);
                wishArrow.gameObject.SetActive(false);
                hydraulicPS.gameObject.SetActive(true);
                hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                hydraulicPS.Play();
                hydraulicPSright.gameObject.SetActive(true);
                hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                hydraulicPSright.Play();
            }
        }

        public void DefaultArrowColor()
        {
            tireArrowLeft.material = defaultMat;
            tireArrowRight.material = defaultMat;
        }

        public void BrakeArrowColor(bool isUnderSteer)
        {
            if (isUnderSteer)
            {
                tireArrowLeft.material = redMat;
            }
            else if (!isUnderSteer)
            {
                tireArrowRight.material = redMat;
            }
            // Set Arrow Color to red
        }

        public void ActivateHydraulicUndersteer()
        {
            hydraulicPS.gameObject.SetActive(true);
            hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 0);

           // hydraulicPS.Simulate(0.0f, true, true);
         //   hydraulicPS.Stop();
            signalPS.gameObject.SetActive(true);
            schlauchPS.gameObject.SetActive(true);
            signalPSright.gameObject.SetActive(true);
            schlauchPSright.gameObject.SetActive(true);

            foreach (var item in restSignalPS)
            {
                item.gameObject.SetActive(true);
                item.Simulate(0.0f, true, true);
                item.Play();
            }
            foreach (var item in restSchlauchPS)
            {
                item.gameObject.SetActive(true);
                item.Simulate(0.0f, true, true);
                item.Play();
            }
        }

        public void ActivateHydraulicOversteer()
        {
           // hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 1f);
            hydraulicPSfrontLeft.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 1f);

            //  hydraulicPSright.gameObject.SetActive(true);
            hydraulicPSfrontLeft.gameObject.SetActive(true);

            //  hydraulicPSright.Simulate(0.0f, true, true);

            // hydraulicPSright.Play();
            hydraulicPSfrontLeft.Play();

            //  signalPSright.gameObject.SetActive(true);
            signalPSfrontLeft.gameObject.SetActive(true);


            //  schlauchPSright.gameObject.SetActive(true);
            schlauchPSfrontLeft.gameObject.SetActive(true);


            foreach (var item in restSignalPS)
            {
                item.gameObject.SetActive(true);
                item.Simulate(0.0f, true, true);
                item.Play();
            }

            foreach (var item in restSchlauchPS)
            {
                item.gameObject.SetActive(true);
                item.Simulate(0.0f, true, true);
                item.Play();
            }
        }

        public void DeactivatePS()
        {
            signalPS.gameObject.SetActive(false);
            schlauchPS.gameObject.SetActive(false);
        }

        public void UnderSteer()
        {
            hydraulicPS.gameObject.SetActive(true);
            hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 1);

          //  hydraulicPS.Simulate(0.0f, true, true);
            hydraulicPS.Play();

            foreach (var item in restSchlauchPS)
            {
                var main = item.main;
                main.startColor = signalDefaultColor;
                item.Stop();
                item.Simulate(0.0f, true, true);
                item.Play();
            }

            foreach (var item in restSignalPS)
            {
                item.GetComponent<ParticleSystemRenderer>().material = greenMat;


                item.Stop();
                item.Simulate(0.0f, true, true);
                item.Play();
            }

            SetSignalColor(true, signalPS, schlauchPS);
            BrakeArrowColor(true);
            espIcon.GetComponent<Animation>().Play("ESP_Blink");

        }

        public void OverSteer()
        {
           // SetSignalColor(true, signalPSright, schlauchPSright);
            SetSignalColor(true, signalPSfrontLeft, schlauchPSfrontLeft);
            BrakeArrowColor(false);
            espIcon.GetComponent<Animation>().Play("ESP_Blink");

        }

        public void SetSignalColor(bool isRedSignal, ParticleSystem signalSystem, ParticleSystem schlauchSystem)
        {
            if (isRedSignal)
            {
                var main = schlauchSystem.main;
                signalSystem.GetComponent<ParticleSystemRenderer>().material = redMat;

                main.startColor = signalBrakeColor;
                schlauchSystem.Stop();
                schlauchSystem.Simulate(0.0f, true, true);
                schlauchSystem.Play();
            }
            else
            {
                var main = schlauchSystem.main;
                signalSystem.GetComponent<ParticleSystemRenderer>().material = greenMat;

                main.startColor = signalDefaultColor;
                schlauchSystem.Stop();
                schlauchSystem.Simulate(0.0f, true, true);
                schlauchSystem.Play();
            }
        }

        public void SetSignalToRed()
        {
            hydraulicPS.Play();

            signalPS.GetComponent<ParticleSystemRenderer>().material = redMat;
            var main = schlauchPS.main;
            schlauchPS.gameObject.SetActive(false);
            main.startColor = signalBrakeColor;
            schlauchPS.gameObject.SetActive(true);
            schlauchPS.Stop();
            schlauchPS.Simulate(0.0f, true, true);
            schlauchPS.Play();

        }

        public void ReleaseBrake()
        {
           // hydraulicPS.Pause();
            hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 0f);
            //hydraulicPSright.Pause();
            hydraulicPSright.GetComponent<ParticleSystemRenderer>().material.color = new Color(hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.r, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.g, hydraulicPS.GetComponent<ParticleSystemRenderer>().material.color.b, 0f);

            SetSignalColor(false, signalPS, schlauchPS);
            SetSignalColor(false, signalPSright, schlauchPSright);
            DefaultArrowColor();
           // espIcon.GetComponent<Animation>().Stop("ESP_Blink");

        }
    }
}

