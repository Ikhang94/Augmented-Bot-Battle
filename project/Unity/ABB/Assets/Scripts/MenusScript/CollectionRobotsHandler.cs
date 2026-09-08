/// Fichier : CollectionRobotsHandler.cs
/// Auteur : CAO Thien-Khang
/// Date : 24/05/2026
/// Description : Handles the player's robot collection,
///               retrieves owned robots from the API,
///               and dynamically creates collection UI elements.
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

public class CollectionRobotsHandler : MonoBehaviour
{
    // Base API URL
    private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";

    // Parent container for robot buttons
    public Transform robotsContainer;

    // Robot button prefab
    public GameObject robotButtonPrefab;

    // Robot detail popup prefab
    public GameObject robotDetail;

    // Robot list UI object
    public GameObject robotListUI;

    // Indicates if the collection is being refreshed
    private bool refresh = false;

    /// <summary>
    /// Initializes the collection system
    /// and retrieves the player's robots.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if initialization fails.
    /// </exception>
    void Start()
    {
        StartCoroutine(GetAllRobotsOwnedByPlayer_Couroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Refreshes the player's robot collection.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if refresh fails.
    /// </exception>
    public void RefreshRobots()
    {
        StartCoroutine(GetAllRobotsOwnedByPlayer_Couroutine());
    }

    /// <summary>
    /// Retrieves all robots owned by the player from the API
    /// and dynamically recreates the collection UI.
    /// </summary>
    /// <returns>IEnumerator coroutine</returns>
    /// <exception cref="UnityWebRequestException">
    /// Triggered if the API request fails.
    /// </exception>
    IEnumerator GetAllRobotsOwnedByPlayer_Couroutine()
    {
        // Remove old robot buttons
        foreach (Transform child in robotsContainer)
        {
            Destroy(child.gameObject);
            refresh = true;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(
            URL + "/users/GetPlayerRobots/" +
            UserTokenService.Instance.selfPlayer.PlayerID))
        {
            yield return www.SendWebRequest();

            // Check request result
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                refresh = false;
            }
            else
            {
                // Retrieve JSON response
                string json = www.downloadHandler.text;

                // Wrap JSON array for JsonUtility compatibility
                string wrappedJson = "{ \"robots\": " + json + "}";

                // Deserialize robot list
                RobotList robotList =
                    JsonUtility.FromJson<RobotList>(wrappedJson);

                // Create a button for each robot
                foreach (RobotDTO robot in robotList.robots)
                {
                    CreateRobotButton(robot);
                }

                // Hide robot list UI after refresh
                if (refresh == true)
                {
                    refresh = false;
                    robotListUI.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Creates a collection button and detail popup
    /// for a robot owned by the player.
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
        // Create robot button
        GameObject newButton =
            Instantiate(robotButtonPrefab, robotsContainer);

        // Retrieve robot name text
        TMP_Text text =
            newButton.GetComponentInChildren<TMP_Text>();

        // Create robot detail popup
        GameObject newRobotDetail =
            Instantiate(robotDetail);

        newRobotDetail.SetActive(false);

        // Set robot name
        if (text != null)
        {
            text.text = robot.robotName;
        }

        // Retrieve button component
        Button button =
            newButton.GetComponent<Button>();

        // Retrieve robot images
        Image imgRobot =
            newButton.GetComponentInChildren<Image>();

        Image imgRobotDetail =
            newRobotDetail.transform
            .Find("Stack/Image")
            .GetComponent<Image>();

        // Assign robot sprites depending on robot ID
        switch (robot.robotID)
        {
            case 1:
                imgRobot.sprite =
                    Resources.Load<Sprite>("PumaBot_portrait");

                imgRobotDetail.sprite =
                    Resources.Load<Sprite>("PumaBot_detail");
                break;

            case 2:
                imgRobot.sprite =
                    Resources.Load<Sprite>("RoninBot_portrait");

                imgRobotDetail.sprite =
                    Resources.Load<Sprite>("RoninBot_detail");
                break;

            case 3:
                imgRobot.sprite =
                    Resources.Load<Sprite>("SamuraiBot_portrait");

                imgRobotDetail.sprite =
                    Resources.Load<Sprite>("SamuraiBot_detail");
                break;

            case 4:
                imgRobot.sprite =
                    Resources.Load<Sprite>("KnightBot_portrait");

                imgRobotDetail.sprite =
                    Resources.Load<Sprite>("KnightBot_detail");
                break;
        }

        // Open detail popup when robot button is clicked
        button.onClick.AddListener(() =>
        {
            Debug.Log("Robot selected : " + robot.robotName);

            newRobotDetail.SetActive(true);
        });
    }
}