using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class DrawingHand : MonoBehaviour
{
    public VRTK_ControllerEvents controllerEvents;

    public LineRenderer line;

    void Awake()
    {
        line.positionCount = 0;
    }

    void Update()
    {
        if(controllerEvents.triggerPressed)
        {
            if (line.positionCount == 0 || Vector3.Distance(controllerEvents.transform.position, line.GetPosition(line.positionCount - 1)) > .05f)
            {                
                line.positionCount++;                
                line.SetPosition(line.positionCount - 1, controllerEvents.transform.position);
            }            
        }
    }
}
