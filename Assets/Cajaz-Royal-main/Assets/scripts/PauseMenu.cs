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
        ToggleHud(!isPaused);

        pauseMenuUI.SetActive(isPaused);
    }
    
    public void CloseSettingsWindow()
    {
        isPaused = !isPaused;
        ToggleHud(!isPaused);
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
    
    
    private void ToggleHud(bool b)
    {
        GameObject[] ennemis = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in ennemis)
        {
            Transform t = player.transform.Find("Canvas");
            if (t)
            {
                t.gameObject.SetActive(b);
            }

        }
    }
}
