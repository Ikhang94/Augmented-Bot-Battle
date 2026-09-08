/// Fichier : BoutiqueHandler.cs
/// Auteur : CAO Thien-Khang
/// Date : 25/05/2026
/// Description : Handles the robot shop system, API requests,
///               robot purchases, and dynamic UI generation.
/// Version : 1.0
/// Derni�re modification : 26/05/2026

using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoutiqueHandler : MonoBehaviour
{
    // Base API URL
    private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";

    // List of robots owned by the player
    private RobotList playerRobotList;

    // Prefab for the robot purchase button
    public GameObject robotBuy;

    // Prefab for robot detail popup
    public GameObject robotBuyDetail;

    // Reference to the collection manager
    public CollectionRobotsHandler collectionRobotsHandler;

    // Prefab used for collection robots
    public GameObject collectionRobot;

    // Parent container for dynamically created buttons
    public Transform robotsContainer;

    /// <summary>
    /// Initializes the shop system and starts API loading coroutines.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if initialization fails.
    /// </exception>
    void Start()
    {
        StartCoroutine(StartFunCouroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Main initialization coroutine.
    /// Retrieves player-owned robots and all available robots.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    /// <exception cref="Exception">
    /// Triggered if a web request fails.
    /// </exception>
    IEnumerator StartFunCouroutine()
    {
        yield return StartCoroutine(GetAllRobotsOwnedByPlayer_Couroutine());
        yield return StartCoroutine(GetAllRobots_Couroutine());
    }

    /// <summary>
    /// Retrieves all robots owned by the current player from the API.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    /// <exception cref="UnityWebRequestException">
    /// Triggered if the API request fails.
    /// </exception>
    IEnumerator GetAllRobotsOwnedByPlayer_Couroutine()
    {
        using (UnityWebRequest www =
            UnityWebRequest.Get(
                URL + "/users/GetPlayerRobots/" +
                UserTokenService.Instance.selfPlayer.PlayerID))
        {
            yield return www.SendWebRequest();

            // Check request result
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Retrieve JSON response
                string json = www.downloadHandler.text;

                // Wrap JSON array for JsonUtility compatibility
                string wrappedJson = "{ \"robots\": " + json + "}";

                // Deserialize robot list
                playerRobotList =
                    JsonUtility.FromJson<RobotList>(wrappedJson);
            }
        }
    }

    /// <summary>
    /// Retrieves all available robots from the API
    /// and creates a shop button for each one.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    /// <exception cref="UnityWebRequestException">
    /// Triggered if the API request fails.
    /// </exception>
    IEnumerator GetAllRobots_Couroutine()
    {
        using (UnityWebRequest www =
            UnityWebRequest.Get(URL + "/robots"))
        {
            yield return www.SendWebRequest();

            // Check request result
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // Retrieve JSON response
                string json = www.downloadHandler.text;

                Debug.Log(json);

                // Wrap JSON array for JsonUtility compatibility
                string wrappedJson = "{ \"robots\": " + json + "}";

                // Deserialize robot list
                RobotList robotList =
                    JsonUtility.FromJson<RobotList>(wrappedJson);

                Debug.Log(robotList);

                // Create a button for each robot
                foreach (RobotDTO robot in robotList.robots)
                {
                    Debug.Log(
                        $"ID: {robot.robotID} | Name: {robot.robotName}");

                    CreateRobotButton(robot);
                }
            }
        }
    }

    /// <summary>
    /// Creates the shop UI button and detail popup for a robot.
    /// </summary>
    /// <param name="robot">
    /// Robot data used to configure the UI.
    /// </param>
    /// <returns>Void function</returns>
    /// <exception cref="NullReferenceException">
    /// Triggered if a UI component is missing.
    /// </exception>
    private void CreateRobotButton(RobotDTO robot)
    {
        // Create the robot purchase button
        GameObject buttonRobotBuy =
            Instantiate(robotBuy, robotsContainer);

        Button button = buttonRobotBuy.GetComponent<Button>();

        // Retrieve robot button image
        Image imgRobotBuy =
            buttonRobotBuy.GetComponentInChildren<Image>();

        // Create the robot detail popup
        GameObject buttonRobotBuyDetail =
            Instantiate(robotBuyDetail);

        // Retrieve detail popup images
        Image imgRobotBuyDetail =
            buttonRobotBuyDetail.transform
            .Find("Stack/Image")
            .GetComponentInChildren<Image>();

        Image imgtBuyButton =
            buttonRobotBuyDetail.transform
            .Find("Stack/Button")
            .GetComponentInChildren<Image>();

        // Hide popup by default
        buttonRobotBuyDetail.SetActive(false);

        // Configure BuyHandler references
        buttonRobotBuyDetail
            .GetComponentInChildren<BuyHandler>()
            .robotIdBuy = robot.robotID;

        buttonRobotBuyDetail
            .GetComponentInChildren<BuyHandler>()
            .robotBuyButton = button;

        buttonRobotBuyDetail
            .GetComponentInChildren<BuyHandler>()
            .collectionHandler = collectionRobotsHandler;

        buttonRobotBuyDetail
            .GetComponentInChildren<BuyHandler>()
            .collectionRobot = collectionRobot;

        // Assign robot sprites depending on robot ID
        switch (robot.robotID)
        {
            case 1:
                imgRobotBuy.sprite =
                    Resources.Load<Sprite>("PumaBot_Buy");

                imgRobotBuyDetail.sprite =
                    Resources.Load<Sprite>("PumaBot_detail");

                imgtBuyButton.sprite =
                    Resources.Load<Sprite>("Buy_20");
                break;

            case 2:
                imgRobotBuy.sprite =
                    Resources.Load<Sprite>("RoninBot_Buy");

                imgRobotBuyDetail.sprite =
                    Resources.Load<Sprite>("RoninBot_detail");

                imgtBuyButton.sprite =
                    Resources.Load<Sprite>("Buy_30");
                break;

            case 3:
                imgRobotBuy.sprite =
                    Resources.Load<Sprite>("SamuraiBot_Buy");

                imgRobotBuyDetail.sprite =
                    Resources.Load<Sprite>("SamuraiBot_detail");

                imgtBuyButton.sprite =
                    Resources.Load<Sprite>("Buy_50");
                break;

            case 4:
                imgRobotBuy.sprite =
                    Resources.Load<Sprite>("KnightBot_Buy");

                imgRobotBuyDetail.sprite =
                    Resources.Load<Sprite>("KnightBot_detail");

                imgtBuyButton.sprite =
                    Resources.Load<Sprite>("Buy_40");
                break;
        }

        // Disable the button if the player already owns the robot
        if (playerRobotList != null &&
            playerRobotList.robots.Any(
                r => r.robotID == robot.robotID))
        {
            Debug.Log("disable");
            button.interactable = false;
        }

        // Open detail popup when button is clicked
        button.onClick.AddListener(() =>
        {
            Debug.Log("Selected robot: " + robot.robotName);

            buttonRobotBuyDetail.SetActive(true);
        });
    }
}