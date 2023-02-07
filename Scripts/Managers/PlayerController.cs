using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerController : MonoBehaviour
{
    [Header("Control Settings")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask tapLayer;
    [SerializeField] private Transform minBound, maxBound;
    [SerializeField] private float moveSpeed;

    private Finger movementFinger;
    private Camera cam;
    private WheelMovementHandler selectedWheel;
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
                selectedWheel = hit.transform.gameObject.GetComponentInParent<WheelMovementHandler>();
                selectedWheel.OnMove();
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, tapLayer))
            {
                SpawnerManager.Instance.TriggerAllSpawners();
            }
        }
    }

    void HandleFingerUp(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            movementFinger = null;

            if (selectedWheel != null)
            {
                selectedWheel.OnPlace();
                selectedWheel = null;
            }
        }
    }

    void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger && selectedWheel != null)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 touchPos = movementFinger.currentTouch.screenPosition;

        //Define Z value according to cam pos.z
        Vector3 targetPos = cam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 5f));

        //Return if input comes outside of screen bounds
        if (targetPos.y < minBound.position.y || targetPos.y > maxBound.position.y ||
            targetPos.x < minBound.position.x || targetPos.x > maxBound.position.x) return;

        //Lerp item position to input position
        selectedWheel.transform.position = Vector3.Lerp(selectedWheel.transform.position,
            targetPos, moveSpeed * Time.deltaTime);
    }
}