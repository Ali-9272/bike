using UnityEngine;

public class MotorcycleController : MonoBehaviour
{
    [Header("Motorcycle Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float brakeForce = 15f;
    public float wheelieForce = 500f;
    public float balanceForce = 300f;
    public float maxWheelieAngle = 45f;
    
    [Header("Physics Components")]
    public Rigidbody motorcycleRigidbody;
    public Transform frontWheel;
    public Transform rearWheel;
    public Transform motorcycleBody;
    
    [Header("Input Settings")]
    public KeyCode accelerateKey = KeyCode.W;
    public KeyCode brakeKey = KeyCode.S;
    public KeyCode wheelieKey = KeyCode.Space;
    
    [Header("Game State")]
    public bool isGrounded = true;
    public bool isWheelieActive = false;
    public float currentSpeed = 0f;
    public float wheelieAngle = 0f;
    
    private GameManager gameManager;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool gameStarted = false;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        if (motorcycleRigidbody == null)
            motorcycleRigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        if (!gameStarted || !gameManager.isGameActive)
            return;
            
        HandleInput();
        UpdateWheeliePhysics();
        CheckGameOver();
    }
    
    private void FixedUpdate()
    {
        if (!gameStarted || !gameManager.isGameActive)
            return;
            
        ApplyMovement();
        ApplyWheelieForces();
    }
    
    public void StartGame()
    {
        gameStarted = true;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        motorcycleRigidbody.velocity = Vector3.zero;
        motorcycleRigidbody.angularVelocity = Vector3.zero;
        currentSpeed = 0f;
        wheelieAngle = 0f;
        isWheelieActive = false;
    }
    
    private void HandleInput()
    {
        // Acceleration
        if (Input.GetKey(accelerateKey) || Input.GetMouseButton(0))
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        }
        
        // Braking
        if (Input.GetKey(brakeKey))
        {
            currentSpeed = Mathf.Max(currentSpeed - brakeForce * Time.deltaTime, 0f);
        }
        
        // Wheelie control
        if (Input.GetKey(wheelieKey) || Input.GetMouseButton(1))
        {
            isWheelieActive = true;
        }
        else
        {
            isWheelieActive = false;
        }
        
        // Mobile touch controls
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
            {
                Vector2 touchPos = Camera.main.ScreenToViewportPoint(touch.position);
                
                if (touchPos.x < 0.5f) // Left side - accelerate
                {
                    currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
                }
                else // Right side - wheelie
                {
                    isWheelieActive = true;
                }
            }
        }
    }
    
    private void ApplyMovement()
    {
        // Forward movement
        Vector3 forwardMovement = transform.forward * currentSpeed;
        motorcycleRigidbody.velocity = new Vector3(forwardMovement.x, motorcycleRigidbody.velocity.y, forwardMovement.z);
    }
    
    private void ApplyWheelieForces()
    {
        if (isWheelieActive && isGrounded)
        {
            // Apply wheelie force
            wheelieAngle = Mathf.Min(wheelieAngle + wheelieForce * Time.deltaTime, maxWheelieAngle);
            motorcycleRigidbody.AddTorque(transform.right * wheelieForce);
        }
        else
        {
            // Return to normal position
            wheelieAngle = Mathf.Max(wheelieAngle - balanceForce * Time.deltaTime, 0f);
            if (wheelieAngle > 5f)
            {
                motorcycleRigidbody.AddTorque(-transform.right * balanceForce);
            }
        }
    }
    
    private void UpdateWheeliePhysics()
    {
        // Update motorcycle body rotation for visual wheelie effect
        if (motorcycleBody != null)
        {
            float targetAngle = isWheelieActive ? wheelieAngle : 0f;
            Vector3 currentRotation = motorcycleBody.localEulerAngles;
            currentRotation.x = Mathf.LerpAngle(currentRotation.x, targetAngle, Time.deltaTime * 5f);
            motorcycleBody.localEulerAngles = currentRotation;
        }
        
        // Rotate wheels based on speed
        if (frontWheel != null && rearWheel != null)
        {
            float wheelRotation = currentSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.5f); // Assuming wheel radius of 0.5
            frontWheel.Rotate(wheelRotation, 0, 0);
            rearWheel.Rotate(wheelRotation, 0, 0);
        }
    }
    
    private void CheckGameOver()
    {
        // Check if motorcycle has fallen over
        if (transform.eulerAngles.z > 90f && transform.eulerAngles.z < 270f)
        {
            gameManager.GameOver();
        }
        
        // Check if motorcycle has fallen backwards too much
        if (wheelieAngle > maxWheelieAngle + 10f)
        {
            gameManager.GameOver();
        }
        
        // Check if motorcycle has fallen off the track (simple Y position check)
        if (transform.position.y < -5f)
        {
            gameManager.GameOver();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        
        if (other.CompareTag("Obstacle"))
        {
            gameManager.GameOver();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    
    public float GetWheelieScore()
    {
        return isWheelieActive && isGrounded ? wheelieAngle / maxWheelieAngle : 0f;
    }
}