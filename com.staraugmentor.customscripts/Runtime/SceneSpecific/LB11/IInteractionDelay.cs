using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionDelay
{
    void EnableInteraction();
    void DisableInteraction();
    void Notify();
}
