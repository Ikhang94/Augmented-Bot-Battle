using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class BaseBot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject opponent;
    public InputAction moveAction;
    public InputAction dodgeAction;
    public InputAction attack, heavyAttack;
    public List<BaseBot> bots, bothit = new List<BaseBot>();

    protected Rigidbody rb;
    public HitBox hitbox;
    public float moveSpd = 4;
    public int hp = 100;
    public int sp = 0;
    public int maxSp = 100;
    public int dmg = 5;
    public float scaling = 1.0f;
    public bool isPlayable = true;
    public string moveController = "Move";
    public string attackController = "Attack";
    public string HeavyattackController = "HeavyAttack";
    public string dodgeController = "Dodge";
    public string state;
    public Animator animator;
    public bool canMove, canTurn, canAttack, canHeavyAttack, canDodge, canUseSkill, canSpacialAttack, canUltimate = true;
    public bool inputDebug;
    public bool doAttack, doHeavyAttack = false;
    public Button attackButton, heavyAttackButton;
    public float timer = 0f;
    public Vector3 dodgeDirection, movementDirection;
    public bool attackInputBuffer, heavyAttackInputBuffer, attackJustPressed, heavyAttackJustPressed, invincible = false;
    public float baseMass;
    public List<GameObject> customButtons;
    void Awake()
    {
        moveAction = InputSystem.actions.FindAction(moveController);
        attack = InputSystem.actions.FindAction(attackController);
        heavyAttack = InputSystem.actions.FindAction(HeavyattackController);
        dodgeAction = InputSystem.actions.FindAction(dodgeController);
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;
        setCustomButtons();
        

        Input.simulateMouseWithTouches = true;
    }

    
    public void setCustomButtons()
    {
        foreach (var button in customButtons)
        {
            button.active = isPlayable;
        }
    }

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

    /// <summary>
    /// Function that makes the player look at their opponent
    /// </summary>
    /// <param name="parameter1">None</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    protected void LockOn()
    {
        if (canTurn && opponent != null)
        {
            Vector3 targetTransform = opponent.transform.position;
            targetTransform.y = transform.position.y;
            transform.LookAt(targetTransform);
        }
    }

    UnityEngine.Touch touchInput;
    Vector2 firstTouchPos;
    Vector2 lastTouchPos;
    public Vector2 moveValue;

    public void AssignedOpponent(BaseBot bot)
    {
        this.opponent = bot.GameObject();
    }

    /// <summary>
    /// Function that allows the player to move
    /// </summary>
    /// <param name="parameter1">None</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>

    bool isWalking = false;

    protected void Move()
    {
        rb.mass = baseMass * transform.lossyScale.y;
        rb.useGravity = false;
        rb.linearVelocity -= new Vector3(0, baseMass * 2 * transform.lossyScale.y * Time.deltaTime, 0);
        if (moveController != null && canMove)
        {
            if(isPlayable)
            {
                moveValue = moveAction.ReadValue<Vector2>();
            }

            if (Input.touchCount > 0 && isPlayable)
            {
                touchInput = Input.GetTouch(0);
                if (touchInput.phase == UnityEngine.TouchPhase.Began) //check for the first touch
                {
                    firstTouchPos = touchInput.position;
                    lastTouchPos = touchInput.position;
                }
                else if (touchInput.phase == UnityEngine.TouchPhase.Ended)
                {
                    lastTouchPos = firstTouchPos;
                }
                else
                {
                    lastTouchPos = touchInput.position;
                }
                moveValue = (lastTouchPos - firstTouchPos);
            }
            transform.position += transform.TransformDirection(new Vector3(moveValue.x, 0, moveValue.y).normalized * moveSpd * transform.lossyScale.x * Time.deltaTime);
            if (animator != null && moveValue != new Vector2(0, 0))
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Armature|Walk") && !animator.IsInTransition(0))
                {
                    animator.CrossFadeInFixedTime("Base Layer.Armature|Walk",0.1f);
                }
            }
            else if (animator != null)
            {
                
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Armature|Idle") && !animator.IsInTransition(0))
                    animator.CrossFadeInFixedTime("Base Layer.Armature|Idle", 0.1f);
            }
        }

        if (state != "hurt")
        {
            scaling = 1;
        }
    }

    /// <summary>
    /// Overridable function that controls the players state
    /// </summary>
    /// <param name="parameter1">None</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    virtual protected void StateMachine()
    {
        switch (state)
        {
            case "dodge":
                canMove = false;
                canTurn = false;
                canAttack = false;
                canDodge = false;
                transform.position += transform.TransformDirection(dodgeDirection * transform.lossyScale.y * (timer * 10) * Time.deltaTime);
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
                transform.position += transform.TransformDirection(new Vector3(0, 0, timer * 10f) * Time.deltaTime);
                timer -= Time.deltaTime;
                if (animator != null)
                {
                    animator.Play("Base Layer.Armature|Attack");
                }

                hitbox.hit();

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

    /// <summary>
    /// Function that makes the player attack
    /// </summary>
    /// <param name="parameter1">None</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    protected void AttackAction()
    {
        bool doAttackAction = (attack != null && attackJustPressed) || doAttack;
        if (doAttackAction && state == "idle")
        {
            state = "attack";
            timer = 0.5f;
        }

    }

    /// <summary>
    /// Function that makes the player do a heavy attack
    /// </summary>
    /// <param name="parameter1">None</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public virtual void HeavyAttackAction()
    {
        bool doAttackAction = (heavyAttack != null && heavyAttackJustPressed) || doHeavyAttack;
        if (doHeavyAttack && canHeavyAttack)
        {
            Debug.Log("heavy attacking: " + canHeavyAttack + heavyAttackJustPressed + heavyAttack.IsPressed());
            state = "heavy attack";
            timer = 1.7f;
        }
    }

    /// <summary>
    /// Function that decreases the health of the player and sets the time the player is in hitstun
    /// </summary>
    /// <param name="dmg">int parameter that defines the damage received by the player</param>
    /// <param name="hitstun">float parameter that defines how much time the player takes before being active</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void Hurt(int dmg, float hitstun)
    {
        timer = hitstun * scaling;
        hp -= (int)(dmg * scaling);
        state = "hurt";
        animator.Play("Base Layer.Armature|Hurt",-1,0f);
    }

    /// <summary>
    /// Function that calls the Hurt function when being attacked
    /// </summary>
    /// <param name="dmg">int parameter that defines the damage received by the player</param>
    /// <param name="hitstun">float parameter that defines how much time the player takes before being active</param>
    /// <param name="knockback">Vector3 function that defines how far and which direction the player is sent at when hit</param>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void Hurt(int dmg, float hitstun, Vector3 knockback)
    {
        Hurt(dmg, hitstun);
        rb.linearVelocity = knockback;
    }


    public bool dodgeInput;
    public void doDodge()
    {
        dodgeInput = true;
    }
    public void Dodge()
    {
        if ((dodgeAction != null && canDodge && dodgeAction.IsPressed()) || dodgeInput)
        {
            state = "dodge";
            timer = 0.5f;
            //Vector2 moveValue = moveAction.ReadValue<Vector2>();
            dodgeDirection = (new Vector3(moveValue.x, 0, moveValue.y).normalized * moveSpd);
            if (Vector3.Dot(dodgeDirection.normalized,Vector3.forward) > 0.5)
            {
                animator.Play("Base Layer.dodge forward", -1, normalizedTime: 0f);
                Debug.Log("dodging forward ");
            }
            else if (Vector3.Dot(dodgeDirection.normalized, Vector3.back) > 0.5)
            {
                animator.Play("Base Layer.dodge back",-1, normalizedTime:0f);
                Debug.Log("dodging back");
            }
            else if (Vector3.Dot(dodgeDirection.normalized, Vector3.right) > 0.5)
            {
                animator.Play("Base Layer.dodge right", -1, normalizedTime: 0f);
                Debug.Log("dodging right");
            }
            else if (Vector3.Dot(dodgeDirection.normalized, Vector3.left) > 0.5)
            {
                animator.Play("Base Layer.dodge left", -1, normalizedTime: 0f);
                Debug.Log("dodging left");
            }
            Debug.Log("dodge: " + Vector3.Dot(dodgeDirection.normalized, Vector3.forward));

        }
        dodgeInput = false;
    }

    /// <summary>
    /// Function that allows the user to attack from an external call source (such as a button)
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void activateDoAttack()
    {
        doAttack = true;
    }

    /// <summary>
    /// Function that allows the user to do a heavy attack from an external call source (such as a button)
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    public void activateDoHeavyAttack()
    {
        doHeavyAttack = true;
    }

    public void Skill()
    {

    }

    public void SpecialAttack()
    {

    }

    public void UltimateAttack()
    {
        //transform.position += new Vector3(0, 10, 0);
        if (canUltimate && sp>50)
        {
            addSp(-50);
            state = "ultimate";
            timer = 0;
        }
    }
    

    public int GetHp()
    {
        return hp;
    }

    public void SetHp(int hp)
    {
        this.hp = hp;
    }

    public int GetDmg()
    {
        return dmg;
    }

    public void SetDmg(int dmg)
    {
        this.dmg = dmg;
    }

    public float GetMoveSpd()
    {
        return moveSpd;
    }

    public void SetMoveSpd(float moveSpd)
    {
        this.moveSpd = moveSpd;
    }

    public void setSp(int value)
    {
        if (value > maxSp)
        {
            sp = maxSp;
        }
        else if (value < 0)
        {
            sp = 0;
        }
        else
        {
            sp = value;
        }
    }

    public void addSp(int value)
    {
        sp += value;
        if (sp > maxSp)
        {
            sp = maxSp;
        }
        else if (sp < 0)
        {
            sp = 0;
        }
    }

    /// <summary>
    /// Function that avoid having the player attack on every frame
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception conditions</exception>
    protected void attackInputBuffering()
    {
        if (attack != null && attack.IsPressed() && attackInputBuffer)
        {
            attackJustPressed = false;
        }
        else if (attack != null && attack.IsPressed())
        {
            attackJustPressed = true;
            attackInputBuffer = true;
        }
        else
        {
            attackJustPressed = false;

            attackInputBuffer = false;
        }

        if (heavyAttack != null && heavyAttack.IsPressed() && heavyAttackInputBuffer)
        {
            Debug.Log("heavyPressed");
            heavyAttackJustPressed = false;
        }
        else if (heavyAttack != null && heavyAttack.IsPressed())
        {
            Debug.Log("heavyJustPressed");
            heavyAttackJustPressed = true;
            heavyAttackInputBuffer = true;
        }
        else
        {

            heavyAttackJustPressed = false;
            heavyAttackInputBuffer = false;
        }

        doAttack = false;
        doHeavyAttack = false;
    }
}


