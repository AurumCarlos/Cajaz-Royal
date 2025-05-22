using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;


public class Gun : MonoBehaviourPunCallbacks
{
    private Vector2 mousePos;
    private Rigidbody2D rb;
    
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }
    private Transform GunEndPointTransform;
    
    [Header("Munition")]
    public int Munition;
    public int CapaciteeChargeur;
    public int Range;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI MunitionsUI;
    
    [Header("Animation")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animation animation;
    [SerializeField] private AnimationClip ReloadAnimation;
    
    [Header("Fire")]
    [SerializeField] private Camera Camera;

    private CameraSmoothFollow cameraSmoothFollow;
    [SerializeField] public float Firerate;
    [SerializeField] public int damage;
    
    private float _timer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Munition = CapaciteeChargeur;
        
        animator = GetComponent<Animator>();
        GunEndPointTransform = transform.Find("GunEndPointPosition");
        cameraSmoothFollow = Camera.GetComponent<CameraSmoothFollow>();
        
        spriteRenderer = transform.Find("Visual").GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer.sprite == null)
        {
            cameraSmoothFollow.maxViewDistance = 5;
            MunitionsUI.text = "";
            return;
        }

        MunitionsUI.text = $"{Munition} / {CapaciteeChargeur}";
        
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x)*Mathf.Rad2Deg -0f;
        bool is_seeing_right = mousePos[0] >= gameObject.transform.position[0];
        transform.localRotation = Quaternion.Euler(
            (!is_seeing_right ? 180f : 0f),
            (!is_seeing_right ? 180f : 0f),
            (!is_seeing_right ? -angle: angle)
            );
        
        if (_timer > 0) _timer -= Time.deltaTime;
        
        if (Input.GetMouseButton(0) && _timer <= 0 && Munition > 0 && !animation.isPlaying)
        {
            Shoot();
            _timer = Firerate;
        }

        if (Input.GetKeyDown(KeyCode.R) && !animation.isPlaying && Munition != CapaciteeChargeur) Reload();
    }

    private void Reload()
    {
        animation.Play(ReloadAnimation.name);
        Munition = CapaciteeChargeur;
    }
    
    private void Shoot()
    {
        Munition--;
        animator.SetTrigger("Shoot");
        OnShoot?.Invoke(this, new OnShootEventArgs { 
                        gunEndPointPosition = GunEndPointTransform.position,
                        shootPosition = mousePos,
                    });
    }
    
}
