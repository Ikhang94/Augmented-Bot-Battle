using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PumaBot : BaseBot
{
    public HitBox attack1,attack2,attack3,attack4;
    public HeavyHitbox heavyAttackHitbox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        LockOn();
        Move();
        AttackAction();
        HeavyAttackAction();
        Dodge();
        StateMachine();
        attackInputBuffering();
    }

    protected override void StateMachine()
    {
        switch (state)
        {
            case "dodge":
                canMove = false;
                canTurn = true;
                canAttack = false;
                canDodge = false;
                transform.position += transform.TransformDirection(dodgeDirection * (timer * 10) * Time.deltaTime)*transform.lossyScale.x;
                timer -= Time.deltaTime;

                if (timer < 0.2f)
                {
                    invincible = false;
                }
                else { invincible = true; }

                if (timer < 0)
                {
                    state = "idle";
                    timer = 0;
                    canMove = true;
                    canTurn = true;
                    canAttack = true;
                    canDodge = true;

                }

                break;

            case "attack":
                canMove = false;
                canTurn = false;
                canAttack = false;
                canDodge = false;
                transform.position += transform.TransformDirection(new Vector3(0, 0, 0) * Time.deltaTime);
                timer -= Time.deltaTime;
                if (animator != null)
                {
                    animator.Play("Base Layer.Armature|Attack 0");
                }
                if (timer < 0.5f - 0.03f && timer > 0.5f - 0.09f)
                {    
                        //bot.Hurt(dmg, 2, transform.TransformDirection(new Vector3(0, 3, 1)));
                        attack1.hit();
                }
                else
                {
                    attack1.clearHitbox();
                }

                if (attack1.bothit.Count > 0 || timer < 0.2f)
                {
                    AttackActionFollowups();
                }


                if (timer <= 0)
                {
                    state = "idle";
                    timer = 0;
                    canMove = true;
                    canTurn = true;
                    canAttack = true;
                    canDodge = true;
                    bothit.Clear();
                }
                break;

            case "attack2":
                canMove = false;
                canTurn = false;
                canAttack = false;
                canDodge = false;
                timer -= Time.deltaTime;
                if (animator != null)
                {
                    animator.Play("Base Layer.Armature|Attack_2");
                }

                if (timer <= 0.5f-0.14f)
                {
                    attack2.hit();
                }
                else
                {
                    attack2.clearHitbox();
                }

                if (timer < 0)
                {
                    state = "idle";
                    timer = 0;
                    canMove = true;
                    canTurn = true;
                    canAttack = true;
                    canDodge = true;
                    bothit.Clear();
                }
                break;
                case "heavy attack":
                    canMove = false;
                    canTurn = false;
                    canAttack = false;
                    canDodge = false;
                    timer -= Time.deltaTime;
                    if (animator != null)
                    {
                        animator.Play("Base Layer.Armature|HeavyAttackV2");
                    }

                    if (timer <= 1.7f - 0.25f && timer >=1.7f - 0.41f)
                    {
                        heavyAttackHitbox.hit();
                        transform.position += transform.TransformDirection(new Vector3(0, 0, 10f) * timer * Time.deltaTime) * transform.lossyScale.x;
                    }
                    else if(timer <= 1.7f - 0.25f)
                    {
                        transform.position += transform.TransformDirection(new Vector3(0, 0, 10f) * timer * Time.deltaTime) * transform.lossyScale.x;
                        heavyAttackHitbox.clearHitbox();
                    }
                    else
                    {
                        heavyAttackHitbox.clearHitbox();
                    }
                    if (timer > 1.7f - 0.25f)
                    {
                        heavyAttackHitbox.isCombo = true;
                    }

                    if (heavyAttackHitbox.isCombo==false && heavyAttackHitbox.bothit.Count<=1 && doAttack)
                    {
                    state = "attack2";
                    timer = 0.5f;
                    attack2.bothit.Clear();
                }

                    if (timer < 0)
                    {
                        state = "idle";
                        timer = 0;
                        canMove = true;
                        canTurn = true;
                        canAttack = true;
                        canDodge = true;
                        bothit.Clear();
                    }
                    break;

            case "hurt":
                canAttack = false;
                canMove = false;
                canTurn = false;
                canDodge = false;
                timer -= Time.deltaTime;
                bothit.Clear();

                if (timer < 0)
                {
                    state = "idle";
                    timer = 0;
                    canMove = true;
                    canTurn = true;
                    canAttack = true;
                    canDodge = true;

                }
                break;
        }

    }

    protected void AttackActionFollowups()
    {
        Debug.Log($"{attackJustPressed} :: {doAttack}");
        if (attack != null && attackJustPressed || doAttack)
        {
            switch(state)
            {
                case "attack":
                    state = "attack2";
                    timer = 0.5f;
                    attack2.bothit.Clear();
                    break;
            }
        }
    }


    
}
