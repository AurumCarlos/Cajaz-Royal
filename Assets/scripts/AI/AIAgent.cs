using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
   
    public AIPath Path;
    [SerializeField] private float MoveSpeed;
    [SerializeField] public Transform Target;
    [SerializeField] private float DistanceThreshold;
    private float TargetDistance;
    public Vector2 StartPos;
    public Health Health; 
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    


    public PatrolState patrolState;
    public ChaseState chaseState; 
    public AttackState attackState;
    State currentState;

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
        StartPos = transform.position;
        currentState = patrolState;
        currentState.Enter();
        Health = GetComponent<Health>();
    }
    public Vector2 GetRandomTarget()
    {
        float randomX = Random.Range(-5f, 5f);  // Limits movement range
        float randomY = Random.Range(-5f, 5f);
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
        Path.updateRotation = false;
        Path.maxSpeed = MoveSpeed;
        currentState.Update();
        Vector2 velocity = Path.desiredVelocity;

        if (velocity.x != 0)
        {
            spriteRenderer.flipX = velocity.x < 0;
        }
        spriteRenderer.transform.rotation = Quaternion.identity;
        
        // // MoveTo(RandomPos);
        // // if (Vector2.Distance(transform.position, RandomPos) < DistanceThreshold)
        // // {
        // //     RandomPos = GetRandomTarget();
        // // }
        // TargetDistance = Vector2.Distance(transform.position, Target.position);
        // if (TargetDistance < DistanceThreshold)
        // {
        //     Path.destination = transform.position;
        // }
        // else
        // {
        //     Path.destination = Target.position;
        // }
    }
}
