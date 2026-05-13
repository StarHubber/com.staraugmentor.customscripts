//    MIT License
//    
//    Copyright (c) 2017 Dustin Whirle
//    
//    My Youtube stuff: https://www.youtube.com/playlist?list=PL-sp8pM7xzbVls1NovXqwgfBQiwhTA_Ya
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.

using UnityEngine;

#if UNITY_EDITOR
#endif

namespace BLINDED_AM_ME
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ParticleSystem))]
	[RequireComponent(typeof(Path_Comp))]
	public class ParticlePathFlow : MonoBehaviour
	{
		public bool isPathUpdating = false;
		public bool hasRandomStartingPoints = false;

		// STAR
		[Space(5)]
		public bool autoSetLifetimeForOneRun = false;
		public bool autoRotatePointsIntoForward = false;
		public bool precalculatePathPoints = false;
		public int precalculationFPS = 60;
		[Space(5)]
		//public bool particlesMoveWithinPathWidth = false;
		public bool entireLinearPathWidth = false;
		// STAR end

		[Range(0.0f, 5.0f)]
		public float pathWidth = 0.0f;

		// STAR
		public bool separatePathWidthAxes = false;
		[Range(0.0f, 5.0f)]
		public float pathWidthX = 0.0f;
		[Range(0.0f, 5.0f)]
		public float pathWidthY = 0.0f;
		public bool pathWidthOverLifetime = false;
		public AnimationCurve pathWidthOverLifetimeCurve;
		// STAR end

		private ParticleSystem.Particle[] _particle_array;
		private ParticleSystem _particle_system;
		private Path_Comp _path_comp;

		private int _numParticles;

		// STAR
		private float currentLifetime;
		private Path_Point[] pathPoints;
		// STAR end

#if UNITY_EDITOR
		private void Reset()
		{
			Start();
		}

		private void OnValidate()
		{
			Start();
		}
#endif

		private void Start()
		{
			_path_comp = GetComponent<Path_Comp>();
			_particle_system = GetComponent<ParticleSystem>();
			_particle_array = new ParticleSystem.Particle[_particle_system.main.maxParticles];
			// STAR
			currentLifetime = 0;

			// Todo: On application playing, remove all children to avoid overhead when moving particles (on HL).
			// But: Check for not updating childcount etc. in update routine - tested: not a big deal on HL yet, but could be improved...

			//if (!isPathUpdating)
			//{
			//	for (int i = transform.childCount; i > 0; i--)
			//	{
			//		Destroy(transform.GetChild(i));
			//	}
			//}
		}

		private void LateUpdate()
		{
			if (_particle_array == null)
			{
				Start();
				_path_comp.Update_Path();
			}
			else if (isPathUpdating)
			{
				_path_comp.Update_Path();
			}

			// STAR
			if (_path_comp.TotalDistance == 0)
			{
				return;
			}

			if (autoSetLifetimeForOneRun)
			{
				var main = _particle_system.main;
				float newLifetime = main.startSpeed.constant != 0 ? CalculateNewLifetime() : main.duration;
				if (newLifetime != currentLifetime)
				{
					main.startLifetime = newLifetime;
					currentLifetime = newLifetime;

					if (precalculatePathPoints)
					{
						GetAllPathPoints();
					}
				}
			}
			// STAR end

			_numParticles = _particle_system.GetParticles(_particle_array);


			if (_numParticles > 0)
			{
				for (int i = 0; i < _numParticles; i++)
				{
					ParticleSystem.Particle obj = _particle_array[i];

					// This made it based on the particle lifetime
					//					float normalizedLifetime = (1.0f - obj.remainingLifetime / obj.startLifetime);
					//
					//					if(hasRandomStartingPoints){
					//						normalizedLifetime += Get_Value_From_Random_Seed_0t1(obj.randomSeed, 100.0f);
					//						normalizedLifetime = normalizedLifetime % 1.0f;
					//					}
					//
					//					Path_Point axis = _path_comp.GetPathPoint(_path_comp.TotalDistance * normalizedLifetime);

					// This made it based on the paritcle speed
					var passedLifeTime = obj.startLifetime - obj.remainingLifetime;
					float dist = passedLifeTime * obj.velocity.magnitude;

					if (hasRandomStartingPoints)
					{
						dist += Get_Value_From_Random_Seed_0t1(obj.randomSeed, 100.0f) * _path_comp.TotalDistance;
					}

					dist = dist % _path_comp.TotalDistance;

					// STAR
					Path_Point axis;
					if (!precalculatePathPoints)
					{
						axis = _path_comp.GetPathPoint(dist);
					}
					else if (pathPoints != null && pathPoints.Length > 0)
					{
						var pathPointIdx = Mathf.FloorToInt(dist / _path_comp.TotalDistance * pathPoints.Length);
						axis = pathPoints[pathPointIdx];
					}
					else
					{
						return;
					}
					// STAR end

					// STAR (lots) changes below
					if (pathWidth > 0 || separatePathWidthAxes == true)
					{
						var offset = Vector2.zero;
						var randomValue = 0f;
						if (entireLinearPathWidth)
						{
							var randomOffset = Get_Value_From_Random_Seed_0t1(obj.randomSeed, 100.0f);
							if (obj.randomSeed % 2 != 0)
							{
								randomOffset = -randomOffset;
							}
							offset.x = randomOffset;
							offset.y = randomOffset;

							randomValue = 1f;
						}
						else
						{
							offset = Math_Functions.AngleToVector2D(obj.randomSeed % 360.0f);
							randomValue = Get_Value_From_Random_Seed_0t1(obj.randomSeed, 100.0f);
						}

						// todo
						//if (particlesMoveWithinPathWidth)
						//{
						//	offset *= Mathf.Cos(passedLifeTime)
						//}

						var passedLifetimeRel = passedLifeTime / obj.startLifetime;

						if (!separatePathWidthAxes)
						{
							offset *= randomValue * pathWidth;

							if (pathWidthOverLifetime)
							{
								offset *= pathWidthOverLifetimeCurve.Evaluate(passedLifetimeRel);
							}

							_particle_array[i].position = axis.point +
							(axis.right * offset.x) +
							(axis.up * offset.y);
						}
						else
						{
							var offsetRandom = offset * randomValue;
							var offsetX = offsetRandom * pathWidthX;
							var offsetY = offsetRandom * pathWidthY;

							if (pathWidthOverLifetime)
							{
								offsetX *= pathWidthOverLifetimeCurve.Evaluate(passedLifetimeRel);
								offsetY *= pathWidthOverLifetimeCurve.Evaluate(passedLifetimeRel);
							}

							_particle_array[i].position = axis.point +
								(axis.right * offsetX.x) +
								(axis.up * offsetY.y);
						}
					}
					else
					{
						// STAR: multiplication optimization
						_particle_array[i].position = axis.point;
					}
				}
				_particle_system.SetParticles(_particle_array, _numParticles);
			}
		}

		public float CalculateNewLifetime()
		{
			if (_particle_system == null)
			{
				return 0;
			}
			var main = _particle_system.main;
			return _path_comp.TotalDistance / main.startSpeed.constant;
		}

		private void GetAllPathPoints()
		{
			// STAR : Calculate all steps in between after lifetime calculated
			var stepCount = Mathf.FloorToInt(currentLifetime * (float)precalculationFPS);
			pathPoints = new Path_Point[stepCount];

			float dist = 0;
			for (int i = 0; i < stepCount; i++)
			{
				pathPoints[i] = _path_comp.GetPathPoint(dist);
				dist += _path_comp.TotalDistance / stepCount;
			}
		}

		private float Get_Value_From_Random_Seed_0t1(float seed, float converter)
		{
			return (seed % converter) / converter;
		}
	}
}