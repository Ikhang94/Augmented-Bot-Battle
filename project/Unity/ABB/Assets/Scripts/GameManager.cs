using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Instance statique accessible partout
    public static GameManager Instance { get; private set; }
    public Button lightAttackBtn, heavyAttackBtn, dodgeBtn;
    private float timer = 500f;
    public BaseBot[] gameObjects;
    public BaseBot p1Bot, p2Bot, p1, p2;
    public HpBar hp1, hp2;
    public TextMeshProUGUI timerDisplay, victoryText;
    public Vector3 spawnpoint1,spawnpoint2;
    public bool startingRound = false;
    public CharacterSelectSolo charaSelect;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1;
        getCharacters();
        gameObjects = GameObject.FindObjectsByType<BaseBot>(FindObjectsSortMode.None);
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
    }
    void Update()
    {
        if (p1!=null && p2!=null)
        {
            CheckLifeRobot();
            CheckTimer();
        }
        if (startingRound && p1Bot != null && p2Bot != null)
        {
            spawnPlayers();
            startingRound = false;
        }
    }
    /// <summary>
    /// Function that checks and decreases the timer and defines a winner if it's at 0
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    private void CheckTimer ()
    {
        if (timerDisplay != null)
        {
            timerDisplay.text = ((int)timer).ToString();
        }
        if (timer <= 0)
        {
            Time.timeScale = 0.2f;
            checkWinCon();
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Function that checks the players health and defines a winner if a player is at 0 hp
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    private void CheckLifeRobot()
    {
        if (p1.GetHp() <= 0 || p2.GetHp() <= 0)
        {
                checkWinCon();
            Time.timeScale = 0.2f;

        }
    }
    /// <summary>
    /// Function that spawns players and sets their values depending on if they're playable
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    public void spawnPlayers()
    {
        if (p1 == null && p1Bot != null)
        {
            p1 = Instantiate(p1Bot, transform.parent);
            p1.state = "idle";
            p1.isPlayable = true;
            p1.setCustomButtons();
            lightAttackBtn.onClick.AddListener( p1.activateDoAttack);
            heavyAttackBtn.onClick.AddListener ( p1.activateDoHeavyAttack);
            dodgeBtn.onClick.AddListener(p1.doDodge);
            p1.transform.localPosition= spawnpoint1;
            Debug.Log(spawnpoint1 + transform.position);
            Debug.Log(p1.transform.position);
            
            //p1.transform.localScale = transform.lossyScale;
            if (p2 != null)
            {
                p1.opponent = p2.GameObject();
                p2.opponent = p1.GameObject();
            }
        }
        if (p2 == null && p2Bot != null)
        {
            p2 = Instantiate(p2Bot, transform.parent);
            p2.moveController = "";p2.dodgeController = "";p2.dodgeController = "";p2.attackController = "";p2.HeavyattackController = "";
            p2.state = "idle";
            p2.isPlayable = false;
            p2.setCustomButtons();
            p2.transform.localPosition = spawnpoint2;
            Debug.Log(spawnpoint2 + transform.position);
            Debug.Log(p2.transform.position);
            //p2.transform.localScale = transform.lossyScale;
            if (p1!= null)
            {
                p1.opponent = p2.GameObject();
                p2.opponent = p1.GameObject();
            }
        }
        setHpbars();
    }

    /// <summary>
    /// Function that sets both players health
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    public void setHpbars()
    {
        if (p1 != null && hp1 != null)
        {
            hp1.setPlayer(p1);
        }
        if (p2 != null && hp2 != null)
        {
            hp2.setPlayer(p2);
        }
    }

    /// <summary>
    /// Function that restarts the round and sets the timescale back to 1
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    public void resetGame()
    {
        Time.timeScale = 1;
        if (p1 != null)
        {
            Destroy(p1.gameObject);
            p1 = null;
        }
        if (p2 != null) 
        { 
            Destroy(p2.gameObject); 
            p2 = null;
        }
        timer = 500;
        victoryText.gameObject.SetActive(false);
        spawnPlayers();
        //p1.transform.position = spawnpoint1 + transform.position;
        //p2.transform.position = spawnpoint2 + transform.position;
    }
    /// <summary>
    /// Function that checks which player has won by comparing their health
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    public void checkWinCon()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
            if (p1.GetHp() > p2.GetHp())
            {
                victoryText.text = "p1 has won!";
            }
            else if (p2.GetHp()>p1.GetHp())
            {
                victoryText.text = "p2 has won!";
            }
            else
            {
                victoryText.text = "draw";
            }
        }
    }
    /// <summary>
    /// Function that gets which characters are selected in the character selector
    /// </summary>
    /// <returns>Void Function</returns>
    /// <exception cref="ExceptionType">Exception Conditions</exception>
    public void getCharacters()
    {
        try
        {
            if (charaSelect == null)
            {
                charaSelect = FindAnyObjectByType<CharacterSelectSolo>();
            }
            p1Bot = charaSelect.chosenBot1;
            p2Bot = charaSelect.chosenBot2;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error getting characters: " + e);
        }
    }

}

