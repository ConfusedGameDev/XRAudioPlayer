using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class XRInputController : MonoBehaviour
{
    [SerializeField] OVRHand leftHand;
    [SerializeField] OVRHand rightHand;
    [SerializeField] OVRSkeleton leftSkeleton, rightSkeleton;
 
    bool leftHandPinch;
    bool rightHandPinch;

    [FoldoutGroup("Right hand Events")]
    public UnityEvent  onRightPinchStarted, onRightPinchEnded;

    [FoldoutGroup("Left hand Events")]
    public UnityEvent onLeftPinchStarted, onLeftPinchEnded;

    Vector3 leftPinchStartPoint, leftPinchEndPoint;
    Vector3 leftPinchDelta;
    Vector3 rightPinchStartPoint, rightPinchEndPoint;
    Vector3 rightPinchDelta;

    private bool hasDirectionDetermined = false;
    private bool isUpdatingX = false;
    [SerializeField]  float directionThreshold;

    public float horizontalSwipeThreshold = 0.2f;
    public float verticalSwipeThreshold = 0.2f;

    [FoldoutGroup("Right hand Events")]
    public UnityEvent onLeftSwipe, onRightSwipe, onUpSwipe, onDownSwipe;

    public float lastLightvalue = 1f;
    [FoldoutGroup("left hand Events")]
    public UnityEvent<float> onLightUpdate;


    public float lastVolume = 0f;
    public XRUIController xRUI;
    public AudioSource audioSource;
    public UnityEvent<float> onVolumeUpdate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastVolume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHand.IsTracked)
        {
            if (!leftHandPinch && leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                onLeftPinchStarted?.Invoke();
                Debug.Log("Left pinch started");
                leftHandPinch = true;
                leftPinchStartPoint = getFingerPosition(leftSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
            }
            else if (leftHandPinch && leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                leftPinchEndPoint = getFingerPosition(leftSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
                leftPinchDelta= leftPinchEndPoint- leftPinchStartPoint;
                Debug.Log("Left pinch  delta "+ leftPinchDelta);
                if (!hasDirectionDetermined)
                {
                    if (Mathf.Abs(leftPinchDelta.y) > Mathf.Abs(leftPinchDelta.x) && Mathf.Abs(leftPinchDelta.y) > directionThreshold)
                    {
                        hasDirectionDetermined = true;
                        isUpdatingX = false;
                    }
                    else if (Mathf.Abs(leftPinchDelta.x) > Mathf.Abs(leftPinchDelta.y) && Mathf.Abs(leftPinchDelta.x) > directionThreshold)
                    {
                        hasDirectionDetermined = true;
                        isUpdatingX = true;
                    }
                }

                if (hasDirectionDetermined)
                {
                    if (isUpdatingX)
                    {
                        updateDeltaX(leftPinchDelta.x);
                    }
                    else
                    {
                        updateDeltaY(leftPinchDelta.y);
                    }
                }
                //Add logic to check for Vertical/ horizontal Controls
            }
            else if(leftHandPinch && !leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                hasDirectionDetermined = false;
                isUpdatingX=false;
                Debug.Log("Left pinch ended");
                leftHandPinch = false;
                onLeftPinchEnded?.Invoke();
                leftPinchDelta = leftPinchEndPoint - leftPinchStartPoint;
                leftPinchEndPoint = getFingerPosition(leftSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);

            }

        }
        if (rightHand.IsTracked)
        {
            if (!rightHandPinch && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                Debug.Log("right pinch started");

                onRightPinchStarted?.Invoke();
                rightHandPinch = true;
                rightPinchStartPoint = getFingerPosition(rightSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
            }
            else if (rightHandPinch && rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {

                rightPinchEndPoint = getFingerPosition(rightSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
                rightPinchDelta = rightPinchEndPoint - rightPinchStartPoint;
                Debug.Log("right pinch delta" + rightPinchDelta);

                //Add logic to check for Vertical/ horizontal Controls
            }
            else if (rightHandPinch && !rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
            {
                rightHandPinch = false;
                onRightPinchEnded?.Invoke();
                rightPinchDelta = rightPinchEndPoint - rightPinchStartPoint;
                if(MathF.Abs( rightPinchDelta.x) > MathF.Abs(rightPinchDelta.y))
                {
                    Debug.Log(" horizontal swipe");
                    if(Mathf.Abs(rightPinchDelta.x)> horizontalSwipeThreshold)
                    {
                        Debug.Log("swipe detected");
                        if (rightPinchEndPoint.x< rightPinchStartPoint.x)
                        {
                            Debug.Log("leftSwipe");
                            onLeftSwipe.Invoke();
                        }
                        else
                        {
                            Debug.Log("rightSwipe");
                            onRightSwipe.Invoke();
                        }
                    }
                }
                else
                {
                    Debug.Log("Vertical swipe");
                    
                    if (Mathf.Abs(rightPinchDelta.y) > verticalSwipeThreshold)
                    {
                        Debug.Log("swipe detected");
                        if (rightPinchEndPoint.y > rightPinchStartPoint.y)
                        {
                            Debug.Log("upSwipe");
                            onUpSwipe.Invoke();
                        }
                        else
                        {
                            Debug.Log("downSwipe");
                            onDownSwipe.Invoke();   
                        }
                    }
                }
                rightPinchEndPoint = getFingerPosition(rightSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
                Debug.Log("right pinch ended");


            }
        }
    }

    private void updateDeltaY(float y)
    {
        lastVolume += y * 0.015f;
        lastVolume = Mathf.Clamp01(lastVolume);
        audioSource.volume = lastVolume;
        onVolumeUpdate.Invoke(lastVolume);
        xRUI.UpdateVolume(lastVolume * 100f);
    }
 
    private void updateDeltaX(float x)
    {
        lastLightvalue -= x*0.021f;

        lastLightvalue = Mathf.Clamp(lastLightvalue, -1f, 0f);
        onLightUpdate.Invoke(lastLightvalue);
        xRUI.UpdateBrightness((lastLightvalue + 1f) * 50f);
    }

    Vector3 getFingerPosition(OVRSkeleton skeleton, OVRSkeleton.BoneId id)
    {
        foreach (var b in skeleton.Bones)
        {
            // If bone is the the hand index tip
            if (b.Id == id)
            {
                // Store its transform and break the loop
                return b.Transform.position;
                
            }
        }
        return Vector3.zero;
    }
    }
