using System.Collections;
using UnityEngine;

namespace StarCooperation
{
	public class ResizeWithCameraDistance : MonoBehaviour
	{
		private static float scaleSpeed = 3;

		private Transform[] children;
		private float[] distances;

		private int previousMinIdx;

		private void OnEnable()
		{
			previousMinIdx = -1;     // Init negative to run properly through all children on activation of GameObject
		}

		// Start is called before the first frame update
		private void Start()
		{
			distances = new float[transform.childCount];
			children = new Transform[transform.childCount];
			for (int i = 0; i < children.Length; i++)
			{
				children[i] = transform.GetChild(i);
			}

			for (int i = 0; i < transform.childCount; i++)
			{
				for (int j = 0; j < transform.GetChild(i).childCount; j++)
				{
					transform.GetChild(i).GetChild(j).localScale = Vector3.zero;
				}
			}
		}

		// Update is called once per frame
		private void Update()
		{
			float minDist = 100;
			float maxDist = 0;
			int minIdx = 0;

			for (int i = 0; i < transform.childCount; i++)
			{
				distances[i] = Vector3.Distance(children[i].position, Camera.main.transform.position);
				if (distances[i] < minDist)
				{
					minDist = distances[i];
					minIdx = i;
				}
				if (distances[i] > maxDist)
				{
					maxDist = distances[i];
				}
			}
			//for (int i = 0; i < transform.childCount; i++)
			//{
			//	foreach (Transform subChild in children[i])
			//	{
			//		subChild.localScale = Vector3.one * (maxDist - distances[i]) / (maxDist - minDist);
			//	}
			//}
			if (minIdx != previousMinIdx)
			{
				StopAllCoroutines();
				StartCoroutine(DoScale(minIdx, true));

				if (previousMinIdx >= 0) // Init value is -1
				{
					StartCoroutine(DoScale(previousMinIdx, false));
				}

				previousMinIdx = minIdx;
			}
		}

		private IEnumerator DoScale(int childIdx, bool scaleUp)
		{
			float startScale = scaleUp ? 0 : 1;
			float t = 0;

			Transform child = transform.GetChild(childIdx);

			while (t < 1)
			{
				t += Time.deltaTime * scaleSpeed;
				if (t > 1)
				{
					t = 1;
				}

				for (int j = 0; j < child.childCount; j++)
				{
					var subChild = child.GetChild(j);
					subChild.localScale = Vector3.one * Mathf.Lerp(startScale, 1 - startScale, t);
				}

				yield return null;
			}
		}
	}
}