using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Playermouv : MonoBehaviourPunCallbacks
{
    public Camera camera;

    [Range(1,100)]
    [SerializeField] private float movementSpeed;
    float speedX, speedY;
    private bool is_seeing_right;
    [SerializeField] private Rigidbody2D rb2D;
    public Animator animator;
    private string currentanimation;
    private bool is_walking = false;
    private bool is_reversed = false;
    private bool is_sprinting = false;
    
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
        
        is_sprinting = Input.GetButton("Sprint");
        
        //Debug.Log(gameObject.transform.position);
        
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        
        is_seeing_right = mousePos.x > gameObject.transform.position[0];
		
        
        if (speedX == 0 && speedY == 0) {
            animator.SetBool("is_walking", false);
            is_walking = false;
        }
        else 
        {
            animator.SetBool("is_walking", true);
            is_walking = true;
            
            if (speedX < 0) // Mouvement Gauche
            {
                if (is_seeing_right)
                {
                    is_reversed = true;
                    animator.SetBool("is_reversed", true);
                } // Regarde a droite
                else
                {
                    is_reversed = false;
                    animator.SetBool("is_reversed", false);
                }
            }
            else if (speedX > 0) // Mouvement Droite 
            {
                if (is_seeing_right) {
                    is_reversed = false;
                    animator.SetBool("is_reversed", false);
                }
                else {
                    is_reversed = true;
                    animator.SetBool("is_reversed", true);
                }
            }
        }
        //checkanimation(is_walking,is_reversed);   
        
        
        if (is_seeing_right) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        

        if (!is_seeing_right)rb2D.transform.Translate(movementSpeed *  (!is_sprinting? 0.01f : 0.02f) * new Vector2(Input.GetAxisRaw("Horizontal")*-1, Input.GetAxisRaw("Vertical")).normalized);
        else rb2D.transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * ( (movementSpeed * (!is_sprinting? 0.01f : 0.02f))));
    }
    
    private void checkanimation(bool w,bool r){
        Debug.Log("w" + w + "r" + r);
        if (w) 
        {
            if (r)changeanimation("reversed_walk");
            else changeanimation("Walk");
        }
        else changeanimation("Idle");
    }
	


    private void changeanimation (string animation) 
    {
        if (currentanimation != animation)
        {
            currentanimation = animation;
            animator.Play(animation);
        }
    }
}
