using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SamuraiBotNetwork : BaseBotNetwork
{
    public HitBoxNetwork attack1, attack2, attack3, attack4, specialAttack, heavySpecialAttack;
    public HeavyHitboxNetwork heavyAttackHitbox;
    public bool isHoldingSpecial = false;
    public float holdCharge = 0;



    // Update is called once per state.Value = "";
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
                transform.position += transform.TransformDirection(dodgeDirection * transform.lossyScale.y * (timer * 10) * Time.deltaTime);
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

            case "SpecialCharge":
                canMove = false;
                canTurn = false;
                canAttack = false;
                canDodge = false;

                animator.Play("Base Layer.SpecialCharge");

                if (isHoldingSpecial)
                {
                    holdCharge += Time.deltaTime;
                }
                else if (holdCharge >= 1)
                {
                    holdCharge = 0;
                    state.Value = "specialStrongFollowUp";
                }
                else
                {
                    if (holdCharge > 0.5)
                    {
                        holdCharge = 0;
                        state.Value = "specialStrongFollowUp";
                    }
                    else
                    {
                        holdCharge = 0;
                        state.Value = "specialFollowUp";
                    }
                }
                break;

            case "specialStrongFollowUp":
                animator.Play("Base Layer.HeavySpecialAttackFollowup");
                rb.angularVelocity = new Vector3(0, 10, 0);
                timer += Time.deltaTime;

                if (timer >= 11f / 60f && timer <= 15f / 60f)
                {
                    heavySpecialAttack.hit();
                }
                else
                {
                    heavySpecialAttack.clearHitboxServerRpc();
                }

                if (timer > 1)
                {
                    state.Value = "default";
                }
                break;

            case "specialFollowUp":
                animator.Play("Base Layer.SpecialAttackFollowup");
                //rb.angularVelocity = new Vector3(1, 1, 0);
                timer += Time.deltaTime;
                if (timer >= 20f / 60f && timer <= 26f / 60f)
                {
                    specialAttack.hit();
                }
                else
                {
                    specialAttack.clearHitboxServerRpc();
                }
                if (timer > 1)
                {
                    state.Value = "default";
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
                    animator.Play("Base Layer.Armature|ChargedAttack");
                }

                if (timer <= 1.7f - 0.25f && timer >= 1.7f - 0.41f)
                {
                    heavyAttackHitbox.hit();
                    transform.position += transform.TransformDirection(new Vector3(0, 0, timer * 10f) * Time.deltaTime) * transform.lossyScale.x;
                }
                else
                {
                    heavyAttackHitbox.clearHitboxServerRpc();
                }
                if (timer > 1.7f - 0.25f)
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
                canSpacialAttack = false;
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

            default:
                state.Value = "";
                timer = 0;
                canMove = true;
                canSpacialAttack = true;
                canUltimate = true;
                canTurn = true;
                canAttack = true;
                canDodge = true;
                if (isHoldingSpecial && canSpacialAttack)
                {
                    state.Value = "SpecialCharge";
                    timer = 0;
                }
                break;
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

    public void specialAttackHold()
    {
        isHoldingSpecial = true;
    }
    public void specialAttackRelease()
    {
        isHoldingSpecial = false;
    }

    public void specialIsHeld()
    {
        //transform.position += new Vector3(0, 10*Time.deltaTime*transform.lossyScale.y, 0);
    }
}
