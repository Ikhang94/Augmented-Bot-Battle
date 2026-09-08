using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServerPlayerSpawn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<GameObject> m_SpawnPoints;
    public GameObject GetRandomSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0)
            return null;
        return m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
    }
}
