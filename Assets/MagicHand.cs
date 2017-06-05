using System;
using UnityEngine;
using VRTK;

public class MagicHand : MonoBehaviour
{
    public VRTK_ControllerEvents leftHand;
    public VRTK_ControllerEvents rightHand;

    public GameObject magicOrbPrefab;

    public float releaseTolerance = .2f;
    public float shootTolerance = .1f;
    public float shootMultiplier = 1;
    
    private GameObject currentOrb;

    private float timeElapsed;
    private bool canShoot = true;

    private bool checkingForRelease;

    private float currentReleaseDifference;

    private VRTK_InteractTouch leftHandTouch;
    private VRTK_InteractTouch rightHandTouch;    

    void Awake()
    {
        leftHandTouch = leftHand.GetComponent<VRTK_InteractTouch>();
        rightHandTouch = rightHand.GetComponent<VRTK_InteractTouch>();
    }    

    void OnEnable()
    {
        leftHand.TriggerReleased += LeftHandTriggerRelease;
        rightHand.TriggerReleased += RightHandTriggerRelease;
    }
    
    void OnDisable()
    {
        leftHand.TriggerReleased -= LeftHandTriggerRelease;
        rightHand.TriggerReleased -= RightHandTriggerRelease;
    }

    private void LeftHandTriggerRelease(object sender, ControllerInteractionEventArgs e)
    {
        if (currentOrb && !checkingForRelease)
        {
            currentReleaseDifference = 0;
            checkingForRelease = true;
        }
    }

    private void RightHandTriggerRelease(object sender, ControllerInteractionEventArgs e)
    {
        if (currentOrb && !checkingForRelease)
        {
            currentReleaseDifference = 0;
            checkingForRelease = true;
        }        
    }

    void Update()
    {
        if(checkingForRelease)
        {
            currentReleaseDifference += Time.deltaTime;
            
            if(currentReleaseDifference > releaseTolerance)
            {
                checkingForRelease = false;
                canShoot = false;

                currentReleaseDifference = 0;

                Destroy(currentOrb);
                currentOrb = null;
            }
            else if (!rightHand.triggerPressed && !leftHand.triggerPressed)
            {
                canShoot = true;
                checkingForRelease = false;
            }
        }
        else if (canShoot)
        {
            // Try to shoot
            Vector3 leftHandVelocity = VRTK_DeviceFinder.GetControllerVelocity(VRTK_ControllerReference.GetControllerReference(leftHand.gameObject));
            Vector3 rightHandVelocity = VRTK_DeviceFinder.GetControllerVelocity(VRTK_ControllerReference.GetControllerReference(rightHand.gameObject));

            Vector3 averageVelocity = (leftHandVelocity + rightHandVelocity) / 2;

            if (Vector3.Angle(rightHandVelocity, leftHandVelocity) < 90 && averageVelocity.magnitude >= shootTolerance)
            {
                currentOrb.GetComponent<Rigidbody>().isKinematic = false;
                currentOrb.GetComponent<Rigidbody>().velocity = averageVelocity * shootMultiplier;
                Destroy(currentOrb, 5);           
            }
            else
            {
                Destroy(currentOrb);
            }

            currentOrb = null;
            canShoot = false;
        }
        else if(!leftHandTouch.GetTouchedObject() && !rightHandTouch.GetTouchedObject())
        {
            if (leftHand.triggerPressed && rightHand.triggerPressed)
            {
                if (!currentOrb)
                {
                    currentOrb = Instantiate(magicOrbPrefab, Vector3.Lerp(leftHand.transform.position, rightHand.transform.position, 0.5f), Quaternion.identity);
                }
                else
                {
                    currentOrb.transform.position = Vector3.Lerp(leftHand.transform.position, rightHand.transform.position, 0.5f);                    
                }
            }
        }
    }           
}
