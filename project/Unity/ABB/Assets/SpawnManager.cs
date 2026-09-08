using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //Chatpgt code that may need to be rework

    public static List<Transform> SpawnPoints;
    [SerializeField] private List<Transform> _spawnPoints;

    private void Awake()
    {
        SpawnPoints = _spawnPoints; 
    }

}
