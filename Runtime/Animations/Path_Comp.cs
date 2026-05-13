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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLINDED_AM_ME
{
	[ExecuteInEditMode]
	public class Path_Comp : MonoBehaviour
	{
		// Todo: When flags changed, recalculate everything - check if best done via Property or Custom Editor Window.

		// inspector variables
		public bool isSmooth = true;
		public bool isCircuit = false;

		// STAR
		public bool drawGizmos = true;
		public bool drawUpRightLines = true;
		// STAR end

		[Range(0.001f, 1.0f)]
		public float gizmoLineSize = 1.0f;

		[HideInInspector]
		public Path _path = new Path();

		public float TotalDistance
		{
			get
			{
				return _path.TotalDistance;
			}
		}

		// STAR
		private int childCount;
		private Vector3[] childPos;
		private Path_Point[] gizmoPoints;

#if UNITY_EDITOR
		private void Reset()
		{
			Update_Path(true);
		}

		private void OnValidate()
		{
			Update_Path(true);
		}
#endif

		//void Awake()
		private void Start()
		{
			Update_Path(true);
		}

		public void Update_Path(bool onReset = false)
		{
			// STAR
			bool childrenUpdated = false;

			if (transform.childCount != childCount)
			{
				childCount = transform.childCount;

				childrenUpdated = true;

				childPos = new Vector3[transform.childCount];
				for (int i = 0; i < childPos.Length; i++)
				{
					childPos[i] = transform.GetChild(i).transform.position;
				}
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).position != childPos[i])
				{
					childrenUpdated = true;
				}
				childPos[i] = transform.GetChild(i).position;
			}

			if (!childrenUpdated && !onReset)
			{
				return;
			}
			// STAR end

			Transform[] children = new Transform[transform.childCount];
			Vector3[] points = new Vector3[children.Length];
			Vector3[] ups = new Vector3[children.Length];

			for (int i = 0; i < transform.childCount; i++)
			{
				children[i] = transform.GetChild(i);
				children[i].gameObject.name = "point " + i;

				points[i] = children[i].localPosition;

				// STAR: multiply positions with local scale to enable scaling of particles with hierarchy mode
				points[i] = new Vector3(points[i].x * Mathf.Abs(children[i].localScale.x), points[i].y * Mathf.Abs(children[i].localScale.y), points[i].z * Mathf.Abs(children[i].localScale.z));

				ups[i] = transform.InverseTransformDirection(children[i].up);
			}

			if (transform.childCount > 1)
			{
				_path.SetPoints(points, ups, isCircuit);
				_path.PrecalculatePathSplines();
				CreateGizmoPoints();
			}
		}

		public Path_Point GetPathPoint(float dist)
		{
			return _path.GetPathPoint(dist, isSmooth);
		}

		#region Gizmo

		private void OnDrawGizmos()
		{
			if (drawGizmos)
			{
				DrawGizmos(false);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (drawGizmos)
			{
				DrawGizmos(true);
			}
		}

		private void DrawGizmos(bool selected)
		{
			Update_Path();

			if (transform.childCount < 1 || gizmoPoints == null)
			{
				return;
			}

			// STAR
			for (int i = 0; i < gizmoPoints.Length - 1; i++)
			{
				var trafoPoint = transform.TransformPoint(gizmoPoints[i + 1].point);

				// Forward = Spline path
				Gizmos.color = selected ? new Color(0, 1, 1, 1) : new Color(0, 1, 1, 0.5f);
				Gizmos.DrawLine(transform.TransformPoint(gizmoPoints[i].point), trafoPoint);

				if (drawUpRightLines)
				{

					// Right
					Gizmos.color = selected ? Color.green : new Color(0, 1, 0, 0.5f);
					Gizmos.DrawLine(trafoPoint, trafoPoint + transform.TransformDirection(gizmoPoints[i + 1].up * gizmoLineSize));

					// Up
					Gizmos.color = selected ? Color.red : new Color(1, 0, 0, 0.5f);
					Gizmos.DrawLine(trafoPoint, trafoPoint + transform.TransformDirection(gizmoPoints[i + 1].right * gizmoLineSize));
				}
			}

			//Path_Point prev = GetPathPoint(0.0f);

			//float dist = -gizmoLineSize;
			//do
			//{
			//	dist = Mathf.Clamp(dist + gizmoLineSize, 0, _path.TotalDistance);

			//	Path_Point next = GetPathPoint(dist);

			//	// Forward = Spline path
			//	Gizmos.color = selected ? new Color(0, 1, 1, 1) : new Color(0, 1, 1, 0.5f);
			//	Gizmos.DrawLine(transform.TransformPoint(prev.point), transform.TransformPoint(next.point));

			//	// Right
			//	Gizmos.color = selected ? Color.green : new Color(0, 1, 0, 0.5f);
			//	Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.up * gizmoLineSize));

			//	// Up
			//	Gizmos.color = selected ? Color.red : new Color(1, 0, 0, 0.5f);
			//	Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.right * gizmoLineSize));

			//	prev = next;

			//} while (dist < _path.TotalDistance);
		}

		private void CreateGizmoPoints()
		{
			gizmoPoints = new Path_Point[Mathf.FloorToInt(_path.TotalDistance / gizmoLineSize) + 1];
			for (int i = 0; i < gizmoPoints.Length - 1; i++)
			{
				float dist = i * gizmoLineSize;
				gizmoPoints[i] = GetPathPoint(dist);
			}
			gizmoPoints[gizmoPoints.Length - 1] = GetPathPoint(_path.TotalDistance);
		}
		#endregion
	}

	public struct Path_Point
	{
		public Vector3 point;
		public Vector3 forward;
		public Vector3 up;
		public Vector3 right;

		public Path_Point(Vector3 point, Vector3 forward, Vector3 up, Vector3 right)
		{
			this.point = point;
			this.forward = forward;
			this.up = up;
			this.right = right;
		}
	}

	public class Path
	{
		public float TotalDistance;

		private Vector3[] _points;
		private Vector3[] _upDirections;
		private float[] _distances;

		private bool _isCircuit = false;
		private int _numPoints;

		// STAR: Store splines in List to avoid several 10k of multiplications and additions per frame!! (kills Hololens)
		private List<Vector3[]> catmullRomFactors = new List<Vector3[]>();

		// repeatedly used values
		private Path_Point _pathPoint = new Path_Point();
		private float _interpolation = 0.0f;
		private int[] _four_indices = new int[] { 0, 1, 2, 3 };
		private Vector3[] _four_points = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

		public void SetPoints(Vector3[] points, Vector3[] ups, bool isCircuit)
		{
			_isCircuit = isCircuit;
			_numPoints = points.Length;

			_points = points;
			_upDirections = ups;

			TotalDistance = 0.0f;
			_distances = new float[_isCircuit ? _numPoints + 1 : _numPoints];
			for (int i = 0; i < _numPoints - 1; ++i)
			{
				_distances[i] = TotalDistance;
				TotalDistance += Vector3.Distance(
					_points[i],
					_points[i + 1]);
			}

			// oneMore
			if (_isCircuit)
			{
				_distances[_numPoints - 1] = TotalDistance;
				TotalDistance += Vector3.Distance(
					_points[_numPoints - 1],
					_points[0]);
			}

			_upDirections[_numPoints - 1] = ups[_numPoints - 1];
			_distances[_distances.Length - 1] = TotalDistance;
		}

		/// <summary>
		/// STAR: Calculate splines once!
		/// </summary>
		/// <param name="index"></param>
		public void PrecalculatePathSplines()
		{
			for (int i = 0; i < _distances.Length; i++)
			{
				var index = i + 1;  // index starts at 1

				index = index % _numPoints;

				if (_isCircuit)
				{
					_four_indices[0] = ((index - 2) + _numPoints) % _numPoints;
					_four_indices[1] = ((index - 1) + _numPoints) % _numPoints;
					_four_indices[2] = index % _numPoints;
					_four_indices[3] = (index + 1) % _numPoints;
				}
				else
				{
					_four_indices[0] = Mathf.Clamp(index - 2, 0, _numPoints - 1);
					_four_indices[1] = ((index - 1) + _numPoints) % _numPoints;
					_four_indices[2] = index % _numPoints;
					_four_indices[3] = Mathf.Clamp(index + 1, 0, _numPoints - 1);
				}

				// assign the four points with the segment in the middle
				_four_points[0] = _points[_four_indices[0]];
				_four_points[1] = _points[_four_indices[1]];
				_four_points[2] = _points[_four_indices[2]];
				_four_points[3] = _points[_four_indices[3]];

				// STAR: calculate once, use precalculated factors
				// you need two points to get a forward direction
				if (catmullRomFactors.Count <= i)       // index starts at 1
				{
					catmullRomFactors.Add(Math_Functions.CalculateCatmullRomFactors(
						_four_points[0],
						_four_points[1],
						_four_points[2],
						_four_points[3]));
				}
				else
				{
					catmullRomFactors[i] = (Math_Functions.CalculateCatmullRomFactors(
						_four_points[0],
						_four_points[1],
						_four_points[2],
						_four_points[3]));
				}
			}
		}

		// STAR end

		public Path_Point GetPathPoint(float dist, bool isSmooth)
		{
			if (_isCircuit)
			{
				dist = (dist + TotalDistance) % TotalDistance;
			}
			else
			{
				dist = Mathf.Clamp(dist, 0.0f, TotalDistance);
			}

			// find segment index
			int index = 1;
			while (_distances[index] < dist)
			{
				index++;
			}

			// the segment in the middle
			_interpolation = Mathf.InverseLerp(
				_distances[index - 1],
				_distances[index],
				dist);

			//index = index % _numPoints;	// don't! (with new precalculations) - is done on calculating already, and will crash when isCircuit = true as index will be 0

			//if (_isCircuit)
			//{
			//	_four_indices[0] = ((index - 2) + _numPoints) % _numPoints;
			//	_four_indices[1] = ((index - 1) + _numPoints) % _numPoints;
			//	_four_indices[2] = index % _numPoints;
			//	_four_indices[3] = (index + 1) % _numPoints;
			//}
			//else
			//{
			//	_four_indices[0] = Mathf.Clamp(index - 2, 0, _numPoints - 1);
			//	_four_indices[1] = ((index - 1) + _numPoints) % _numPoints;
			//	_four_indices[2] = index % _numPoints;
			//	_four_indices[3] = Mathf.Clamp(index + 1, 0, _numPoints - 1);
			//}

			if (isSmooth)
			{
				// assign the four points with the segment in the middle
				_four_points[0] = _points[_four_indices[0]];
				_four_points[1] = _points[_four_indices[1]];
				_four_points[2] = _points[_four_indices[2]];
				_four_points[3] = _points[_four_indices[3]];

				//// you need two points to get a forward direction
				//_pathPoint.point = Math_Functions.CatmullRom(
				//	_four_points[0],
				//	_four_points[1],
				//	_four_points[2],
				//	_four_points[3],
				//	_interpolation);
				//_pathPoint.forward = Math_Functions.CatmullRom(
				//	_four_points[0],
				//	_four_points[1],
				//	_four_points[2],
				//	_four_points[3],
				//	_interpolation + 0.01f) - _pathPoint.point;

				// STAR
				_pathPoint.point = Math_Functions.GetCatmullRomPointFromFactors(catmullRomFactors[index - 1], _interpolation);
				_pathPoint.forward = Math_Functions.GetCatmullRomPointFromFactors(catmullRomFactors[index - 1], _interpolation + 0.01f) - _pathPoint.point;
				// STAR end

				_pathPoint.forward.Normalize();
			}
			else // strait shooting
			{
				_pathPoint.point = Vector3.Lerp(
					_points[_four_indices[index - 1]],
					_points[_four_indices[index]],
					_interpolation);

				_pathPoint.forward = _points[_four_indices[index]] - _points[_four_indices[index - 1]];
				_pathPoint.forward.Normalize();
			}

			// 90 degree turn to right
			_pathPoint.right = Vector3.Cross(
				Vector3.Lerp(
					_upDirections[_four_indices[1]],
					_upDirections[_four_indices[2]],
					_interpolation), // lerp
				_pathPoint.forward).normalized; // cross

			// 90 degree turn to up
			_pathPoint.up = Vector3.Cross(_pathPoint.forward, _pathPoint.right).normalized;

			// now all directions are 90 degrees from each other

			return _pathPoint;
		}
	}
}