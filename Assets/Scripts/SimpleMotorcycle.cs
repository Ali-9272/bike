using UnityEngine;

public class SimpleMotorcycle : MonoBehaviour
{
    [Header("Motorcycle Parts")]
    public Transform frontWheel;
    public Transform rearWheel;
    public Transform body;
    
    [Header("Physics Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;
    public float maxSteerAngle = 30f;
    public float wheelieForce = 800f;
    
    [Header("Wheelie Settings")]
    public float maxWheelieAngle = 45f;
    public float wheelieSpeed = 50f;
    public float returnSpeed = 30f;
    
    private Rigidbody rb;
    private float motor;
    private float steering;
    private bool isWheelieActive;
    private float currentWheelieAngle;
    private bool isGrounded = true;
    
    // Game references
    private GameManager gameManager;
    private AudioManager audioManager;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Set center of mass lower for stability
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }
    
    void Update()
    {
        // Get input
        GetInput();
        
        // Apply wheelie rotation
        HandleWheelieRotation();
        
        // Rotate wheels visually
        RotateWheels();
        
        // Check for crashes
        CheckForCrash();
    }
    
    void FixedUpdate()
    {
        // Apply motor force
        Vector3 forwardForce = transform.forward * motor * motorForce;
        rb.AddForce(forwardForce);
        
        // Apply wheelie force
        if (isWheelieActive && isGrounded)
        {
            rb.AddTorque(transform.right * wheelieForce);
        }
        
        // Limit speed
        if (rb.velocity.magnitude > 25f)
        {
            rb.velocity = rb.velocity.normalized * 25f;
        }
    }
    
    void GetInput()
    {
        // Desktop controls
        motor = 0f;
        isWheelieActive = false;
        
        if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
        {
            motor = 1f;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            motor = -0.5f;
        }
        
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1))
        {
            isWheelieActive = true;
        }
        
        // Mobile touch controls
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToViewportPoint(touch.position);
            
            if (touchPos.x < 0.5f) // Left side - accelerate
            {
                motor = 1f;
            }
            else // Right side - wheelie
            {
                isWheelieActive = true;
            }
        }
    }
    
    void HandleWheelieRotation()
    {
        if (isWheelieActive && isGrounded)
        {
            currentWheelieAngle += wheelieSpeed * Time.deltaTime;
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0, maxWheelieAngle);
        }
        else
        {
            currentWheelieAngle -= returnSpeed * Time.deltaTime;
            currentWheelieAngle = Mathf.Clamp(currentWheelieAngle, 0, maxWheelieAngle);
        }
        
        // Apply rotation to body
        if (body != null)
        {
            body.localRotation = Quaternion.Euler(currentWheelieAngle, 0, 0);
        }
    }
    
    void RotateWheels()
    {
        float wheelRotation = rb.velocity.magnitude * Time.deltaTime * 360f / (2f * Mathf.PI * 0.33f);
        
        if (frontWheel != null)
        {
            frontWheel.Rotate(wheelRotation, 0, 0);
        }
        
        if (rearWheel != null)
        {
            rearWheel.Rotate(wheelRotation, 0, 0);
        }
    }
    
    void CheckForCrash()
    {
        // Check if motorcycle has tipped over
        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180f) zRotation -= 360f;
        
        if (Mathf.Abs(zRotation) > 90f || currentWheelieAngle > maxWheelieAngle + 5f)
        {
            if (gameManager != null && gameManager.isGameActive)
            {
                gameManager.GameOver();
                if (audioManager != null)
                {
                    audioManager.PlayCrashSound();
                }
            }
        }
        
        // Check if fallen off track
        if (transform.position.y < -5f)
        {
            if (gameManager != null && gameManager.isGameActive)
            {
                gameManager.GameOver();
            }
        }
    }
    
    public void StartGame()
    {
        // Reset motorcycle state
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        currentWheelieAngle = 0f;
        
        if (body != null)
        {
            body.localRotation = Quaternion.identity;
        }
    }
    
    public float GetWheelieScore()
    {
        return isWheelieActive && isGrounded ? currentWheelieAngle / maxWheelieAngle : 0f;
    }
    
    public bool IsWheelieActive()
    {
        return isWheelieActive && currentWheelieAngle > 5f;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        {
            isGrounded = true;
        }
        
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Road"))
        {
            isGrounded = false;
        }
    }
}