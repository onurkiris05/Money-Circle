using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class BasePlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] private Transform minBound, maxBound;
    [SerializeField] private float moveSpeed;

    private Finger movementFinger;
    private Camera cam;
    private WheelMovementHandler selectedWheelMovementHandler;
    private RaycastHit hit;
    private Ray ray;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;

        cam = Camera.main;
    }

    void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;

        EnhancedTouchSupport.Disable();
    }

    void HandleFingerDown(Finger touchedFinger)
    {
        if (movementFinger == null)
        {
            movementFinger = touchedFinger;

            ray = cam.ScreenPointToRay(movementFinger.currentTouch.screenPosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                selectedWheelMovementHandler = hit.transform.gameObject.GetComponentInParent<WheelMovementHandler>();
            }
        }
    }

    void HandleFingerUp(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;
            selectedWheelMovementHandler = null;
        }
    }

    void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 touchPos = movementFinger.currentTouch.screenPosition;

        //Define Z value according to cam pos.z
        Vector3 targetPos = cam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 10f));

        //Return if input comes outside of screen bounds
        if (targetPos.y < minBound.position.y || targetPos.y > maxBound.position.y) return;

        //Lerp item position to input position
        selectedWheelMovementHandler.transform.position = Vector3.Lerp(selectedWheelMovementHandler.transform.position, targetPos,
            moveSpeed * Time.deltaTime);
    }
}