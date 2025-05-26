using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    private static readonly int IsChasing = Animator.StringToHash("is_chasing");
    private float Range = 10.0f; 
    private float attackRange = 2.0f;

    public ChaseState(AIAgent ai) : base(ai){}
    public override void Enter()
    {
        if (!Enemy.israt)
        {
            attackRange = 8.0f;
        }
        Enemy.Path.destination = Enemy.Target.transform.position;
        if (Enemy.israt)
        {
            Enemy.animator.SetBool(IsChasing, true);

        }
    }
    public override void Update()
    {
        var targetDistance = Vector2.Distance(Enemy.transform.position,
            Enemy.Target.position);
        if (targetDistance>Range)
        {
            if (Enemy.israt)
            {
                Enemy.animator.SetBool(IsChasing, false);
            }
            Enemy.SwitchState(Enemy.patrolState);
            return;
        }
        if (targetDistance<attackRange)
        {
            if (Enemy.israt)
            {
                Enemy.animator.SetBool(IsChasing, false);
            }
            Enemy.SwitchState(Enemy.attackState);
            return;
        }
        Enemy.Path.destination = Enemy.Target.transform.position;
    }
}
