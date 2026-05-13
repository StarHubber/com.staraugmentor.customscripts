using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDefaultPosition : MonoBehaviour
{
	private Vector3 defaultPosition;

	private void Awake()
	{
		defaultPosition = transform.position;
	}
	
    // Update is called once per frame
    void Update()
    {
		transform.position = defaultPosition;
    }
}
