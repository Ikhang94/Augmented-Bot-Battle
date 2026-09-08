/// Fichier : StartGameButton.cs
/// Auteur : CAO Thien-Khang
/// Date : 23/05/2026
/// Description : Handles the start game button behavior,
///               checks player authentication,
///               and opens the correct UI menu.
/// Version : 1.0
/// Dernière modification : 26/05/2026

using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    // Start game button
    public Button startButton;

    // Login card UI
    public GameObject loginCard;

    // Hub menu displayed after login
    public GameObject hubMenu;

    // Animator used for UI transitions
    public Animator animator;

    /// <summary>
    /// Initializes the start button listener.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="NullReferenceException">
    /// Triggered if the button reference is missing.
    /// </exception>
    void Start()
    {
        startButton.onClick.AddListener(HandleStartButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Handles the start button behavior.
    /// Opens the login menu if the player is not connected,
    /// otherwise opens the hub menu.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if UI references are missing.
    /// </exception>
    private void HandleStartButton()
    {
        // Check if the player is logged in
        if (UserTokenService.Instance.selfPlayer.PlayerID == null)
        {
            // Open login menu
            loginCard.SetActive(true);
        }
        else
        {
            // Open hub menu
            hubMenu.SetActive(true);

            // Play UI transition animation
            animator.Play("Base Layer.Start");
        }
    }
}