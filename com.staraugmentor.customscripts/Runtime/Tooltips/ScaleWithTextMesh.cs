using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScaleWithTextMesh : MonoBehaviour
{
	public TextMeshPro textMesh;
	public MeshRenderer whiteBorderMeshRend;
	public float additionalHeightPerLine;

	private float lastLineCount;
	private float originalBorderWidth = 0;

	private void Start()
	{
		lastLineCount = textMesh.textInfo.lineCount;
		//originalBorderWidth = whiteBorderMeshRend.material.GetFloat("_BorderWidth");
		AdjustPanelHeight();
	}

	// Update is called once per frame
	void Update()
	{
		if (textMesh.textInfo.lineCount != lastLineCount)
		{
			AdjustPanelHeight();
			lastLineCount = textMesh.textInfo.lineCount;
		}
    }

	private void AdjustPanelHeight()
	{
		transform.localScale = new Vector3(1, 1 + (textMesh.textInfo.lineCount - 1) * additionalHeightPerLine, 1);
		whiteBorderMeshRend.material.SetFloat("_BorderWidth", originalBorderWidth / transform.localScale.y);
	}
}
