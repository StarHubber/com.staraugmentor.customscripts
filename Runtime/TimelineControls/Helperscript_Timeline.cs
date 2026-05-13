using STAR.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helperscript_Timeline : MonoBehaviour
{
    public GameEvent reset;

    void Start()
    {
        reset.RaiseBool(true);
        //timelineStop.time = 0;
        //timelineStop.Evaluate();
        //timelineStop.Stop();
    }
}
