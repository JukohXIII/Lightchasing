using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("SceneController");
                instance = obj.AddComponent<SceneController>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
  
    /// <summary>
    /// Load the main game scene
    /// </summary>
    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    

    /// <summary>
    /// Load the main menu scene
    /// </summary>
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit game
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Restat the game
    /// </summary>
    public void RestartGame()
    {
        LoadMainScene();
    }
}