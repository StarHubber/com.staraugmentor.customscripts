using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace StarCooperation
{
	public class ABS_Animation : MonoBehaviour
	{

		public GameObject[] tires;
        public GameObject hullTires, ABSicon;
		public MeshRenderer[] tireArrows;
		public ParticleSystem[] hydraulicsPS, brakePS, schlauchPS;
		public Animation pedalAnim;
		public MeshRenderer pedalStange, pedal;
		public Material highlightMaterial, greyMaterial, redSignalMaterial, greenSignalMaterial, arrowBrakeMaterial, arrowIdleMaterial;
		public Toggle panelToggle, absToggle, espToggle;
        public Color signalDefaultColor, signalBrakeColor;
		private Animator carController;
		private enum CarState { idle, brake, abs, esp, start };
		private bool pedalFlag = false;


        private void Start()
        {
            panelToggle.onValueChanged.AddListener(delegate
            {
                SetCarState(CarState.start, panelToggle);
            });
            absToggle.onValueChanged.AddListener(delegate
            {
                SetCarState(CarState.abs, absToggle);
            });

            foreach (ParticleSystem item in hydraulicsPS)
            {
                item.gameObject.SetActive(true);
                item.GetComponent<ParticleSystemRenderer>().material.color = new Color(item.GetComponent<ParticleSystemRenderer>().material.color.r, item.GetComponent<ParticleSystemRenderer>().material.color.g, item.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                item.Play();


            }
        }
            private void OnEnable()
		{
			carController = GetComponent<Animator>();
		}
		private void SetCarState(CarState carstate, Toggle toggle)
		{
			switch (carstate)
			{
				case CarState.idle:
					if (toggle.isOn)
					{
						Idle();
					}
					else if (!toggle.isOn)
					{
						Idle();
					}
					break;

				case CarState.abs:
					ToggleABS(toggle.isOn);
					break;

				case CarState.start:
					if (toggle.isOn)
					{
						if (hullTires != null)
						{
							hullTires.SetActive(false);
						}

						StartCar();
					}
					else
					{
						if (hullTires != null)
						{
							hullTires.SetActive(true);
						}

						SetDefaultState();

					}
					break;
			}
		}
		public void SetDefaultState()
		{
			carController.SetBool("ActivateABS", false);
			Idle();
			foreach (var item in hydraulicsPS)
			{
				item.gameObject.SetActive(true);
                item.GetComponent<ParticleSystemRenderer>().material.color = new Color(item.GetComponent<ParticleSystemRenderer>().material.color.r, item.GetComponent<ParticleSystemRenderer>().material.color.g, item.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                item.Play();
            }
			foreach (var item in brakePS)
			{
				item.gameObject.SetActive(false);
			}
			foreach (var item in schlauchPS)
			{
				item.gameObject.SetActive(false);
			}
			foreach (var item in tires)
			{
				item.gameObject.SetActive(false);
			}
			if (pedalFlag)
			{
				pedalStange.material = greyMaterial;
				pedal.material = greyMaterial;
				pedalAnim.Play("PedalRelease");
				pedalFlag = false;
			}

		}
		public void StartCar()
		{
			foreach (var item in brakePS)
			{
				item.gameObject.SetActive(true);
			}
			foreach (var item in schlauchPS)
			{
				item.gameObject.SetActive(true);
			}
			foreach (var item in tires)
			{
				item.gameObject.SetActive(true);
			}
            foreach (ParticleSystem item in hydraulicsPS)
            {
                item.gameObject.SetActive(true);
                item.GetComponent<ParticleSystemRenderer>().material.color = new Color(item.GetComponent<ParticleSystemRenderer>().material.color.r, item.GetComponent<ParticleSystemRenderer>().material.color.g, item.GetComponent<ParticleSystemRenderer>().material.color.b, 0);
                item.Play();

            }


        }

		public void ToggleESP(bool activateESP)
		{
			if (activateESP)
			{
				carController.SetBool("ActivateESP", true);
			}
			else
			{

				carController.SetBool("ActivateESP", false);
				StopAllCoroutines();
				StartCar();
				Idle();
			}

		}
		public void ToggleABS(bool activateABS)
		{
			if (activateABS)
			{
				carController.SetBool("ActivateABS", true);
			}
			else
			{

				carController.SetBool("ActivateABS", false);
				Idle();
			}
		}
		private void ToggleHydraulikAnimation(bool isBraking)
		{
			if (!isBraking)
			{
				foreach (ParticleSystem item in hydraulicsPS)
				{
					item.GetComponent<ParticleSystemRenderer>().material.color = new Color(item.GetComponent<ParticleSystemRenderer>().material.color.r, item.GetComponent<ParticleSystemRenderer>().material.color.g, item.GetComponent<ParticleSystemRenderer>().material.color.b, 0f);
					//item.Pause();

				}
			}
			else if (isBraking)
			{
				foreach (ParticleSystem item in hydraulicsPS)
				{
					item.gameObject.SetActive(true);
					item.GetComponent<ParticleSystemRenderer>().material.color = new Color(item.GetComponent<ParticleSystemRenderer>().material.color.r, item.GetComponent<ParticleSystemRenderer>().material.color.g, item.GetComponent<ParticleSystemRenderer>().material.color.b, 1f);
                  //  item.Stop();
                  //  item.Simulate(4f,true, true, true);
					//item.Play();



                }

			}
		}
		public void SetSignalColor(bool isRedSignal)
		{
			if (isRedSignal)
			{
				foreach (ParticleSystem item in brakePS)
				{
					item.GetComponent<ParticleSystemRenderer>().material = redSignalMaterial;

					//item.Simulate(0.0f, true, true);

					item.Play();

				}
				foreach (ParticleSystem item in schlauchPS)
				{
                    var main = item.main;
                    main.startColor = signalBrakeColor;

					item.Stop();
					item.Simulate(0.0f, true, true);
					item.Play();
				}
			}
			else
			{
				foreach (ParticleSystem item in brakePS)
				{
					item.GetComponent<ParticleSystemRenderer>().material = greenSignalMaterial;

					//  item.startColor = color;
					//   item.Stop();
					//  item.Simulate(0.0f, true, true);
					//   item.Play();
				}
				foreach (ParticleSystem item in schlauchPS)
				{
                    var main = item.main;
                    main.startColor = signalDefaultColor;
					item.Stop();
					item.Simulate(0.0f, true, true);
					item.Play();
				}
			}
		}

		public void Brake()
		{
			ToggleHydraulikAnimation(true);
			SetSignalColor(true);
			foreach (var item in tireArrows)
			{
				item.material = arrowBrakeMaterial;
			}


		}
        public void ABSIdle()
        {
            SetSignalColor(false);
            // ToggleHydraulikAnimation(false);
            //DefaultArrowColor();

        }
        public void Idle()
		{
            if (pedalFlag)
            {
                AnimatePedalRelease();
            }


            SetSignalColor(false);
			ToggleHydraulikAnimation(false);
			foreach (var item in tireArrows)
			{
				item.material = arrowIdleMaterial;
			}

            
		}

		public void AnimateBrakePedal()
		{

			pedalAnim.Play("PedalBreak");
			pedalStange.material = highlightMaterial;
			pedal.material = highlightMaterial;
			pedalFlag = true;


		}
		public void AnimatePedalPulse()
		{

			pedalAnim.Play("PedalPulse");
			pedal.material = highlightMaterial;
            pedalFlag = true;
            ABSicon.GetComponent<Animation>().Play("ABS_Blink");


        }
        public void AnimatePedalRelease()
		{
			pedalFlag = false;

			pedalAnim.Play("PedalRelease");
			pedalStange.material = greyMaterial;
			pedal.material = greyMaterial;
            //ABSicon.GetComponent<Animation>().Stop("ABS_Blink");

        }

        public void DefaultArrowColor()
		{
			foreach (var item in tireArrows)
			{
				item.material = arrowIdleMaterial;
			}


		}
		public void BrakeArrowColor(bool isUnderSteer)
		{
			if (isUnderSteer)
			{
				foreach (var item in tireArrows)
				{
					if (item.name == "ArrowBackLeft")
					{
						item.material = arrowBrakeMaterial;
					}
				}
			}
			else
			{
				foreach (var item in tireArrows)
				{
					if (item.name == "ArrowBackRight")
					{
						item.material = arrowBrakeMaterial;
					}
				}
			}


		}





	}
}




