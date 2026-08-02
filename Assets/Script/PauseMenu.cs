using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public AudioSource backgroundMusic;

    public string gameSceneName = "GameScene"; 

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ExitGame()
    {
        
        SceneManager.LoadScene(gameSceneName);
        
    }

    public void toggleMusic()
    {
        backgroundMusic.mute = !backgroundMusic.mute;
    }
}