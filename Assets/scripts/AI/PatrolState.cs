using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolState : State
{
    private float DistanceThreshold = 2.0f;
    Vector2 RandomPos;
    private static readonly int IsPatrolling = Animator.StringToHash("is_patrolling");
    public PatrolState(AIAgent ai) : base(ai){}
    private float timer = 10.0f;
    private Transform lastpos;
    public override void Enter()
    {
        lastpos = Enemy.transform;
        if (Enemy.israt)
        {
            Enemy.animator.SetBool(IsPatrolling,true);
        }
        RandomPos = Enemy.GetRandomTarget();
    }
    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Enemy.Path.destination = Enemy.GetRandomTarget();
            timer = 5.0f;
            
        }
       
        var targetDistance = Vector2.Distance(Enemy.transform.position,
            Enemy.Target.position);
        if (targetDistance < 10.0f)
        {
            if (Enemy.israt)
            {
                Enemy.animator.SetBool(IsPatrolling,false);
            }
            Enemy.SwitchState(Enemy.chaseState);
            return;
        }
        if (Vector2.Distance(Enemy.transform.position, RandomPos) < 
            DistanceThreshold)
        {
            RandomPos = Enemy.GetRandomTarget();
        }
        Enemy.Path.destination = RandomPos;
    }
}
