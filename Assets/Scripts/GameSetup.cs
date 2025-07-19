using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoCreateMotorcycle = true;
    public bool autoCreateRoad = true;
    public bool autoCreateUI = true;
    
    [Header("Motorcycle Settings")]
    public Material motorcycleMaterial;
    public Material wheelMaterial;
    
    private GameObject motorcycle;
    private GameObject road;
    private Canvas gameCanvas;
    
    void Start()
    {
        if (autoCreateMotorcycle)
        {
            CreateMotorcycle();
        }
        
        if (autoCreateRoad)
        {
            CreateRoad();
        }
        
        if (autoCreateUI)
        {
            CreateUI();
        }
        
        SetupCamera();
        SetupGameManager();
    }
    
    void CreateMotorcycle()
    {
        // Create main motorcycle object
        motorcycle = new GameObject("Motorcycle");
        motorcycle.transform.position = new Vector3(0, 1, 0);
        motorcycle.tag = "Motorcycle";
        
        // Add Rigidbody
        Rigidbody rb = motorcycle.AddComponent<Rigidbody>();
        rb.mass = 150f;
        rb.drag = 0.3f;
        rb.angularDrag = 3f;
        
        // Add motorcycle script
        SimpleMotorcycle motorcycleScript = motorcycle.AddComponent<SimpleMotorcycle>();
        
        // Create body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(motorcycle.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(0.5f, 0.3f, 1.5f);
        
        if (motorcycleMaterial != null)
        {
            body.GetComponent<Renderer>().material = motorcycleMaterial;
        }
        else
        {
            body.GetComponent<Renderer>().material.color = Color.red;
        }
        
        // Create front wheel
        GameObject frontWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frontWheel.name = "FrontWheel";
        frontWheel.transform.SetParent(motorcycle.transform);
        frontWheel.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
        frontWheel.transform.localScale = new Vector3(0.66f, 0.1f, 0.66f);
        frontWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        
        if (wheelMaterial != null)
        {
            frontWheel.GetComponent<Renderer>().material = wheelMaterial;
        }
        else
        {
            frontWheel.GetComponent<Renderer>().material.color = Color.black;
        }
        
        // Create rear wheel
        GameObject rearWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rearWheel.name = "RearWheel";
        rearWheel.transform.SetParent(motorcycle.transform);
        rearWheel.transform.localPosition = new Vector3(0, -0.3f, -0.6f);
        rearWheel.transform.localScale = new Vector3(0.66f, 0.1f, 0.66f);
        rearWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        
        if (wheelMaterial != null)
        {
            rearWheel.GetComponent<Renderer>().material = wheelMaterial;
        }
        else
        {
            rearWheel.GetComponent<Renderer>().material.color = Color.black;
        }
        
        // Create rider
        GameObject rider = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        rider.name = "Rider";
        rider.transform.SetParent(body.transform);
        rider.transform.localPosition = new Vector3(0, 0.8f, 0);
        rider.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);
        rider.GetComponent<Renderer>().material.color = Color.blue;
        
        // Assign references
        motorcycleScript.body = body.transform;
        motorcycleScript.frontWheel = frontWheel.transform;
        motorcycleScript.rearWheel = rearWheel.transform;
        
        // Add collider to main object
        BoxCollider mainCollider = motorcycle.AddComponent<BoxCollider>();
        mainCollider.size = new Vector3(1f, 1f, 2f);
        mainCollider.center = new Vector3(0, 0, 0);
    }
    
    void CreateRoad()
    {
        // Create ground plane
        road = GameObject.CreatePrimitive(PrimitiveType.Plane);
        road.name = "Road";
        road.transform.position = new Vector3(0, 0, 0);
        road.transform.localScale = new Vector3(50, 1, 50);
        road.tag = "Road";
        
        // Set road material
        road.GetComponent<Renderer>().material.color = Color.gray;
        
        // Create road markings
        for (int i = -10; i < 50; i += 5)
        {
            GameObject marking = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marking.name = "RoadMarking";
            marking.transform.position = new Vector3(0, 0.01f, i);
            marking.transform.localScale = new Vector3(0.2f, 0.01f, 2f);
            marking.GetComponent<Renderer>().material.color = Color.white;
            marking.transform.SetParent(road.transform);
        }
        
        // Add environment controller
        EnvironmentController envController = road.AddComponent<EnvironmentController>();
        
        // Create simple road segment prefab
        GameObject roadSegmentPrefab = GameObject.CreatePrimitive(PrimitiveType.Plane);
        roadSegmentPrefab.name = "RoadSegment";
        roadSegmentPrefab.transform.localScale = new Vector3(5, 1, 5);
        roadSegmentPrefab.tag = "Road";
        roadSegmentPrefab.GetComponent<Renderer>().material.color = Color.gray;
        
        // Save as prefab reference (in real Unity, you'd save this as a prefab)
        envController.roadPrefab = roadSegmentPrefab;
        envController.roadParent = road.transform;
        
        // Create simple obstacles
        CreateObstacles(envController);
    }
    
    void CreateObstacles(EnvironmentController envController)
    {
        // Create obstacle prefabs
        GameObject[] obstacles = new GameObject[3];
        
        // Barrier obstacle
        obstacles[0] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacles[0].name = "Barrier";
        obstacles[0].transform.localScale = new Vector3(3f, 1f, 0.3f);
        obstacles[0].tag = "Obstacle";
        obstacles[0].GetComponent<Renderer>().material.color = Color.yellow;
        
        // Cone obstacle
        obstacles[1] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obstacles[1].name = "Cone";
        obstacles[1].transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        obstacles[1].tag = "Obstacle";
        obstacles[1].GetComponent<Renderer>().material.color = Color.red;
        
        // Box obstacle
        obstacles[2] = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacles[2].name = "Box";
        obstacles[2].transform.localScale = new Vector3(1f, 1f, 1f);
        obstacles[2].tag = "Obstacle";
        obstacles[2].GetComponent<Renderer>().material.color = Color.brown;
        
        envController.obstaclePrefabs = obstacles;
    }
    
    void CreateUI()
    {
        // Create Canvas
        GameObject canvasGO = new GameObject("GameCanvas");
        gameCanvas = canvasGO.AddComponent<Canvas>();
        gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Add UIManager
        UIManager uiManager = canvasGO.AddComponent<UIManager>();
        
        // Create main menu panel
        GameObject mainMenuPanel = new GameObject("MainMenuPanel");
        mainMenuPanel.transform.SetParent(gameCanvas.transform, false);
        UnityEngine.UI.Image menuPanelImage = mainMenuPanel.AddComponent<UnityEngine.UI.Image>();
        menuPanelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform menuRect = mainMenuPanel.GetComponent<RectTransform>();
        menuRect.anchorMin = Vector2.zero;
        menuRect.anchorMax = Vector2.one;
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = Vector2.zero;
        
        // Create game title
        GameObject titleGO = new GameObject("GameTitle");
        titleGO.transform.SetParent(mainMenuPanel.transform, false);
        UnityEngine.UI.Text titleText = titleGO.AddComponent<UnityEngine.UI.Text>();
        titleText.text = "Wheelie Master:\nMoto Ride 3D";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(400, 200);
        
        // Create studio name
        GameObject studioGO = new GameObject("StudioName");
        studioGO.transform.SetParent(mainMenuPanel.transform, false);
        UnityEngine.UI.Text studioText = studioGO.AddComponent<UnityEngine.UI.Text>();
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
        
        // Create description
        GameObject descGO = new GameObject("Description");
        descGO.transform.SetParent(mainMenuPanel.transform, false);
        UnityEngine.UI.Text descText = descGO.AddComponent<UnityEngine.UI.Text>();
        descText.text = "Contains ads • In-app purchases\n\nMaster the art of motorcycle wheelies!";
        descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        descText.fontSize = 16;
        descText.alignment = TextAnchor.MiddleCenter;
        descText.color = Color.gray;
        
        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.5f, 0.45f);
        descRect.anchorMax = new Vector2(0.5f, 0.55f);
        descRect.anchoredPosition = Vector2.zero;
        descRect.sizeDelta = new Vector2(400, 100);
        
        // Create play button
        GameObject playButtonGO = new GameObject("PlayButton");
        playButtonGO.transform.SetParent(mainMenuPanel.transform, false);
        UnityEngine.UI.Image playButtonImage = playButtonGO.AddComponent<UnityEngine.UI.Image>();
        playButtonImage.color = new Color(0.2f, 0.7f, 0.2f);
        
        UnityEngine.UI.Button playButton = playButtonGO.AddComponent<UnityEngine.UI.Button>();
        
        RectTransform playButtonRect = playButtonGO.GetComponent<RectTransform>();
        playButtonRect.anchorMin = new Vector2(0.5f, 0.3f);
        playButtonRect.anchorMax = new Vector2(0.5f, 0.35f);
        playButtonRect.anchoredPosition = Vector2.zero;
        playButtonRect.sizeDelta = new Vector2(200, 60);
        
        // Play button text
        GameObject playTextGO = new GameObject("PlayText");
        playTextGO.transform.SetParent(playButtonGO.transform, false);
        UnityEngine.UI.Text playText = playTextGO.AddComponent<UnityEngine.UI.Text>();
        playText.text = "PLAY";
        playText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        playText.fontSize = 24;
        playText.alignment = TextAnchor.MiddleCenter;
        playText.color = Color.white;
        
        RectTransform playTextRect = playTextGO.GetComponent<RectTransform>();
        playTextRect.anchorMin = Vector2.zero;
        playTextRect.anchorMax = Vector2.one;
        playTextRect.offsetMin = Vector2.zero;
        playTextRect.offsetMax = Vector2.zero;
        
        // Create gameplay panel
        GameObject gameplayPanel = new GameObject("GameplayPanel");
        gameplayPanel.transform.SetParent(gameCanvas.transform, false);
        gameplayPanel.SetActive(false);
        
        // Create game over panel
        GameObject gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(gameCanvas.transform, false);
        gameOverPanel.SetActive(false);
        
        // Assign UI references
        uiManager.mainMenuPanel = mainMenuPanel;
        uiManager.gameplayPanel = gameplayPanel;
        uiManager.gameOverPanel = gameOverPanel;
        uiManager.gameTitle = titleText;
        uiManager.studioName = studioText;
        uiManager.gameDescription = descText;
        uiManager.playButton = playButton;
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
            camController.offset = new Vector3(0, 3, -8);
            camController.followSpeed = 5f;
        }
    }
    
    void SetupGameManager()
    {
        GameObject gameManagerGO = new GameObject("GameManager");
        GameManager gameManager = gameManagerGO.AddComponent<GameManager>();
        
        if (motorcycle != null)
        {
            SimpleMotorcycle motorcycleScript = motorcycle.GetComponent<SimpleMotorcycle>();
            // Link game manager with motorcycle
        }
        
        // Add audio manager
        GameObject audioManagerGO = new GameObject("AudioManager");
        AudioManager audioManager = audioManagerGO.AddComponent<AudioManager>();
        
        // Add audio sources
        audioManager.musicSource = audioManagerGO.AddComponent<AudioSource>();
        audioManager.sfxSource = audioManagerGO.AddComponent<AudioSource>();
        
        audioManager.musicSource.loop = true;
        audioManager.musicSource.volume = 0.7f;
        audioManager.sfxSource.volume = 0.8f;
    }
}