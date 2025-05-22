using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PatrolState : State
{
    private float DistanceThreshold = 2.0f;
    Vector2 RandomPos;
    private static readonly int IsPatrolling = Animator.StringToHash("is_patrolling");
    public PatrolState(AIAgent ai) : base(ai){}
    public override void Enter()
    {
        Enemy.animator.SetBool(IsPatrolling,true);
        RandomPos = Enemy.GetRandomTarget();
    }
    public override void Update()
    {
        var targetDistance = Vector2.Distance(Enemy.transform.position,
            Enemy.Target.position);
        if (targetDistance < 10.0f)
        {
            Enemy.animator.SetBool(IsPatrolling,false);
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
