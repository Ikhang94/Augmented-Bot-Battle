using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkSpawner : NetworkBehaviour

{
    public List<BaseBotNetwork> robots;
    public BaseBotNetwork chosenBot;
    public NetworkObject no;
    public ulong clientId, networkId;
    public  NetworkVariable<int>  player = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public override void OnNetworkSpawn()
    {
        clientId = OwnerClientId;
        networkId = NetworkObjectId;

        //if (!IsOwner) return;
        chosenBot = GameObject.Find("CharacterChoice").GetComponent<CharacterSelect>().selectedBot;
        chosenBot.enabled = true;
        if (IsOwner)
        {
            no = gameObject.GetComponent<NetworkObject>();
            player.Value = robots.IndexOf(chosenBot);

            Debug.Log($"{player.Value} Host:{IsHost} Client:{IsClient}");
}
    }

    void Awake()
    {
        Debug.Log("Awake index: " + player.Value);
        player.OnValueChanged += (int oldValue, int newValue) =>
        {
            Debug.Log($"value update {oldValue} : {newValue}");
            chosenBot = robots[newValue];
            //Instantiate(robots[player.Value], transform.parent).GetComponent<NetworkObject>().Spawn(AsPlayerObject(OwnerClientId, true));
            if (IsHost && IsOwner)
                NetworkManager.Singleton.GetComponent<GameManagerNetwork>().spawnPlayerServerRPC(robots[newValue].gameObject, OwnerClientId);
            else if (IsClient && IsOwner)
            {
                spawnPlayerServerRpc(newValue);
            }
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
            [ServerRpc(RequireOwnership =false)]
            public void spawnPlayerServerRpc(int index)
    {
        NetworkManager.Singleton.GetComponent<GameManagerNetwork>().spawnPlayerServerRPC(robots[index].gameObject, OwnerClientId);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


