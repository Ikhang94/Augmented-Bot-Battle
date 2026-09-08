using Unity.Netcode;
using UnityEngine;
namespace NetcodeDemo
{
    public class ClientPlayerMove : NetworkBehaviour
    {
        [SerializeField]
        BaseBotNetwork m_BaseBot;

        [SerializeField]
        Transform m_CameraFollow;
        private void Awake()
        {

            m_BaseBot.enabled = false;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            enabled = IsClient; // Enable if this is a client.
            if (!GameManagerNetwork.Instance.p1)
            {
                GameManagerNetwork.Instance.p1 = m_BaseBot;
            }
            else
            {
                GameManagerNetwork.Instance.p2 = m_BaseBot;
            }
            if (GameManagerNetwork.Instance.p1 && GameManagerNetwork.Instance.p2)
            {
                GameManagerNetwork.Instance.p1.AssignedOpponent(GameManagerNetwork.Instance.p2);
                GameManagerNetwork.Instance.p2.AssignedOpponent(GameManagerNetwork.Instance.p1);
                GameManagerNetwork.Instance.p1Bot = GameManagerNetwork.Instance.p1;
                GameManagerNetwork.Instance.p2Bot = GameManagerNetwork.Instance.p2;
            }
            if (!IsOwner)
            {
                // Disable if this is not the owner
                enabled = false;
                m_BaseBot.enabled = true;
                m_BaseBot.moveController = "";
                m_BaseBot.attackController = "";
                m_BaseBot.dodgeController = "";
                return;
            }
            // Enable if this is an owner
            m_BaseBot.enabled = true;
        }
    }
}