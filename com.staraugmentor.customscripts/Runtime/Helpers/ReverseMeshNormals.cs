using UnityEngine;

namespace StarCooperation
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	public class ReverseMeshNormals : MonoBehaviour
	{
		public bool reverseNow = false;

		void Start()
		{

		}

		private void Update()
		{
			if (reverseNow)
			{
				ReverseNormals();
				reverseNow = false;
			}
		}

		private void ReverseNormals()
		{
			// Edited from: https://wiki.unity3d.com/index.php/ReverseNormals

			MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
			if (filter != null)
			{
				Mesh mesh = filter.sharedMesh;

				Vector3[] normals = mesh.normals;
				for (int i = 0; i < normals.Length; i++)
				{
					normals[i] = -normals[i];
				}

				mesh.normals = normals;

				for (int m = 0; m < mesh.subMeshCount; m++)
				{
					int[] triangles = mesh.GetTriangles(m);
					for (int i = 0; i < triangles.Length; i += 3)
					{
						int temp = triangles[i + 0];
						triangles[i + 0] = triangles[i + 1];
						triangles[i + 1] = temp;
					}
					mesh.SetTriangles(triangles, m);
				}
			}
		}
	}
}