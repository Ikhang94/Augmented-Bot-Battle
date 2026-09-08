using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnightBotNetwork : BaseBotNetwork
{
    public HitBoxNetwork attack1, attack2, attack3, attack4;
    public HeavyHitboxNetwork heavyAttackHitbox;

    void Update()
    {
        LockOn();
        if (IsOwner)
        {
            Move();
            AttackAction();
            HeavyAttackAction();
            Dodge();
            StateMachineRPC();
            attackInputBuffering();
        }
    }

    public override void StateMachineRPC()
    {
        switch (instanceState)
        {
            case "dodge":
                canMove = false;
                canTurn = true;
                canAttack = false;
                canDodge = false;
                transform.position += transform.TransformDirection(dodgeDirection * (timer * 10) * Time.deltaTime) * transform.lossyScale.x;
                timer -= Time.deltaTime;

                if (timer < 0.2f)
                {
                    invincible.Value = false;
                }
                else { invincible.Value = true; }

                if (timer < 0)
                {
                    state.Value = "";
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
                    animator.Play("Base Layer.Armature|LightAttack1");
                }
                if (timer < 0.5f - 0.03f && timer > 0.5f - 0.09f)
                {
                    //bot.Hurt(dmg, 2, transform.TransformDirection(new Vector3(0, 3, 1)));
                    attack1.hit();
                }
                else
                {
                    attack1.clearHitboxServerRpc();
                }

                if (attack1.bothit.Count > 0 || timer < 0.2f)
                {
                    AttackActionFollowups();
                }


                if (timer <= 0)
                {
                    state.Value = "";
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
                    animator.Play("Base Layer.Armature|LightAttack2");
                }

                if (timer <= 0.5f - 0.14f)
                {
                    attack2.hit();
                }
                else
                {
                    attack2.clearHitboxServerRpc();
                }

                if (timer < 0)
                {
                    state.Value = "";
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
                    animator.Play("Base Layer.Armature|HeavyAttack");
                }

                if (timer <= 0.7f - 0.3f && timer >= 0.7f - 0.375f)
                {
                    heavyAttackHitbox.hit();
                    //transform.position += transform.TransformDirection(new Vector3(0, 0, 10f) * timer * Time.deltaTime) * transform.lossyScale.x;
                }
                else if (timer <= 0.7f - 0.375f)
                {
                    //transform.position += transform.TransformDirection(new Vector3(0, 0, 10f) * timer * Time.deltaTime) * transform.lossyScale.x;
                    heavyAttackHitbox.clearHitboxServerRpc();
                }
                else
                {
                    heavyAttackHitbox.clearHitboxServerRpc();
                }
                if (timer > 0.7f - 0.375f)
                {
                    heavyAttackHitbox.isCombo = true;
                }

                if (heavyAttackHitbox.isCombo == false && heavyAttackHitbox.bothit.Count <= 1 && doAttack)
                {
                    state.Value = "attack2";
                    timer = 0.5f;
                    attack2.bothit.Clear();
                }

                if (timer < 0)
                {
                    state.Value = "";
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
                    state.Value = "";
                    timer = 0;
                    canMove = true;
                    canTurn = true;
                    canAttack = true;
                    canDodge = true;

                }
                break;
        }

    }

    public override void HeavyAttackAction()
    {
        bool doAttackAction = (heavyAttack != null && heavyAttackJustPressed) || doHeavyAttack;
        if (doHeavyAttack && canHeavyAttack)
        {
            Debug.Log("heavy attacking: " + canHeavyAttack + heavyAttackJustPressed + heavyAttack.IsPressed());
            state.Value = "heavy attack";
            timer = 0.7f;
        }
    }

    protected void AttackActionFollowups()
    {
        Debug.Log($"{attackJustPressed} :: {doAttack}");
        if (attack != null && attackJustPressed || doAttack)
        {
            switch (instanceState)
            {
                case "attack":
                    state.Value = "attack2";
                    timer = 0.5f;
                    attack2.bothit.Clear();
                    break;
            }
        }
    }



}
