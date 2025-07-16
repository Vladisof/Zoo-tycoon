using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    
    [System.Serializable]
    public class CameraPosition
    {
        public Vector3 positionOffset;
        public Vector3 rotation;
    }
    
    public CameraPosition[] cameraPositions = new CameraPosition[4];
    private int currentPositionIndex = 0;
    
    private Transform currentTarget;
    private Vector3 currentOffset;
    private Quaternion currentRotation;
    
    private bool isTransitioning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float transitionTime = 0f;
    private float transitionDuration = 0.5f;

    void Start()
    {
        currentTarget = target;
        if (cameraPositions.Length > 0)
        {
            currentOffset = cameraPositions[0].positionOffset;
            currentRotation = Quaternion.Euler(cameraPositions[0].rotation);
            transform.rotation = currentRotation;
        }
    }

    void FixedUpdate()
    {
        if (isTransitioning)
        {
            TransitionCamera();
        }
        else
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = currentTarget.position + currentOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void TransitionCamera()
    {
        transitionTime += Time.fixedDeltaTime;
        float progress = Mathf.Clamp01(transitionTime / transitionDuration);

        // Smoothly interpolate position and rotation
        Vector3 targetPosition = currentTarget.position + currentOffset;
        transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
        transform.rotation = Quaternion.Slerp(startRotation, currentRotation, progress);

        if (progress >= 1.0f)
        {
            isTransitioning = false;
        }
    }

    public void SwitchToPosition(int positionIndex)
    {
        if (positionIndex < 0 || positionIndex >= cameraPositions.Length || isTransitioning)
            return;

        isTransitioning = true;
        transitionTime = 0f;
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        currentPositionIndex = positionIndex;
        currentOffset = cameraPositions[positionIndex].positionOffset;
        currentRotation = Quaternion.Euler(cameraPositions[positionIndex].rotation);
    }

    public void SwitchToNextPosition()
    {
        int nextIndex = (currentPositionIndex + 1) % cameraPositions.Length;
        SwitchToPosition(nextIndex);
    }

    public void SwitchToPreviousPosition()
    {
        int prevIndex = (currentPositionIndex - 1 + cameraPositions.Length) % cameraPositions.Length;
        SwitchToPosition(prevIndex);
    }
    
    public void RotateRight()
    {
        SwitchToNextPosition();
    }

    public void RotateLeft()
    {
        SwitchToPreviousPosition();
    }
}