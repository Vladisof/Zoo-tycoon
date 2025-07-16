using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Processors;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
    [SerializeField] private Vector2 joystickSize = new Vector2(300, 300);
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private NavMeshAgent player;
    [SerializeField] private Camera mainCamera; // Reference to the main camera

    public float currentSpeed;

    private Finger MovementFinger;
    private Vector2 MovementAmount;

    private void Awake()
    {
        // Auto-assign the main camera if not set
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerLose;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    public void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerLose;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        EnhancedTouchSupport.Disable();
    }

    public void OnDeactivate()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        MovementFinger = null;
        joystick.Knob.anchoredPosition = Vector2.zero;
        joystick.gameObject.SetActive(false);
        MovementAmount = Vector2.zero;
    }

    public void OnActivate()
    {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerLose;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void HandleFingerMove(Finger aObj)
    {
        if (aObj == MovementFinger)
        {
            Vector2 knobPosition;
            float maxMovement = joystickSize.x / 2f;
            ETouch.Touch currentTouch = aObj.currentTouch;

            if (Vector2.Distance(currentTouch.screenPosition, joystick.RectTransform.anchoredPosition) > maxMovement)
            {
                knobPosition = (currentTouch.screenPosition -
                               joystick.RectTransform.anchoredPosition).normalized * maxMovement;
            }
            else
            {
                knobPosition = currentTouch.screenPosition - joystick.RectTransform.anchoredPosition;
            }

            joystick.Knob.anchoredPosition = knobPosition;
            MovementAmount = knobPosition / maxMovement;
        }
    }

    private void HandleFingerLose(Finger aObj)
    {
        if(aObj==MovementFinger)
        {
            MovementFinger = null;
            joystick.Knob.anchoredPosition = Vector2.zero;
            joystick.gameObject.SetActive(false);
            MovementAmount = Vector2.zero;
        }
    }

    private void HandleFingerDown(Finger aObj)
    {
        // Check if touch is over UI element
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = aObj.screenPosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
        
            // If touch is over UI, don't activate joystick
            if (results.Count > 0)
                return;
        }
    
        if (MovementFinger == null)
        {
            MovementFinger = aObj;
            MovementAmount = Vector2.zero;
            joystick.gameObject.SetActive(true);
            joystick.RectTransform.sizeDelta = joystickSize;
            joystick.RectTransform.anchoredPosition = ClampStartPosition(aObj.screenPosition);
        }
    }

    private Vector2 ClampStartPosition(Vector2 aStartPosition)
    {
        if (aStartPosition.x < joystickSize.x / 2)
            aStartPosition.x = joystickSize.x / 2;

        if (aStartPosition.y < joystickSize.y / 2)
            aStartPosition.y = joystickSize.y / 2;

        else if (aStartPosition.y > Screen.height - joystickSize.y / 2)
            aStartPosition.y = Screen.height - joystickSize.y / 2;

        return aStartPosition;
    }

    private void Update()
    {
        if (MovementAmount == Vector2.zero)
        {
            // Reset current speed when not moving
            currentSpeed = 0;
            return;
        }

        // Get the camera's forward and right vectors, but ignore Y component
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
    
        cameraForward.y = 0;
        cameraRight.y = 0;
    
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera orientation
        Vector3 movementDirection = cameraRight * MovementAmount.x + cameraForward * MovementAmount.y;
    
        // Scale the movement
        Vector3 scaledMovement = player.speed * Time.deltaTime * movementDirection*1.5f;

        // Make the player face the movement direction
        if (movementDirection != Vector3.zero)
        {
            player.transform.rotation = Quaternion.LookRotation(movementDirection);
        }
    
        // Move the player
        player.Move(scaledMovement);

        currentSpeed = scaledMovement.magnitude / Time.deltaTime;
    }
}