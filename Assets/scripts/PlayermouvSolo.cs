using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayermouvSolo : MonoBehaviour
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
    
    private InventoryHandlerSolo _inventoryHandler;

    private void Awake()
    {
        _inventoryHandler = transform.GetComponent<InventoryHandlerSolo>();
    }

    void FixedUpdate()
    {
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");
        
        is_sprinting = Input.GetButton("Sprint");
        
        
        Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        is_seeing_right = mousePos.x > gameObject.transform.position[0];
		
        
        if (speedX == 0 && speedY == 0) {
            is_walking = false;
            animator.SetBool("is_walking", is_walking);
        }
        else 
        {
            is_walking = true;
            animator.SetBool("is_walking", is_walking);
            
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
        
        if (is_seeing_right) gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        else gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        

        if (!is_seeing_right)rb2D.transform.Translate(movementSpeed *  (!is_sprinting? 0.01f : 0.02f) * new Vector2(Input.GetAxisRaw("Horizontal")*-1, Input.GetAxisRaw("Vertical")).normalized);
        else rb2D.transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * ( (movementSpeed * (!is_sprinting? 0.01f : 0.02f))));
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        WorldItemSolo worldItem = other.GetComponent<WorldItemSolo>();
        if (worldItem!= null)
        {
            if (_inventoryHandler.Inventory != null && _inventoryHandler.Inventory.AddItem(worldItem.Item))
            {
                Destroy(worldItem.gameObject);
                _inventoryHandler.UpdateInventory();
            }
        }
    }
}
