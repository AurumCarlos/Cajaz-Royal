using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

public class AttackState : State
{
    private static readonly int ScratchBool = Animator.StringToHash("scratch");
    private static readonly int BiteBool = Animator.StringToHash("bite");
    

    private float attackCooldown = 1.5f;
    private float timer;
    private float attackRange = 2.0f;
    //private float lastAttackTime = -Mathf.Infinity;

    private bool hasAttacked = false;

    public AttackState(AIAgent ai) : base(ai) { }

    public override void Enter()
    {
        Enemy.Path.canMove = false;
        ResetAttackBools();
        timer = 0;
    }

    public override void Update()
    {
        float distance = Vector2.Distance(Enemy.transform.position, Enemy.Target.position);

        if (distance > attackRange)
        {
            ExitToChase();
            return;
        }

        var random = new Random();
        int r = random.Next(0, 2);
        if (timer<=0)
        {
            if (r==0)
            {
                Enemy.animator.SetBool(ScratchBool, true);
                DealDamage(5);

            }
            else
            {
                Enemy.animator.SetBool(BiteBool, true);
                DealDamage(10);

            }
            timer = attackCooldown;
        }
        else
        {
            timer -= Time.deltaTime;
        }
            
    }

    private void DealDamage(int amount)
    {
        var health = Enemy.Target.GetComponent<HealthSolo>();
        if (health != null)
        {
            //health.Damage(amount);
            Enemy.Target.transform.gameObject.GetComponent<HealthSolo>().Damage(10);
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

  
}
