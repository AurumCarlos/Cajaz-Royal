using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CameraSmoothFollow : MonoBehaviourPunCallbacks
{
    
    [SerializeField] private GameObject player;
    [SerializeField] public float maxViewDistance = 10f;
    [SerializeField] private float velocity = .1f;

    private Rigidbody2D rb2D;


    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    
    private Vector3 GetDesiredPosition(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - player.transform.position).normalized;
        var dist = Vector3.Distance(player.transform.position, targetPos);
        
        if (dist > maxViewDistance) dist = maxViewDistance;
        return player.transform.position + dir * dist * 0.5f + new Vector3(0, 0, -10);
    }
    
    void FixedUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 desiredPosition = GetDesiredPosition(mousePos);
        
        Vector3 smoothFollow = Vector3.Lerp(transform.position, desiredPosition, velocity);
        transform.position = smoothFollow;
    }
}
