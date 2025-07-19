using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The motorcycle to follow
    public Vector3 offset = new Vector3(0, 5, -10);
    public Vector3 lookAtOffset = new Vector3(0, 0, 0);
    
    [Header("Camera Movement")]
    public float followSpeed = 5f;
    public float rotationSpeed = 3f;
    public bool smoothFollow = true;
    
    [Header("Camera Shake")]
    public float shakeIntensity = 0f;
    public float shakeDuration = 0f;
    
    private Vector3 originalOffset;
    private Vector3 velocity;
    private Vector3 shakeOffset;
    
    private void Start()
    {
        originalOffset = offset;
        
        // If no target is assigned, try to find the motorcycle
        if (target == null)
        {
            SimpleMotorcycle motorcycle = FindObjectOfType<SimpleMotorcycle>();
            if (motorcycle != null)
                target = motorcycle.transform;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        UpdateCameraShake();
        FollowTarget();
        LookAtTarget();
    }
    
    private void FollowTarget()
    {
        Vector3 desiredPosition = target.position + target.TransformDirection(offset) + shakeOffset;
        
        if (smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / followSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
    
    private void LookAtTarget()
    {
        Vector3 lookAtPosition = target.position + lookAtOffset;
        Vector3 direction = lookAtPosition - transform.position;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateCameraShake()
    {
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeIntensity;
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, 5f * Time.deltaTime);
        }
    }
    
    public void ShakeCamera(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    public void ResetToOriginalOffset()
    {
        offset = originalOffset;
    }
}