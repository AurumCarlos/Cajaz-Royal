using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuEndGame : MonoBehaviour
{



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
