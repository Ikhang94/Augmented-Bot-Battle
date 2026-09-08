using UnityEngine;
using TMPro;
using Unity.Services.Core;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Authentication;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;

public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TextMeshProUGUI codeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignOut();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
    }

    async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); 
        codeText.text = "Code: " + joinCode;

        NetworkManager.Singleton.StartHost();
        Debug.Log("created");
    }   

    async void JoinRelay(string joinCode)
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
        NetworkManager.Singleton.StartClient();
        Debug.Log("connected");
    }
}
