using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIAgent : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("is_walking");
    [SerializeField]public bool israt;
    public AIPath Path;
    [SerializeField] protected float MoveSpeed;
    [SerializeField] public Transform Target;
    [SerializeField] private float DistanceThreshold;
    private float TargetDistance;
    public Vector2 StartPos;
    public Health Health; 
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    protected float timer = 0f;
    [SerializeField] public Sprite shootFlashSprite;
    [SerializeField] public Material weaponTracerMaterial;
    public PatrolState patrolState;
    public ChaseState chaseState; 
    [SerializeField] public AttackState attackState;
    State currentState;
    [SerializeField] public SpriteRenderer gun;
    [SerializeField] public Transform muzzle; 
    public LayerMask mask;
    private float separationRadius=4.0f;
    private Rigidbody2D rb2D;
    private float scantime;

    private void MoveTo(Vector2 pos)
    {
        Path.destination = pos;
    }
    

    private void Start()
    {
        Path = GetComponent<AIPath>();
        patrolState = new PatrolState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        if (!israt)
        {
            attackState = new AttackStateEnemy(this);
        }
        StartPos = transform.position;
        currentState = patrolState;
        currentState.Enter();
        Health = GetComponent<Health>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    public Vector2 GetRandomTarget()
    {
        float randomX = Random.Range(-10f, 10f);  // Limits movement range
        float randomY = Random.Range(-10f, 10f);
        return StartPos + new Vector2(randomX, randomY);
    }

    public void SwitchState(State newState)
    {
        currentState = newState;
        if (newState is PatrolState)
        {
            StartPos = transform.position;
        }
        currentState.Enter();
    }
   

    private void Update()
    {
        scantime -= Time.deltaTime;
        if (scantime < 0)
        {
            GameObject[] l = GameObject.FindGameObjectsWithTag("Player");
            GameObject closest = l[0];
            if (l.Length == 0) ;
            else
                for (int i = 1; i < l.Length; i++)
                {
                    
                }
            Target = closest.transform;
            scantime = 1;
        }
        
        
        
        if (!israt)
        {
            if (Path.velocity!=Vector3.zero)
            {
                animator.SetBool(IsWalking,true);
            }
            AimAtTarget();
        }
        if (timer <= 0)
        {
            FindClosestTarget();
            timer = 1;
        }
        timer -= Time.deltaTime;

        Path.updateRotation = false;
        Path.maxSpeed = MoveSpeed;
        currentState.Update();
        Vector2 velocity = Path.desiredVelocity;
        if (velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }
        spriteRenderer.transform.rotation = Quaternion.identity;
        
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, separationRadius, mask);
        Vector2 avoidance = Vector2.zero;
        int count = 0;

        foreach (var col in neighbors)
        {
            if (col.gameObject == gameObject) continue;

            Vector2 diff = (Vector2)(transform.position - col.transform.position);
            float dist = diff.magnitude;

            if (dist > 0.01f)
            {
                // Apply stronger force when closer using inverse square law
                Vector2 pushDir = diff.normalized;

                // Add slight variation to break symmetry
                pushDir += new Vector2(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));

                // Final repulsion, stronger at close distances
                avoidance += pushDir / (dist * dist); 
                count++;
            }
        }

        if (count > 0)
        {
            avoidance /= count;

            // Optional: Normalize to avoid overly large forces
            if (avoidance.magnitude > 1f)
                avoidance = avoidance.normalized;

            rb2D.AddForce(avoidance * 5.0f, ForceMode2D.Force);
        }
        
    }
    
    public Vector3 GetRandomValidPatrolPoint()
    {
        

        for (int i = 0; i < 10; i++)
        {
            // Generate random point in 2D circle
            Vector2 randomPoint2D = (Vector2)transform.position + Random.insideUnitCircle * 10f;
            Vector3 randomPoint = new Vector3(randomPoint2D.x, randomPoint2D.y, transform.position.z);

            // Find the nearest node to the random point
            NNInfo nodeInfo = AstarPath.active.GetNearest(randomPoint, NNConstraint.Default);

            if (nodeInfo.node != null && nodeInfo.node.Walkable)
            {
                return (Vector3)nodeInfo.position;
            }
        }

        // If no valid point found, return current position
        return transform.position;
    }
    
    public void AimAtTarget()
    {
        Vector2 direction = (Target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate gun
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Flip the sprite if looking to the left (angle > 90 or < -90)
        bool facingLeft = angle > 90f || angle < -90f;
        gun.flipY = facingLeft;

        // Flip muzzle position if needed (optional)
       
            // Vector3 localPos = muzzle.localPosition;
            // localPos.y = facingLeft ? -Mathf.Abs(localPos.y) : Mathf.Abs(localPos.y);
            // muzzle.localPosition = localPos;
    }
   

    

    
    private void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        if (targets.Length > 0)
        {
            GameObject closest = targets[0];
            for (int i = 1; i < targets.Length; i++)
            {
                if (Vector2.Distance(transform.position, targets[i].transform.position) <
                    Vector2.Distance(transform.position, closest.transform.position)) closest = targets[i];
            }
            Target = closest.transform;
        }
    }
}
