using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenuUI.SetActive(isPaused);
        
    }
    
    public void CloseSettingsWindow()
    {
        pauseMenuUI.SetActive(false);
    }
    
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Remplace si le nom est différent
    }
}
