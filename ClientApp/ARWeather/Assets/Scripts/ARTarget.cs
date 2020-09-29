using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARTarget : MonoBehaviour
{
    public float AnimationSpeed = 1.0f;
    public float MovementSpeed = 0.5f;

    private ARRaycastManager arOriginRaycaster;

    private Vector3 targetPosition = new Vector3();
    private bool holdPosition = false;

    private Vector3 firstTouchPoint;
    private Vector3 lastTouchPoint;
    private bool isLongSwipe = false;

    void Start()
    {
        arOriginRaycaster = FindObjectOfType<ARRaycastManager>();

        targetPosition = transform.position;
    }

    void Update()
    {
        // Raycast to the center of the screen to obtain a location to place the AR visualization at
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arOriginRaycaster.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.All);

        if (hits.Count > 0 && !holdPosition)
        {
            targetPosition = hits[0].pose.position;
        }

        // Lerp between the current, and new position smoothly
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (transform.position - targetPosition).magnitude * MovementSpeed);

        HandleTouchInput();
    }

    // Detect some basic 1 finger touch input for scaling, rotating and locking the AR visualization
    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0); 
            if (touch.phase == TouchPhase.Began)
            {
                firstTouchPoint = touch.position;
                lastTouchPoint = touch.position;
                isLongSwipe = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 currentTouchPoint = touch.position;

                // Check if it is a swipe
                float dragDistance = Screen.height * 15 / 100;
                if (Mathf.Abs(currentTouchPoint.x - firstTouchPoint.x) > dragDistance || Mathf.Abs(currentTouchPoint.y - firstTouchPoint.y) > dragDistance)
                {
                    isLongSwipe = true;
                }

                if (isLongSwipe)
                {
                    if (Mathf.Abs(lastTouchPoint.x - currentTouchPoint.x) > Mathf.Abs(lastTouchPoint.y - currentTouchPoint.y))
                    {
                        // Rotate the carousel
                        float delta = (lastTouchPoint.x - currentTouchPoint.x) / Screen.width * 90.0f;
                        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), delta);
                    }
                    else
                    {
                        // Scale the carousel
                        float delta = (lastTouchPoint.y - currentTouchPoint.y) / Screen.height;
                        transform.localScale = new Vector3(transform.localScale.x - delta, transform.localScale.y - delta, transform.localScale.z - delta);
                        if (transform.localScale.x < 0.01f)
                        {
                            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        }
                    }
                }

                lastTouchPoint = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!isLongSwipe) // It was just a tap
                {
                    holdPosition = !holdPosition;
                }
            }
        }
    }
}
