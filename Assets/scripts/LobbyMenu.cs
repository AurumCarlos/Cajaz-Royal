using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class LobbyMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject LobbyMenuUI;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject Game;
    [SerializeField] private GameObject Circle;
    //[SerializeField] private GameObject filtre;
    [SerializeField] private RoomManager roomManager;
    
    [SerializeField] private GameObject camera;
    [SerializeField] private PlayableDirector cine;
    [SerializeField] private GameObject cineCam;


    private float T = 0;
    

    void Update()
    {
        T -= Time.deltaTime;
        if (T <= 0 ) textMesh.text = "There is " + PhotonNetwork.CurrentRoom.PlayerCount + " Players in your room";
    }

    public void ResumeGameCall()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        photonView.RPC("ResumeGame", RpcTarget.AllBuffered);
    }

    
    [PunRPC]
    public void ResumeGame()
    {
        Game.SetActive(true);
        Circle.SetActive(true);
        camera.SetActive(false);
        //filtre.SetActive(true);
        roomManager.Spwan();
        
        LobbyMenuUI.SetActive(false);   
        cineCam.SetActive(true);
        cine.Play();
        DesactivateCineCam desac = cineCam.GetComponent<DesactivateCineCam>();
        desac.DesactivateCam();
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
