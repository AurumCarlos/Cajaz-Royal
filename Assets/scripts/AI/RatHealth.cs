using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RatHealth : MonoBehaviour
{
    private static readonly int IsDead = Animator.StringToHash("is_dead");
    [SerializeField] private AIAgent playercontAI;
    public bool islocalplayer;
    [Header("UI")]
    

    [SerializeField] private int health;

    private void Start()
    {
        //healthText.text = health.ToString();
    }
    [PunRPC]
    public void damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (islocalplayer)
            {
                RoomManager.Instance.Spwan();
            }
            Destroy(gameObject);
            //playercontAI.animator.Play(null);
            //playercontGameObject.animator.SetBool(IsDead, true);
        }
    }

    // Update is called once per frame
    
    private void Update()
    {
       
    }
}
