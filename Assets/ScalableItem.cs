using UnityEngine;
using VRTK;

public class ScalableItem : VRTK_InteractableObject
{
    [Header("Inherited Members")]
    public GameObject[] handles;

    bool scalingDisabled = true;
    
    private VRTK_ControllerEvents touchingController;

    new void Awake()
    {        
        for (int i = 0; i < handles.Length; i++)
        {
            handles[i].SetActive(false);
        }

        scalingDisabled = true;
    }

    public override void StartTouching(GameObject touchingObject)
    {
        base.StartTouching(touchingObject);

        if (!touchingController)
        {
            touchingController = touchingObjects[0].GetComponent<VRTK_ControllerEvents>();
            touchingController.TouchpadPressed += HandleTouchpadPress;
        }
    }

    public override void StopTouching(GameObject touchingObject)
    {
        base.StopTouching(touchingObject);

        if(touchingController)
        {
            touchingController.TouchpadPressed -= HandleTouchpadPress;
            touchingController = null;
        }
    }
    
    private void HandleTouchpadPress(object sender, ControllerInteractionEventArgs e)
    {
        if (scalingDisabled)
        {
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i].SetActive(true);
            }

            isGrabbable = false;

            GetComponent<Rigidbody>().isKinematic = true;
            ZeroVelocity();

            scalingDisabled = false;
        }
        else
        {
            for (int i = 0; i < handles.Length; i++)
            {
                handles[i].SetActive(false);
            }

            isGrabbable = true;

            GetComponent<Rigidbody>().isKinematic = false;
            
            scalingDisabled = true;
        }       
    }
}
