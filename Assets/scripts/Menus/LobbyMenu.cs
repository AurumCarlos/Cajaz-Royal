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


    [SerializeField] private WalkerGenerator W;
    
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
        int seed = (int)(DateTime.Now.Millisecond);
        photonView.RPC("ResumeGame", RpcTarget.Others, seed);
        
        StartCoroutine(ResumeGameLocal(seed));
    }

    private IEnumerator ResumeGameLocal(int s)
    {
        yield return new WaitForSeconds(0.5f);
        ResumeGame(s);
    }
    
    [PunRPC]
    public void ResumeGame(int s)
    {
        Game.SetActive(true);
        Circle.SetActive(true);
        camera.SetActive(false);
        //filtre.SetActive(true);
        W.launch(s);
        roomManager.Spwan();
        
        LobbyMenuUI.SetActive(false);   
        cineCam.SetActive(true);
        cine.Play();
        DesactivateCineCam desac = cineCam.GetComponent<DesactivateCineCam>();
        desac.DesactivateCam();
    }
    
        
    public void QuitToMainMenu()
    {
        PhotonNetwork.Disconnect();
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
