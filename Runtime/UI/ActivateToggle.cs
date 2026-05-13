using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class ActivateToggle : MonoBehaviour
{
	[Tooltip("Toggle that gets On when clicked on collider.")]
	public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Collider>() == null)
		{
			gameObject.AddComponent<MeshCollider>();
		}
    }

	private void OnMouseDown()
	{
		toggle.isOn = true;
	}
}
