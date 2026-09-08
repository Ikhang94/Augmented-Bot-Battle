using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BaseBotNetwork : NetworkBehaviour
{
    public GameObject opponent;
    public InputAction moveAction;
    public InputAction dodgeAction;
    public InputAction attack, heavyAttack;
    public List<BaseBotNetwork> bots, bothit = new List<BaseBotNetwork>();

    protected Rigidbody rb;
    public HitBoxNetwork hitbox;
    public float moveSpd = 4;
    public NetworkVariable<int> hp = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //public NetworkVariable<float> timer = new NetworkVariable<float>(0);
    public NetworkVariable<FixedString32Bytes> state = new NetworkVariable<FixedString32Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int instanceHp;
    public string instanceState;
    public int dmg = 5;
    public float scaling = 1.0f;
    public string moveController = "Move";
    public string attackController = "Attack";
    public string HeavyattackController = "HeavyAttack";
    public string dodgeController = "Dodge";
    public Animator animator;
    public bool canMove = true, canTurn = true, canAttack = true, canHeavyAttack = true, canDodge = true, canUseSkill = true, canSpacialAttack = true, canUltimate = true;
    public bool inputDebug;
    public bool doAttack, doHeavyAttack = false;
    public Button attackButton, heavyAttackButton;
    public float timer = 0f;
    public Vector3 dodgeDirection, movementDirection;
    public bool attackInputBuffer, heavyAttackInputBuffer, attackJustPressed, heavyAttackJustPressed= false;
    public NetworkVariable<bool> invincible = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float baseMass;
    public ulong clientId, networkId;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction(moveController);
        attack = InputSystem.actions.FindAction(attackController);
        heavyAttack = InputSystem.actions.FindAction(HeavyattackController);
        dodgeAction = InputSystem.actions.FindAction(dodgeController);
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;

        Input.simulateMouseWithTouches = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) { return; }
        LockOn();
        StateMachineRPC();
        Move();
        AttackAction();
        HeavyAttackAction();
        Dodge();
        attackInputBuffering();
    }

    public override void OnNetworkSpawn()
    {
        enabled = true;
        transform.position = new Vector3(0, 1, 0);
        if (IsOwner)
            gameObject.name += "(You)";
        else
            gameObject.name += "(Not you)";
            try
            {
                GameObject.FindAnyObjectByType<GameManagerNetwork>().addPlayer(this);

            }
            catch(Exception e)
            {
                Debug.LogWarning("There is no network game manager , players will not be able to lock on to eachother: " + e.Message);
            }
        if (IsOwner)
        {
            Button attackBtn = GameObject.Find("Attack").GetComponent<Button>();
            attackBtn.onClick.AddListener(activateDoAttack);
            Button heavyAttackButton = GameObject.Find("HeavyAttack").GetComponent<Button>();
            heavyAttackButton.onClick.AddListener(activateDoHeavyAttack);
            Button dodgeButton = GameObject.Find("Dodge").GetComponent<Button>();
            dodgeButton.onClick.AddListener(doDodge);

        }
        hp.OnValueChanged += OnHpChanged;
        state.OnValueChanged += OnStateChange;
        clientId = OwnerClientId;
        networkId = NetworkObjectId;
    }

    private void OnHpChanged(int oldValue, int newValue)
    {
        UpdateHp(newValue);
    }
    private void OnStateChange(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        UpdateState(newValue.ToString());
    }

    private void UpdateHp(int newValue)
    {
        instanceHp = newValue;
    }
    private void UpdateState(string newValue)
    {
        instanceState = newValue;
    }


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

    public void AssignedOpponent(BaseBotNetwork bot)
    {
        this.opponent = bot.GameObject();
    }
    protected void Move()
    {
        rb.mass = baseMass * transform.lossyScale.y;
        rb.useGravity = false;
        rb.linearVelocity -= new Vector3(0, baseMass * 2 * transform.lossyScale.y * Time.deltaTime, 0);
        if (moveAction != null && canMove)
        {
            moveValue = moveAction.ReadValue<Vector2>();

            if (Input.touchCount > 0 && IsOwner)
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
                animator.Play("Base Layer.Armature|Walk");
            }
            else if (animator != null)
            {
                animator.Play("Base Layer.Armature|Idle");
            }
        }
        if (state.Value != "hurt")
        {
            scaling = 1;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    virtual public void StateMachineRPC()
    {
        switch (instanceState)
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
                transform.position += transform.TransformDirection(new Vector3(0, 0, timer * 10f) * Time.deltaTime);
                timer -= Time.deltaTime;
                if (animator != null)
                {
                    animator.Play("Base Layer.Armature|Attack");
                }

                //foreach (BaseBot bot in getBothit())
                //{
                //    bot.Hurt(dmg, 2, transform.TransformDirection(new Vector3(0, 5, 1)));
                //}
                hitbox.hit();

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
        /*if (state == "attack" && timer>2f)
        {
            state = "";
            timer = 0;
            canMove = true;
            canTurn = true;
        }
        if (state == "attack")
        {
            
        }*/
    }
    protected void AttackAction()
    {
        bool doAttackAction = (attack != null && attackJustPressed) || doAttack;
        if (doAttackAction && state.Value == "")
        {
            state.Value = "attack";
            timer = 0.5f;
        }

    }
    public virtual void HeavyAttackAction()
    {
        bool doAttackAction = (heavyAttack != null && heavyAttackJustPressed) || doHeavyAttack;
        if (doHeavyAttack && canHeavyAttack)
        {
            Debug.Log("heavy attacking: " + canHeavyAttack + heavyAttackJustPressed + heavyAttack.IsPressed());
            state.Value = "heavy attack";
            timer = 1.7f;
        }
    }

    public void Hurt(int dmg, float hitstun)
    {
        timer = hitstun * scaling;
        hp.Value -= (int)(dmg * scaling);
        state.Value = "hurt";
        animator.Play("Base Layer.Armature|Hurt", -1, 0f);
    }
    [Rpc(SendTo.Everyone)]
    public void HurtRPC(int dmg, float hitstun, Vector3 knockback, float scalingDecrease)
    {
        if (!IsOwner) return;
        if(!invincible.Value)
        {
            Hurt(dmg, hitstun);
            rb.linearVelocity = knockback;
            scaling -= scalingDecrease;
            if (scaling < 0.1f)
            { 
                scaling = 0.1f;
            }

        }

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
            state.Value = "dodge";
            timer = 0.5f;
            //Vector2 moveValue = moveAction.ReadValue<Vector2>();
            dodgeDirection = (new Vector3(moveValue.x, 0, moveValue.y).normalized * moveSpd);
            if (Vector3.Dot(dodgeDirection.normalized, Vector3.forward) > 0.5)
            {
                animator.Play("Base Layer.dodge forward", -1, normalizedTime: 0f);
                Debug.Log("dodging forward ");
            }
            else if (Vector3.Dot(dodgeDirection.normalized, Vector3.back) > 0.5)
            {
                animator.Play("Base Layer.dodge back", -1, normalizedTime: 0f);
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

    public void activateDoAttack()
    {
        doAttack = true;
    }

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

    }
    public List<BaseBotNetwork> getBothit()
    {
        //Collider detectedColliders = Physics.OverlapBox(colHitbox);

        List<BaseBotNetwork> botList = new List<BaseBotNetwork>();

        foreach (BaseBotNetwork bot in bots)
        {
            if (bot != null && !bothit.Contains(bot))
            {
                botList.Add(bot);
                bothit.Add(bot);
            }
        }

        return botList;
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseBotNetwork otherComponent = other.GetComponent<BaseBotNetwork>();
        if (otherComponent != null)
        {
            bots.Add(otherComponent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BaseBotNetwork otherComponent = other.GetComponent<BaseBotNetwork>();
        if (otherComponent != null)
        {
            bots.Remove(otherComponent);
        }
    }

    public int GetHp()
    {
        return hp.Value;
    }

    public void SetHp(int hp)
    {
        this.hp.Value = hp;
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
