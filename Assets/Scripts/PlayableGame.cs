using UnityEngine;
using UnityEngine.UI;

public class PlayableGame : MonoBehaviour
{
    // Game Objects
    private GameObject motorcycle;
    private GameObject road;
    private Canvas gameCanvas;
    private SimpleMotorcycle bikeController;
    
    // UI Elements
    private Text scoreText;
    private Text speedText;
    private Text instructionsText;
    private Button playButton;
    private GameObject mainMenu;
    private GameObject gameplayUI;
    
    // Game State
    private bool gameActive = false;
    private int score = 0;
    private int highScore = 0;
    private float gameSpeed = 0f;
    
    void Start()
    {
        CreateCompleteGame();
        LoadHighScore();
        ShowMainMenu();
    }
    
    void Update()
    {
        if (gameActive)
        {
            UpdateScore();
            UpdateUI();
            CheckGameState();
        }
        
        // Quick start for testing
        if (Input.GetKeyDown(KeyCode.Return) && !gameActive)
        {
            StartGame();
        }
    }
    
    void CreateCompleteGame()
    {
        CreateMotorcycle();
        CreateRoad();
        CreateUI();
        SetupCamera();
    }
    
    void CreateMotorcycle()
    {
        // Create motorcycle with all components
        motorcycle = new GameObject("Motorcycle");
        motorcycle.transform.position = new Vector3(0, 1, 0);
        motorcycle.tag = "Motorcycle";
        
        // Physics
        Rigidbody rb = motorcycle.AddComponent<Rigidbody>();
        rb.mass = 150f;
        rb.drag = 0.3f;
        rb.angularDrag = 3f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        
        // Controller
        bikeController = motorcycle.AddComponent<SimpleMotorcycle>();
        
        // Visual components
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(motorcycle.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.5f, 0.3f, 1.5f);
        body.GetComponent<Renderer>().material.color = Color.red;
        
        GameObject frontWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frontWheel.name = "FrontWheel";
        frontWheel.transform.SetParent(motorcycle.transform);
        frontWheel.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
        frontWheel.transform.localScale = new Vector3(0.66f, 0.1f, 0.66f);
        frontWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        frontWheel.GetComponent<Renderer>().material.color = Color.black;
        
        GameObject rearWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rearWheel.name = "RearWheel";
        rearWheel.transform.SetParent(motorcycle.transform);
        rearWheel.transform.localPosition = new Vector3(0, -0.3f, -0.6f);
        rearWheel.transform.localScale = new Vector3(0.66f, 0.1f, 0.66f);
        rearWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        rearWheel.GetComponent<Renderer>().material.color = Color.black;
        
        GameObject rider = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rider.name = "Rider";
        rider.transform.SetParent(body.transform);
        rider.transform.localPosition = new Vector3(0, 0.8f, 0);
        rider.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
        rider.GetComponent<Renderer>().material.color = Color.blue;
        
        // Assign references
        bikeController.body = body.transform;
        bikeController.frontWheel = frontWheel.transform;
        bikeController.rearWheel = rearWheel.transform;
        
        // Collider
        BoxCollider mainCollider = motorcycle.AddComponent<BoxCollider>();
        mainCollider.size = new Vector3(1f, 1f, 2f);
    }
    
    void CreateRoad()
    {
        // Main road
        road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.name = "Road";
        road.transform.position = new Vector3(0, 0, 0);
        road.transform.localScale = new Vector3(50, 1, 50);
        road.tag = "Road";
        road.GetComponent<Renderer>().material.color = Color.gray;
        
        // Road markings
        for (int i = -100; i < 500; i += 10)
        {
            GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marking.name = "RoadMarking";
            marking.transform.position = new Vector3(0, 0.01f, i);
            marking.transform.localScale = new Vector3(0.3f, 0.01f, 3f);
            marking.GetComponent<Renderer>().material.color = Color.white;
            marking.transform.SetParent(road.transform);
        }
        
        // Create some obstacles
        CreateObstacles();
    }
    
