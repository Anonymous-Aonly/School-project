using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "GameScene"; 
    public AudioSource BackgroundMusic;

    [Header("UI Panels")]
    [Tooltip("Drag your Mode Selection Panel here")]
    public GameObject modeSelectionPanel;

    // Static variable so the Game Scene knows if the music is muted
    public static bool IsMuted { get; private set; }

    private void Awake()
    {
        // Keep this object alive when loading the Game Scene
        DontDestroyOnLoad(this.gameObject);

        // ✅ Make sure the panel is hidden when the game first starts
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(false);
        }

        // Apply the saved mute state
        if (BackgroundMusic != null)
        {
            BackgroundMusic.mute = IsMuted;
        }
    }

    // ✅ Renamed from StartGame - Hook your main "Start" button to this
    public void OpenModeSelection()
    {
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(true);
        }
    }

    // ✅ NEW - Hook your "Back" button inside the panel to this
    public void BackToMainMenu()
    {
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(false);
        }
    }

    // ✅ NEW - Hook your specific mode buttons (e.g., Classic, Timed) to this
    public void SelectAndStartMode()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); 
    }

    public void ToggleAudio()
    {
        if (BackgroundMusic != null)
        {
            IsMuted = !IsMuted;
            BackgroundMusic.mute = IsMuted;
        }
    }
}