/// Fichier : LoginButton.cs
/// Auteur : CAO Thien-Khang
/// Date : 23/05/2026
/// Description : Handles the player login system,
///               sends authentication requests to the API,
///               and opens the main hub menu after login.
/// Version : 1.1
/// Derni�re modification : 26/05/2026

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginButton : MonoBehaviour
{
    // Base API URL
    private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";

    [Header("Inputs")]

    // Username/email input field
    public TMP_InputField usernameInput;

    // Password input field
    public TMP_InputField passwordInput;

    [Header("UI")]

    // Error message text
    public TMP_Text errorText;

    [Header("Button")]

    // Login button
    public Button buttonLogin;

    // Start menu object
    public GameObject menuDemarrage;

    // Hub menu object displayed after login
    public GameObject hubMenu;

    // Animator used for UI transitions
    public Animator animator;

    /// <summary>
    /// Initializes the login button listener.
    /// </summary>
    void Start()
    {
        buttonLogin.onClick.AddListener(Login);

        // Hide error message at start
        errorText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts the login coroutine.
    /// </summary>
    void Login() => StartCoroutine(Login_Couroutine());

    /// <summary>
    /// Sends login credentials to the backend API
    /// and authenticates the player.
    /// </summary>
    IEnumerator Login_Couroutine()
    {
        // Clear previous error
        errorText.gameObject.SetActive(false);

        // Check empty fields
        if (string.IsNullOrWhiteSpace(usernameInput.text) ||
            string.IsNullOrWhiteSpace(passwordInput.text))
        {
            yield return ShowError("Veuillez remplir tous les champs.");
            yield break;
        }

        // Create player login DTO
        PlayerDTO user = new PlayerDTO
        {
            Email = usernameInput.text,
            Password = passwordInput.text
        };

        // Convert DTO to JSON
        string json = JsonUtility.ToJson(user);

        Debug.Log(json);

        // Send login POST request
        using (UnityWebRequest www =
            UnityWebRequest.Post(
                URL + "/users/login",
                json,
                "application/json"))
        {
            yield return www.SendWebRequest();

            // Check request result
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);

                yield return ShowError(
                    "Erreur de connexion au serveur.");
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);

                // Deserialize API response
                UserToken response =
                    JsonUtility.FromJson<UserToken>(
                        www.downloadHandler.text);

                // Check token validity
                if (response == null ||
                    string.IsNullOrEmpty(response.result))
                {
                    yield return ShowError(
                        "Email ou mot de passe incorrect.");
                    yield break;
                }

                Debug.Log(response.result);

                // Decode JWT token payload
                UserTokenService.Instance
                    .DecodePayload(response.result);

                // Open hub menu after successful login
                hubMenu.SetActive(true);

                // Play UI transition animation
                animator.Play("Base Layer.Start");
            }
        }
    }

    /// <summary>
    /// Displays an error message temporarily.
    /// </summary>
    /// <param name="message">Message to display</param>
    IEnumerator ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        errorText.gameObject.SetActive(false);
    }
}