    void CreateObstacles()
    {
        for (int i = 20; i < 500; i += 50)
        {
            if (Random.value < 0.5f) // 50% chance to spawn obstacle
            {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = "Obstacle";
                obstacle.transform.position = new Vector3(Random.Range(-2f, 2f), 0.5f, i);
                obstacle.transform.localScale = new Vector3(1f, 1f, 1f);
                obstacle.tag = "Obstacle";
                obstacle.GetComponent<Renderer>().material.color = Color.yellow;
                
                // Add collider trigger
                obstacle.GetComponent<Collider>().isTrigger = true;
            }
        }
    }
    
    void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("GameCanvas");
        gameCanvas = canvasGO.AddComponent<Canvas>();
        gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Main Menu
        CreateMainMenu();
        
        // Gameplay UI
        CreateGameplayUI();
    }
    
    void CreateMainMenu()
    {
        mainMenu = new GameObject("MainMenu");
        mainMenu.transform.SetParent(gameCanvas.transform, false);
        
        Image menuBG = mainMenu.AddComponent<Image>();
        menuBG.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        RectTransform menuRect = mainMenu.GetComponent<RectTransform>();
        menuRect.anchorMin = Vector2.zero;
        menuRect.anchorMax = Vector2.one;
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;
        
        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(mainMenu.transform, false);
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Wheelie Master:\nMoto Ride 3D";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.85f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(400, 150);
        
        // Studio
        GameObject studioGO = new GameObject("Studio");
        studioGO.transform.SetParent(mainMenu.transform, false);
        Text studioText = studioGO.AddComponent<Text>();
        studioText.text = "Wayfu Studio";
        studioText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        studioText.fontSize = 24;
        studioText.alignment = TextAnchor.MiddleCenter;
        studioText.color = new Color(0.7f, 0.8f, 1f);
        
        RectTransform studioRect = studioGO.GetComponent<RectTransform>();
        studioRect.anchorMin = new Vector2(0.5f, 0.6f);
        studioRect.anchorMax = new Vector2(0.5f, 0.65f);
        studioRect.anchoredPosition = Vector2.zero;
        studioRect.sizeDelta = new Vector2(300, 50);
        
        // Description
        GameObject descGO = new GameObject("Description");
        descGO.transform.SetParent(mainMenu.transform, false);
        Text descText = descGO.AddComponent<Text>();
        descText.text = "Contains ads • In-app purchases\n\nMaster the art of motorcycle wheelies!\n\nControls:\nW or Left Click - Accelerate\nSpace or Right Click - Wheelie";
        descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.fontSize = 16;
        descText.alignment = TextAnchor.MiddleCenter;
        descText.color = Color.gray;
        
        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.35f);
        descRect.anchorMax = new Vector2(0.5f, 0.55f);
        descRect.anchoredPosition = Vector2.zero;
        descRect.sizeDelta = new Vector2(400, 200);
        
        // Play Button
        GameObject playButtonGO = new GameObject("PlayButton");
        playButtonGO.transform.SetParent(mainMenu.transform, false);
        
        Image buttonImage = playButtonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.7f, 0.2f);
        
        playButton = playButtonGO.AddComponent<Button>();
        playButton.onClick.AddListener(StartGame);
        
        RectTransform buttonRect = playButtonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.2f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.28f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(200, 60);
        
        // Button Text
        GameObject buttonTextGO = new GameObject("ButtonText");
        buttonTextGO.transform.SetParent(playButtonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "PLAY";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 24;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        // High Score
        GameObject highScoreGO = new GameObject("HighScore");
        highScoreGO.transform.SetParent(mainMenu.transform, false);
        Text highScoreTextComp = highScoreGO.AddComponent<Text>();
        highScoreTextComp.text = "High Score: " + highScore;
        highScoreTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        highScoreTextComp.fontSize = 20;
        highScoreTextComp.alignment = TextAnchor.MiddleCenter;
        highScoreTextComp.color = Color.yellow;
        
        RectTransform highScoreRect = highScoreGO.GetComponent<RectTransform>();
        highScoreRect.anchorMin = new Vector2(0.5f, 0.1f);
        highScoreRect.anchorMax = new Vector2(0.5f, 0.15f);
        highScoreRect.anchoredPosition = Vector2.zero;
        highScoreRect.sizeDelta = new Vector2(300, 50);
    }
    
    void CreateGameplayUI()
    {
        gameplayUI = new GameObject("GameplayUI");
        gameplayUI.transform.SetParent(gameCanvas.transform, false);
        gameplayUI.SetActive(false);
        
        // Score
        GameObject scoreGO = new GameObject("Score");
        scoreGO.transform.SetParent(gameplayUI.transform, false);
        scoreText = scoreGO.AddComponent<Text>();
        scoreText.text = "Score: 0";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 24;
        scoreText.alignment = TextAnchor.MiddleLeft;
        scoreText.color = Color.white;
        
        RectTransform scoreRect = scoreGO.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0.9f);
        scoreRect.anchorMax = new Vector2(0.5f, 1f);
        scoreRect.offsetMin = new Vector2(20, -50);
        scoreRect.offsetMax = new Vector2(-20, -20);
        
        // Speed
        GameObject speedGO = new GameObject("Speed");
        speedGO.transform.SetParent(gameplayUI.transform, false);
        speedText = speedGO.AddComponent<Text>();
        speedText.text = "Speed: 0 km/h";
        speedText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        speedText.fontSize = 20;
        speedText.alignment = TextAnchor.MiddleRight;
        speedText.color = Color.cyan;
        
        RectTransform speedRect = speedGO.GetComponent<RectTransform>();
        speedRect.anchorMin = new Vector2(0.5f, 0.9f);
        speedRect.anchorMax = new Vector2(1f, 1f);
        speedRect.offsetMin = new Vector2(20, -50);
        speedRect.offsetMax = new Vector2(-20, -20);
        
        // Instructions
        GameObject instructGO = new GameObject("Instructions");
        instructGO.transform.SetParent(gameplayUI.transform, false);
        instructionsText = instructGO.AddComponent<Text>();
        instructionsText.text = "W/Click: Accelerate  |  Space/Right-Click: Wheelie";
        instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionsText.fontSize = 16;
        instructionsText.alignment = TextAnchor.MiddleCenter;
        instructionsText.color = Color.gray;
        
        RectTransform instructRect = instructGO.GetComponent<RectTransform>();
        instructRect.anchorMin = new Vector2(0, 0);
        instructRect.anchorMax = new Vector2(1f, 0.1f);
        instructRect.offsetMin = new Vector2(20, 20);
        instructRect.offsetMax = new Vector2(-20, -20);
    }
    
    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && motorcycle != null)
        {
            CameraController camController = mainCamera.GetComponent<CameraController>();
            if (camController == null)
            {
                camController = mainCamera.gameObject.AddComponent<CameraController>();
            }
            
            camController.target = motorcycle.transform;
            camController.offset = new Vector3(0, 4, -10);
            camController.followSpeed = 3f;
            camController.rotationSpeed = 2f;
        }
    }
    
    public void StartGame()
    {
        gameActive = true;
        score = 0;
        gameSpeed = 0f;
        
        mainMenu.SetActive(false);
        gameplayUI.SetActive(true);
        
        if (bikeController != null)
        {
            bikeController.StartGame();
        }
        
        // Move camera to follow motorcycle
        Camera.main.transform.position = new Vector3(0, 4, -10);
    }
    
    void UpdateScore()
    {
        if (bikeController != null)
        {
            gameSpeed = bikeController.GetComponent<Rigidbody>().velocity.magnitude;
            score += Mathf.RoundToInt(gameSpeed * Time.deltaTime * 10f);
            
            // Bonus for wheelie
            if (bikeController.IsWheelieActive())
            {
                score += Mathf.RoundToInt(20f * Time.deltaTime);
            }
        }
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
            
        if (speedText != null)
            speedText.text = "Speed: " + Mathf.RoundToInt(gameSpeed * 3.6f) + " km/h";
    }
    
    void CheckGameState()
    {
        // Check if motorcycle exists and is still active
        if (motorcycle == null || !motorcycle.activeInHierarchy)
        {
            GameOver();
            return;
        }
        
        // Check if motorcycle fell off the world
        if (motorcycle.transform.position.y < -5f)
        {
            GameOver();
        }
    }
    
    void GameOver()
    {
        gameActive = false;
        
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }
        
        ShowMainMenu();
    }
    
    void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        gameplayUI.SetActive(false);
        
        // Update high score display
        Text highScoreText = mainMenu.transform.Find("HighScore").GetComponent<Text>();
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }
    
    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
}