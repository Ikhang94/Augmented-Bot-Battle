/// Fichier : BuyHandler.cs
/// Auteur : CAO Thien-Khang
/// Date : 25/05/2026
/// Description : Handles the robot purchase system,
///               sends purchase requests to the API,
///               and updates the player collection UI.
/// Version : 1.0
/// Derni�re modification : 26/05/2026

using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuyHandler : MonoBehaviour
{
    // ID of the robot to purchase
    public int robotIdBuy;

    // Base API URL
    private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";

    // Buy confirmation button
    public Button buttonBuy;

    // Robot detail popup window
    public GameObject robotDetailBuy;

    // Reference to the main robot shop button
    public Button robotBuyButton;

    // Reference to the collection manager
    public CollectionRobotsHandler collectionHandler;

    // Collection UI object
    public GameObject collectionRobot;

    /// <summary>
    /// Initializes the buy button listener.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="NullReferenceException">
    /// Triggered if the button reference is missing.
    /// </exception>
    void Start()
    {
        buttonBuy.onClick.AddListener(BuyRobot);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Starts the robot purchase coroutine.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if the purchase process fails.
    /// </exception>
    void BuyRobot() => StartCoroutine(BuyRobot_Couroutine());

    /// <summary>
    /// Sends a robot purchase request to the backend API
    /// and updates the player collection UI.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    /// <exception cref="UnityWebRequestException">
    /// Triggered if the API request fails.
    /// </exception>
    IEnumerator BuyRobot_Couroutine()
    {
        // Create player-robot purchase DTO
        PlayersRobotDTO playerRobot = new PlayersRobotDTO();

        playerRobot.playerId =
            (int)UserTokenService.Instance.selfPlayer.PlayerID;

        playerRobot.robotId = robotIdBuy;

        // Convert DTO to JSON
        string json = JsonUtility.ToJson(playerRobot);

        Debug.Log(json);
        Debug.Log(URL + "/users/playerAcquireRobot");

        // Send POST request to the API
        using (UnityWebRequest www =
            UnityWebRequest.Post(
                URL + "/users/playerAcquireRobot",
                json,
                "application/json"))
        {
            yield return www.SendWebRequest();

            // Check request result
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                // Refresh collection UI
                collectionRobot.SetActive(true);

                collectionHandler.RefreshRobots();

                // Close detail popup
                robotDetailBuy.SetActive(false);

                // Disable buy button after purchase
                robotBuyButton.interactable = false;
            }
        }
    }
}