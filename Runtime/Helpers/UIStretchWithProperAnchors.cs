using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class UIStretchWithProperAnchors : MonoBehaviour
{
	public bool setAnchors = false;

	private void Awake()
	{
		//SetAnchorsToElementExtents();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (setAnchors)
		{
			SetAnchorsToElementExtents();
			setAnchors = false;
		}
    }

	private void SetAnchorsToElementExtents()
	{
		var prt = transform.parent.GetComponent<RectTransform>();
		var rt = GetComponent<RectTransform>();

		var rectMinWorld = rt.TransformPoint(rt.rect.xMin, rt.rect.yMin, 0);
		var rectMaxWorld = rt.TransformPoint(rt.rect.xMax, rt.rect.yMax, 0);
		//Debug.Log(rectMinWorld);
		//Debug.Log(rectMaxWorld);

		var rectMinInParentLocal = prt.InverseTransformPoint(rectMinWorld);
		var rectMaxInParentLocal = prt.InverseTransformPoint(rectMaxWorld);
		//Debug.Log(rectMinInParentLocal);
		//Debug.Log(rectMaxInParentLocal);

		var rectMinInParentLocalNormalized = new Vector3(rectMinInParentLocal.x / prt.rect.width, rectMinInParentLocal.y / prt.rect.height, 0);
		var rectMaxInParentLocalNormalized = new Vector3(rectMaxInParentLocal.x / prt.rect.width, rectMaxInParentLocal.y / prt.rect.height, 0);
		//Debug.Log(rectMinInParentLocalNormalized);
		//Debug.Log(rectMaxInParentLocalNormalized);

		var rectMinInParentLocalNormalizedPivoted = rectMinInParentLocalNormalized + new Vector3(prt.pivot.x, prt.pivot.y, 0);
		var rectMaxInParentLocalNormalizedPivoted = rectMaxInParentLocalNormalized + new Vector3(prt.pivot.x, prt.pivot.y, 0);

		rt.anchorMin = rectMinInParentLocalNormalizedPivoted;
		rt.anchorMax = rectMaxInParentLocalNormalizedPivoted;

		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}
}
