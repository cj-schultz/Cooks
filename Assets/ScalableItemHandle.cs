using UnityEngine;
using VRTK;

public class ScalableItemHandle : VRTK_InteractableObject
{
    [Header("Inherited Members")]    
    public Vector3 scaleDirection;
    public bool negativeDirection;
    public Transform scalableItem;

    private VRTK_ControllerEvents usingController;

    private Vector3 lastPosition;
    private bool usingHandle;

    private bool startedUsingThisFrame;

    private Quaternion origRot;

    new void OnEnable()
    {
        origRot = transform.rotation;
        UpdateTransform();
    }

    public override void StartUsing(GameObject usingObject)
    {        
        if(!usingHandle)
        {
            base.StartUsing(usingObject);
            usingHandle = true;
            usingController = usingObject.GetComponent<VRTK_ControllerEvents>();
            startedUsingThisFrame = true;
        }        
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
        usingHandle = false;
        usingController = null;
    }

    new void Update()
    {
        if(usingHandle)
        {
            if(startedUsingThisFrame)
            {
                lastPosition = usingController.transform.position;
            }
            else
            {
                Vector3 currentPosition = usingController.transform.position;

                float distanceThisFrame = Vector3.Distance(currentPosition, lastPosition);

                if(Vector3.Distance(currentPosition, scalableItem.position) < Vector3.Distance(lastPosition, scalableItem.position))
                {
                    distanceThisFrame = -distanceThisFrame;
                }

                Vector3 amountToScale = scaleDirection * distanceThisFrame;

                scalableItem.localScale += amountToScale;

                if(!negativeDirection)
                {
                    scalableItem.position += scalableItem.TransformDirection(amountToScale / 2);
                }
                else
                {
                    scalableItem.position -= scalableItem.TransformDirection(amountToScale / 2);
                }

                lastPosition = currentPosition;
            }
        }

        UpdateTransform();

        startedUsingThisFrame = false;
    }

    private void UpdateTransform()
    {
        //transform.rotation = Quaternion.Euler(origRot.eulerAngles + scalableItem.rotation.eulerAngles);
        transform.rotation = scalableItem.rotation;

        transform.position = scalableItem.position + transform.TransformDirection(new Vector3(
            scalableItem.localScale.x * (negativeDirection ? -scaleDirection.x : scaleDirection.x) / 2,
            scalableItem.localScale.y * (negativeDirection ? -scaleDirection.y : scaleDirection.y) / 2,
            scalableItem.localScale.z * (negativeDirection ? -scaleDirection.z : scaleDirection.z) / 2));      
    }
}
