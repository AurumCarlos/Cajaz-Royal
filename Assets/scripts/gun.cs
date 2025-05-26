using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Random = UnityEngine.Random;


public class Gun : MonoBehaviourPunCallbacks
{
    private Vector2 mousePos;
    private Rigidbody2D rb;
    
    public event EventHandler<OnShootEventArgs> OnShoot;
    public event EventHandler<OnShootEventArgs> OnReload;
    public class OnShootEventArgs : EventArgs {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }
    
    private Transform GunEndPointTransform;
    
    [Header("Munition")]
    public Item Munition;
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
    private bool _isInInventory;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        GunEndPointTransform = transform.Find("GunEndPointPosition");
        cameraSmoothFollow = Camera.GetComponent<CameraSmoothFollow>();
        
        spriteRenderer = transform.Find("Visual").GetComponentInChildren<SpriteRenderer>();
        _isInInventory = false;
    }

    void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.I)) _isInInventory = !_isInInventory;
            if (_isInInventory) return;
            


        
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x)*Mathf.Rad2Deg -0f;
            bool is_seeing_right = mousePos[0] >= gameObject.transform.position[0];
            transform.localRotation = Quaternion.Euler(
                (!is_seeing_right ? 180f : 0f),
                (!is_seeing_right ? 180f : 0f),
                (!is_seeing_right ? -angle: angle)
            );
            
            if (spriteRenderer.sprite == null)
            {
                cameraSmoothFollow.maxViewDistance = 5;
                MunitionsUI.text = "";
                return;
            }
            
            cameraSmoothFollow.maxViewDistance = Range * 0.5f;
            MunitionsUI.text = $"{Munition.Amount} / {CapaciteeChargeur}";

        
            if (_timer > 0) _timer -= Time.deltaTime;
        
            if (Input.GetMouseButton(0) && _timer <= 0 && Munition.Amount > 0 && !animation.isPlaying && !_isInInventory)
            {
                Shoot();
                _timer = Firerate;
            }

            if (Input.GetKeyDown(KeyCode.R) && !animation.isPlaying && Munition.Amount != CapaciteeChargeur) Reload();
        } catch (NullReferenceException) { }
    }

    private void Reload()
    {
        animation.Play(ReloadAnimation.name);
        OnReload?.Invoke(this, new OnShootEventArgs());
    }
    
    private void Shoot()
    {
        AudioSource a = GetComponent<AudioSource>();
        Debug.Log(a);
        if (a) a.Play();
        Munition.Amount--;
        animator.SetTrigger("Shoot");
        OnShoot?.Invoke(this, new OnShootEventArgs { 
                        gunEndPointPosition = GunEndPointTransform.position,
                        shootPosition = mousePos,
                    });
        
        
    }
    
}
