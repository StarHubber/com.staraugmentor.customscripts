using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarCooperation
{
	public class LuftSystem : MonoBehaviour
	{
		public ParticleSystem[] particleSystemsFreshHot;
		public ParticleSystem[] particleSystemsColdHot;
		public List<ParticleSystem> particleSystemsPump;
		public ModelHighlighter highlighterWaermetauscher;

		[Space(5)]
		public Slider sliderSpeed;
		public Slider sliderTemperature;

		[Space(5)]
		public float rateMin = 15;
		public float rateMax = 100;
		public float dividerPump = 5;

		[Space(5)]
		public float speedMin = 0;
		public float speedMax = 3;

		[Space(5)]
		public Color colorFresh;
		public Color colorCold;
		public Color colorHot;

		private List<ParticleSystem> particleSystemsTotal;
		private bool acOn = false;

		private void OnEnable()
		{
			highlighterWaermetauscher.Highlight(true);
			if (particleSystemsTotal == null)
			{
				particleSystemsTotal = new List<ParticleSystem>();
				particleSystemsTotal.AddRange(particleSystemsFreshHot);
				particleSystemsTotal.AddRange(particleSystemsColdHot);
				particleSystemsTotal.AddRange(particleSystemsPump);
			}
			SetLuftMenge();
			SetAirTemperature();
		}

		private void OnDisable()
		{
			highlighterWaermetauscher.Highlight(false);
		}

		public void SetAirTemperature()
		{
			foreach (var ps in particleSystemsFreshHot)
			{
				var main = ps.main;
				//var newColor = Color.Lerp(colorFresh, colorHot, sliderTemperature.value);
				var newColor = acOn ? colorCold : colorFresh;
				main.startColor = newColor;

				ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
				ps.GetParticles(particles);
				for (int i = 0; i < particles.Length; i++)
				{
					particles[i].startColor = newColor;
				}
				ps.SetParticles(particles);
			}

			foreach (var ps in particleSystemsColdHot)
			{
				var main = ps.main;
				Color newColor;

				newColor = Color.Lerp(acOn ? colorCold : colorFresh, colorHot, sliderTemperature.value);

				main.startColor = newColor;
				ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
				ps.GetParticles(particles);
				for (int i = 0; i < particles.Length; i++)
				{
					particles[i].startColor = newColor;
				}
				ps.SetParticles(particles);
			}

			highlighterWaermetauscher.highlightMat.color = Color.Lerp(colorFresh, colorHot, sliderTemperature.value / sliderTemperature.maxValue);
		}

		public void SetLuftMenge()
		{
			foreach (var ps in particleSystemsTotal)
			{
				ps.Stop();
				ps.Clear();

				var emission = ps.emission;
				emission.rateOverTime = Mathf.Lerp(rateMin, rateMax, (sliderSpeed.value - sliderSpeed.minValue) / (sliderSpeed.maxValue - sliderSpeed.minValue));
				if (particleSystemsPump.Contains(ps))
				{
					emission.rateOverTime = emission.rateOverTime.constant / dividerPump;
				}

				var main = ps.main;
				main.startSpeed = Mathf.Lerp(speedMin, speedMax, (sliderSpeed.value - sliderSpeed.minValue) / (sliderSpeed.maxValue - sliderSpeed.minValue));
				if (particleSystemsPump.Contains(ps))
				{
					main.startSpeed = main.startSpeed.constant / dividerPump;
				}

				var oldLifetime = main.startLifetime;
				var newLifetime = ps.gameObject.GetComponent<BLINDED_AM_ME.ParticlePathFlow>().CalculateNewLifetime();
				main.startLifetime = newLifetime != 0 ? newLifetime : oldLifetime;    // Calculated lifetime == 0 when particle are stopped (or not started yet)

				if (main.startSpeed.constant != 0)
				{
					ps.Play();
				}
			}
		}

		public void SetACOnOff(bool on)
		{
			acOn = on;
			SetAirTemperature();
		}
	}
}