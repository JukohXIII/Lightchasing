using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    public Button startButton;
    public Button quitButton;
    public Button settingsButton;

    [Header("SettingsPanel")]
    public GameObject settingsPanel;

    void Start()
    {
        // Bond button click events
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClick);
        }

        // Make sure settings panel is hidden at start
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Unlock and show cursor (if back from gameplay)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Start game button click event
    /// </summary>
    private void OnStartButtonClick()
    {
        //Debug.Log("Start Game button clicked");
        
        // Play button click sound (if any)
        // AudioManager.Instance.PlayButtonClickSound();
        
        // Load main game scene
        SceneController.Instance.LoadMainScene();
    }

    /// <summary>
    /// Quit game button click event
    /// </summary>
    private void OnQuitButtonClick()
    {
        //Debug.Log("Quit Game button clicked");
        
        // Play button click sound (if any)
        // AudioManager.Instance.PlayButtonClickSound();
        
        // Quit game
        SceneController.Instance.QuitGame();
    }

    /// <summary>
    /// Settings button click event
    /// </summary>
    private void OnSettingsButtonClick()
    {
        Debug.Log("Settings button clicked");
        
        // Play button click sound (if any)
        // AudioManager.Instance.PlayButtonClickSound();
        
        // Shift settings panel visibility
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }

    /// <summary>
    /// Show quit confirmation dialog
    /// </summary>
    // private void ShowQuitConfirmation()
    // {
    //     // 简单版本：直接退出
    //     SceneController.Instance.QuitGame();

        // 复杂版本：弹出确认对话框
        /*
        #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog("退出游戏", "确定要退出游戏吗？", "确定", "取消"))
            {
                SceneController.Instance.QuitGame();
            }
        #else
            // 在构建版本中使用UI确认对话框
            ShowQuitDialogUI();
        #endif
        */
    // }

    void Update()
    {
        // Hot key handling
        HandleKeyboardInput();
    }

    /// <summary>
    /// 处理键盘输入
    /// </summary>
    private void HandleKeyboardInput()
    {
        // Enter or Space to start game
        // if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnStartButtonClick();
        // }

        // ESC to quit game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnQuitButtonClick();
        }
    }

    void OnDestroy()
    {
        // Clear button click events, to avoid memory leaks
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartButtonClick);
        
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitButtonClick);
        
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
    }
}