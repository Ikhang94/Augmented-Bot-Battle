using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerNetwork : NetworkManager
{
    public static GameManagerNetwork Instance { get; private set; }
    public Button lightAttackBtn, heavyAttackBtn, dodgeBtn;
    private float timer = 500f;
    public BaseBotNetwork[] gameObjects;
    public BaseBotNetwork p1Bot, p2Bot, p1, p2;
    public TextMeshProUGUI timerDisplay, victoryText;
    Vector3 spawnpoint1 = new Vector3(0.5f,1,0), spawnpoint2 = new Vector3(-0.5f, 1, 0);
    public bool startingRound = false;
    public HpBar hp1, hp2;
    public CharacterSelect charaSelect;
    public List<BaseBotNetwork> robots;

    private void Awake()
    {
        hp1 = GameObject.Find("hp1").GetComponent<HpBar>();
        hp2 = GameObject.Find("hp2").GetComponent<HpBar>();
        charaSelect = GameObject.Find("CharacterChoice").GetComponent<CharacterSelect>();
        victoryText = GameObject.Find("victory").GetComponent<TextMeshProUGUI>();
        victoryText.gameObject.active = false;
        timerDisplay = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        Cleanup();
        //OnFetchLocalPlayerPrefabToSpawn();
    }

    public void Start()
    {
    }
    void Update()
    {
        if (p1 != null && p2 != null)
        {
            startingRound = true;
            CheckLifeRobot();
            CheckTimer();
        }
    }
    void Cleanup()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }

    //[Rpc(SendTo.Server)]
    [ServerRpc(RequireOwnership = false)]
    public void spawnPlayerServerRPC(GameObject player, ulong id)
    {
        //if (IsServer)
        Debug.Log("spawned: "+player.name);
        GameObject playerObject = Instantiate(player, transform.parent).gameObject;
            playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
        if (p2 == null)
        playerObject.transform.position = new Vector3(0, 2, 0);
    }
    
    public void spawnPlayerClientRPC(GameObject player)
    {
        spawnPlayerServerRPC(player, LocalClientId);
    }

    public void quit()
    {
        AuthenticationService.Instance.SignOut(true);
        Destroy(gameObject);
    }

    public void spawnPlayer()
    {
        NetworkObject player = Instantiate(charaSelect.selectedBot.gameObject.GetComponent<NetworkObject>());
        player.SpawnAsPlayerObject(LocalClientId, true);
    }


    public void checkWinCon()
    {
        //Shutdown();
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
            if (p1.GetHp() > p2.GetHp())
            {
                victoryText.text = "p1 has won!";
            }
            else if (p2.GetHp() > p1.GetHp())
            {
                victoryText.text = "p2 has won!";
            }
            else
            {
                victoryText.text = "draw";
            }
        }
        
    }
    private void CheckTimer()
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
    private void CheckLifeRobot()
    {
        if (p1.GetHp() <= 0 || p2.GetHp() <= 0)
        {
            checkWinCon();
            Time.timeScale = 0.2f;

        }
    }
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

    public void addPlayer(BaseBotNetwork bot)
    {
        bot.transform.position = new Vector3(0, 2, 0);
        if (p1 == null)
        {
            p1 = bot;
            p1.transform.position = new Vector3(0,2,0);
        }
        else
        {
            p2 = bot;
            p2.transform.position = new Vector3(0, 2, 0);
        }
        //if (bot.IsServer)
        //{
        //    p1=bot;
        //}
        //else
        //{
        //    p2=bot;
        //}
        setHpbars();
        if (p1 != null && p2 != null)
        {
            p1.opponent = p2.gameObject;
            p2.opponent = p1.gameObject;

            p1.transform.position = spawnpoint1;
            p2.transform.position = spawnpoint2;
            p1.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            p2.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            Debug.Log($"p1: {p1.name} : {p1.transform.position} : {spawnpoint1}\np2: {p2.name} : {p2.transform.position} : {spawnpoint2}");
        }
    }


}

