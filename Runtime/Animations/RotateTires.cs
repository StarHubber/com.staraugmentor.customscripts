using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RotateTires : MonoBehaviour
{
    // Start is called before the first frame update
    public bool rotate = false;
    public float speedX = 0.0f;
    public float speedY = 0.0f;
    public float speedZ = 0.0f;

    public List<Transform> tires;

    private Time t;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            foreach (Transform tire in tires)
            {
                tire.Rotate(Time.deltaTime * speedX, Time.deltaTime* speedY, Time.deltaTime * speedZ, Space.Self);
            }
        }
    }
}
