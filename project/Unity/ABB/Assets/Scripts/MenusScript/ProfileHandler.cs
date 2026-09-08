/// Fichier : ProfileHandler.cs
/// Auteur : CAO Thien-Khang
/// Date : 23/05/2026
/// Description : retrieves player's username,
///               and display it on header's hub
/// Version : 1.0
/// Dernière modification : 26/05/2026

using TMPro;
using UnityEngine;

public class ProfileHandler : MonoBehaviour
{
    // text component for username on Header's hub
    public TextMeshProUGUI userNameProfile;
    void Start()
    {
        //update username on header
        userNameProfile.text = UserTokenService.Instance.selfPlayer.UserName;
    }

}
