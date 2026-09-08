using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class ServerPlayerMove : NetworkBehaviour
{


    public override void OnNetworkSpawn()
    {

        // Only execute on the Server
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        SpawnPlayer();
        base.OnNetworkSpawn();
    }
    void SpawnPlayer()
    {
        Debug.Log(SpawnManager.SpawnPoints.Count);
        var spawnPosition = SpawnManager.SpawnPoints[Random.Range(0, SpawnManager.SpawnPoints.Count - 1)];
        transform.position = spawnPosition.transform.position;
    }
}
