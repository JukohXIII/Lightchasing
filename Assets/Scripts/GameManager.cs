using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // or TMPro if you use TextMeshPro

public class GameManager : MonoBehaviour
{
    public PressureController pressureController;  // Reference to your pressure script
    public GameObject gameOverText;                 // Reference to your UI Text GameObject
    public GameObject finishText;
    public GameObject fadePanel;
    private bool isVictory = false;
    public int maxLives = 3;
    [SerializeField] private int currentLives;
    private bool isGameOver = false;
    public GameObject hearts;

    void Start()
    {
        currentLives = maxLives;
        if (gameOverText != null)
            gameOverText.SetActive(false);
        if (finishText != null)
            finishText.SetActive(false);
        if (fadePanel != null)
            fadePanel.SetActive(false);
            
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
        if (currentLives == 2)
        {
            hearts.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (currentLives == 1)
        {
            hearts.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (currentLives <= 0 || currentLives == 0)
        {
            hearts.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        

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

    public void HandleVictory()
    {
        if (isVictory || isGameOver) return;

        isVictory = true;
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
        {
            yield return StartCoroutine(FadeManager.Instance.FadeToBlack());

            Time.timeScale = 0f;

            if (finishText != null)
                finishText.SetActive(true);
            if (fadePanel != null)
                fadePanel.SetActive(false);
        }

    void Update()
    {
        if ((isGameOver || isVictory) && Input.GetKeyDown(KeyCode.Space))
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
