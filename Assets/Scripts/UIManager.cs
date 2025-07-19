using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    public Text gameTitle;
    public Text studioName;
    public Text gameDescription;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;
    public Image gameIcon;
    
    [Header("Gameplay UI")]
    public GameObject gameplayPanel;
    public Text scoreText;
    public Text speedText;
    public Text wheelieText;
    public Slider wheelieBarSlider;
    public Button pauseButton;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;
    public Text gameOverTitle;
    public Button restartButton;
    public Button mainMenuButtonGameOver;
    
    [Header("Settings UI")]
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public Toggle soundToggle;
    public Button backButton;
    
    [Header("Visual Effects")]
    public ParticleSystem wheelieParticles;
    public GameObject speedLines;
    
    private GameManager gameManager;
    private SimpleMotorcycle motorcycleController;
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        motorcycleController = FindObjectOfType<SimpleMotorcycle>();
        
        SetupMainMenu();
        SetupButtonListeners();
    }
    
    private void Update()
    {
        if (gameManager.isGameActive)
        {
            UpdateGameplayUI();
        }
    }
    
    private void SetupMainMenu()
    {
        // Set up the main menu to look like an app store listing
        if (gameTitle != null)
            gameTitle.text = "Wheelie Master:\nMoto Ride 3D";
            
        if (studioName != null)
            studioName.text = "Wayfu Studio";
            
        if (gameDescription != null)
            gameDescription.text = "Contains ads • In-app purchases\n\nMaster the art of motorcycle wheelies in this thrilling 3D racing game!";
    }
    
    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
            
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
            
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (mainMenuButtonGameOver != null)
            mainMenuButtonGameOver.onClick.AddListener(ShowMainMenu);
            
        if (backButton != null)
            backButton.onClick.AddListener(HideSettings);
    }
    
    public void StartGame()
    {
        if (gameManager != null)
        {
            gameManager.StartGame();
            ShowGameplayUI();
        }
    }
    
    public void PauseGame()
    {
        if (gameManager != null)
            gameManager.PauseGame();
    }
    
    public void RestartGame()
    {
        if (gameManager != null)
            gameManager.RestartGame();
    }
    
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        if (gameManager != null)
            gameManager.ShowMainMenu();
    }
    
    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
    
    public void HideSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
    
    private void ShowGameplayUI()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    
    public void ShowGameOverUI()
    {
        gameOverPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        
        if (finalScoreText != null && gameManager != null)
            finalScoreText.text = "Final Score: " + gameManager.score.ToString();
            
        if (highScoreText != null && gameManager != null)
            highScoreText.text = "High Score: " + gameManager.highScore.ToString();
    }
    
    private void UpdateGameplayUI()
    {
        if (gameManager != null)
        {
            if (scoreText != null)
                scoreText.text = "Score: " + gameManager.score.ToString();
                
            if (speedText != null)
                speedText.text = "Speed: " + Mathf.RoundToInt(gameManager.gameSpeed).ToString() + " km/h";
        }
        
        if (motorcycleController != null)
        {
            float wheelieScore = motorcycleController.GetWheelieScore();
            
            if (wheelieText != null)
                wheelieText.text = motorcycleController.IsWheelieActive() ? "WHEELIE!" : "";
                
            if (wheelieBarSlider != null)
                wheelieBarSlider.value = wheelieScore;
            
            // Visual effects for wheelie
            if (wheelieParticles != null)
            {
                if (motorcycleController.IsWheelieActive() && !wheelieParticles.isPlaying)
                    wheelieParticles.Play();
                else if (!motorcycleController.IsWheelieActive() && wheelieParticles.isPlaying)
                    wheelieParticles.Stop();
            }
            
            // Speed lines effect
            if (speedLines != null)
            {
                speedLines.SetActive(gameManager.gameSpeed > 10f);
            }
        }
    }
    
    public void ApplyWheelieEffect()
    {
        StartCoroutine(WheelieScreenEffect());
    }
    
    private IEnumerator WheelieScreenEffect()
    {
        // Add screen shake or color flash for wheelie feedback
        Camera mainCamera = Camera.main;
        Vector3 originalPosition = mainCamera.transform.position;
        
        float shakeDuration = 0.5f;
        float shakeIntensity = 0.1f;
        
        for (float t = 0; t < shakeDuration; t += Time.deltaTime)
        {
            float xOffset = Random.Range(-shakeIntensity, shakeIntensity);
            float yOffset = Random.Range(-shakeIntensity, shakeIntensity);
            
            mainCamera.transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);
            yield return null;
        }
        
        mainCamera.transform.position = originalPosition;
    }
}