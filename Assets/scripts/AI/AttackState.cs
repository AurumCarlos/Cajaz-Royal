using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackState : State
{
    private static readonly int ScratchBool = Animator.StringToHash("scratch");
    private static readonly int BiteBool = Animator.StringToHash("bite");
    

    private float attackCooldown = 1.5f;
    private float attackRange = 2.0f;
    private float lastAttackTime = -Mathf.Infinity;

    private bool hasAttacked = false;

    public AttackState(AIAgent ai) : base(ai) { }

    public override void Enter()
    {
        Enemy.Path.canMove = false;
        ResetAttackBools();
        hasAttacked = false;
    }

    public override void Update()
    {
        float distance = Vector2.Distance(Enemy.transform.position, Enemy.Target.position);

        if (distance > attackRange)
        {
            ExitToChase();
            return;
        }
        
        if (!hasAttacked && Time.time >= lastAttackTime + attackCooldown)
        {
            hasAttacked = true;
            lastAttackTime = Time.time;

            // Randomly choose attack
            bool isScratch = Random.value > 0.5f;
            ResetAttackBools();

            if (isScratch)
            {
                Enemy.animator.SetBool(ScratchBool, true);
                DealDamage(5);
            }
            else
            {
                Enemy.animator.SetBool(BiteBool, true);
                DealDamage(10);
            }
        }
    }

    private void DealDamage(int amount)
    {
        var health = Enemy.Target.GetComponent<Health>();
        if (health != null)
        {
            health.damage(amount);
        }
    }
    private void ResetAttackBools()
    {
        Enemy.animator.SetBool(ScratchBool, false);
        Enemy.animator.SetBool(BiteBool, false);
    }
    private void ExitToChase()
    {
        ResetAttackBools();
        Enemy.Path.canMove = true;
        hasAttacked = false;
        Enemy.SwitchState(Enemy.chaseState);
    }

    public void FinishAttackAnimation()
    {
        hasAttacked = false;
        ResetAttackBools();
    }
}
