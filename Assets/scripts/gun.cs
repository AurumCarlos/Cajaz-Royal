using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class gun : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    
    private Vector2 mousePos;
    Rigidbody2D rb;
    [Header("Munition")]
    private int Munition;
    public int CapaciteeChargeur;
    [SerializeField] private GameObject ExploPrefab;
    [SerializeField] private GameObject BbulletPrefab;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI MunitionsUI;
    
    
    [Header("Animation")]
    [SerializeField] private Animation animation;
    [SerializeField] private AnimationClip ReloadAnimation;
    [SerializeField] private AnimationClip ReculAnimation;
    
    
    
    
    [Header("Fire")]
    [SerializeField] private Transform firing;
    [SerializeField] private Transform firingbul;
    [SerializeField] private Camera Camera;
    [SerializeField] private float Firerate;
    [SerializeField] private int damage;
    private float Fire;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Munition = CapaciteeChargeur;
        MunitionsUI.text = Munition.ToString() + " / " + CapaciteeChargeur.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x)*Mathf.Rad2Deg -0f;
        bool is_seeing_right = mousePos[0] >= gameObject.transform.position[0];
        transform.localRotation = Quaternion.Euler((!is_seeing_right ? 180f : 0f),(!is_seeing_right ? 180f : 0f),(!is_seeing_right ? -angle: angle));
        
        if (Fire > 0) Fire -= Time.deltaTime;
        
        
        if (Input.GetMouseButton(0) && Fire<=0 && Munition > 0 && !animation.isPlaying){
            Shoot();
            Fire = 1/Firerate;
            Munition -= 1;
            MunitionsUI.text = Munition.ToString() + " / " + CapaciteeChargeur.ToString();
        }

        if (Input.GetKeyDown(KeyCode.R) && !animation.isPlaying && Munition != CapaciteeChargeur)
        {
            Reload();
        }
        
        
    }

    private void Reload()
    {
        animation.Play(ReloadAnimation.name);
        Munition = CapaciteeChargeur;
        
        MunitionsUI.text = Munition.ToString() + " / " + CapaciteeChargeur.ToString();
    }
    
    private void Shoot()
    {

        Ray ray = new (firing.transform.position, firing.transform.up);

        RaycastHit2D hit;
        Debug.DrawRay(firing.transform.position, firing.transform.up * 10, Color.red);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.GetComponent<Health>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("damage", RpcTarget.All, damage);
            }
        }
        PhotonNetwork.Instantiate(BbulletPrefab.name,firingbul.position,firingbul.rotation);
        PhotonNetwork.Instantiate(ExploPrefab.name,ray.origin,firing.rotation);
        
        //animation.Play(ReculAnimation.name);
    }
    
}
