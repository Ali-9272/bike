using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameSpeed = 5f;
    public float speedIncrement = 0.1f;
    public int score = 0;
    public int highScore = 0;
    
    [Header("UI References")]
    public Text scoreText;
    public Text highScoreText;
    public Text gameOverText;
    public Text instructionText;
    public Button restartButton;
    public Button mainMenuButton;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    
    [Header("Game State")]
    public bool isGameActive = false;
    public bool isGamePaused = false;
    
    private SimpleMotorcycle motorcycleController;
    
    private void Start()
    {
        LoadHighScore();
        motorcycleController = FindObjectOfType<SimpleMotorcycle>();
        ShowMainMenu();
    }
    
    private void Update()
    {
        if (isGameActive && !isGamePaused)
        {
            UpdateScore();
            IncreaseSpeed();
            UpdateUI();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameActive)
                PauseGame();
            else
                ShowMainMenu();
        }
    }
    
    public void StartGame()
    {
        isGameActive = true;
        isGamePaused = false;
        score = 0;
        gameSpeed = 5f;
        
        ShowGameplayPanel();
        if (motorcycleController != null)
            motorcycleController.StartGame();
    }
    
    public void GameOver()
    {
        isGameActive = false;
        
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }
        
        ShowGameOverPanel();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void PauseGame()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
    }
    
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        isGameActive = false;
        Time.timeScale = 1f;
    }
    
    private void ShowGameplayPanel()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }
    
    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }
    
    private void UpdateScore()
    {
        score += Mathf.RoundToInt(gameSpeed * Time.deltaTime);
    }
    
    private void IncreaseSpeed()
    {
        gameSpeed += speedIncrement * Time.deltaTime;
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore.ToString();
    }
    
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
}