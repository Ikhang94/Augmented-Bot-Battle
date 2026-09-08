/// Fichier : UserTokenService.cs
/// Auteur : CAO Thien-Khang
/// Date : 23/05/2026
/// Description : Handles JWT token decoding,
///               stores player authentication data,
///               and provides a persistent singleton service.
/// Version : 1.0
/// Dernière modification : 24/05/2026

using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

public class UserTokenService : MonoBehaviour
{
    // Singleton instance
    public static UserTokenService Instance { get; private set; }

    // Stores authenticated player data
    public PlayerDTO selfPlayer;

    /// <summary>
    /// Initializes the singleton instance
    /// and keeps the object persistent across scenes.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// Triggered if multiple instances exist.
    /// </exception>
    private void Awake()
    {
        // Prevent duplicate singleton instances
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Preserve object between scene changes
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    /// <returns>Void function</returns>
    /// <exception cref="Exception">
    /// No exception conditions.
    /// </exception>
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Decodes a JWT token payload
    /// and stores the authenticated player information.
    /// </summary>
    /// <param name="jwt">
    /// JWT token received from the backend API.
    /// </param>
    /// <returns>Void function</returns>
    /// <exception cref="FormatException">
    /// Triggered if the JWT format is invalid.
    /// </exception>
    public void DecodePayload(string jwt)
    {
        // Split JWT into its three parts
        string[] parts = jwt.Split('.');

        // Validate JWT structure
        if (parts.Length != 3)
        {
            Debug.LogError("Invalid JWT");
        }

        // Extract payload section
        string payload = parts[1];

        // Convert Base64URL format into standard Base64
        payload = payload.Replace('-', '+').Replace('_', '/');

        // Add missing padding if necessary
        switch (payload.Length % 4)
        {
            case 2:
                payload += "==";
                break;

            case 3:
                payload += "=";
                break;
        }

        // Decode Base64 payload
        byte[] jsonBytes = Convert.FromBase64String(payload);

        // Convert bytes to JSON string
        string json = Encoding.UTF8.GetString(jsonBytes);

        // Deserialize token payload
        TokenPayloadClaim payloadToken =
            JsonConvert.DeserializeObject<TokenPayloadClaim>(json);

        // Store authenticated player data
        selfPlayer.Email = payloadToken.Email;
        selfPlayer.UserName = payloadToken.Username;
        selfPlayer.PlayerID = payloadToken.UserId;
    }
}