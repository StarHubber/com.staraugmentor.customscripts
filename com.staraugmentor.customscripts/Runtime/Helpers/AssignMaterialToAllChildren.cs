using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AssignMaterialToAllChildren : MonoBehaviour
{
	public Material mat;
	public Material keepWhenMaterialFound;
	public bool assignMaterialNow = false;

	// Update is called once per frame
	private void Update()
	{
		if (assignMaterialNow)
		{
			foreach (var meshRend in GetComponentsInChildren<MeshRenderer>(true))
			{
				Material[] newMatArray = new Material[meshRend.materials.Length];
				for (int i = 0; i < meshRend.materials.Length; i++)
				{
					newMatArray[i] = mat;
				}
				List<Material> matList = new List<Material>(meshRend.materials);
				if (keepWhenMaterialFound != null)
				{
					if (matList.Find(m => m.name == keepWhenMaterialFound.name + " (Instance)"))
					{
						continue;
					}
				}

				meshRend.materials = newMatArray; // shared materials due to edit mode
			}
			assignMaterialNow = false;
		}
	}
}
