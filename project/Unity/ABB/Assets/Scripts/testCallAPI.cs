using UnityEngine;

public class testCallAPI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(UserTokenService.Instance.selfPlayer.PlayerID);
        Debug.Log(UserTokenService.Instance.selfPlayer.Email);
        Debug.Log(UserTokenService.Instance.selfPlayer.UserName);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
