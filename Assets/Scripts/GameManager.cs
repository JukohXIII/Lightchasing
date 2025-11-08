using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // or TMPro if you use TextMeshPro

public class GameManager : MonoBehaviour
{
    public PressureController pressureController;  // Reference to your pressure script
    public GameObject gameOverText;                 // Reference to your UI Text GameObject
    public int maxLives = 3;
    private int currentLives;
    private bool isGameOver = false;
    public Text livesText;

    void Start()
    {
        currentLives = maxLives;
        if (gameOverText != null)
            gameOverText.SetActive(false);

        UpdateLivesUI();
    }

    public void TakeDamage(int damage = 1)
    {
        if (isGameOver) return;

        currentLives -= damage;
        if (currentLives < 0) currentLives = 0;

        UpdateLivesUI();

        if (currentLives <= 0)
        {
            HandleGameOver();
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "" + currentLives;
    }

    void OnEnable()
    {
        if (pressureController != null)
            pressureController.OnPressureMaxReached.AddListener(HandleGameOver);
    }

    void OnDisable()
    {
        if (pressureController != null)
            pressureController.OnPressureMaxReached.RemoveListener(HandleGameOver);
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over! Showing message...");
        isGameOver = true;
        if (gameOverText != null)
            gameOverText.SetActive(true);
        // Optionally pause game logic here by setting timescale to 0
        Time.timeScale = 0f; // Pause the game
    }

    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f; // Resume time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
