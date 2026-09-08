using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HubHandler : MonoBehaviour
{
    public TextMeshProUGUI userNameHeader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userNameHeader.text = UserTokenService.Instance.selfPlayer.UserName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
