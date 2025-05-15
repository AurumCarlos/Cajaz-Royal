using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEndGame : MonoBehaviour
{
    [Header("UI du Menu de Fin de Partie")]
    public GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowEndGame()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}
