using UnityEngine;
using Unity.Netcode;
public class PlayerMove : NetworkBehaviour
{
    [SerializeField]
    CharacterController _characterController;


    private void Awake()
    {
        _characterController.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        if (!IsOwner)
        {
            enabled = false;
            _characterController.enabled = false;
            return;
        }
        _characterController.enabled = true;
    }
}
