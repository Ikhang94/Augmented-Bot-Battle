using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TestLoginCall : MonoBehaviour
{

    private string URL = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io/api";
    [Header("Inputs")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("UI")]
    public TMP_Text errorText;

    [Header("Button")]
    public Button buttonLogin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetText());
        //StartCoroutine(TestData());
        buttonLogin.onClick.AddListener(Login);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(usernameInput.text);
        //Debug.Log(passwordInput.text);

    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL + "/users/getall");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
    void Login() => StartCoroutine(Login_Couroutine());
    IEnumerator Login_Couroutine()
    {
        PlayerDTO user = new PlayerDTO();
        user.Email = usernameInput.text;
        user.Password = passwordInput.text;
        string json = JsonUtility.ToJson(user);
        Debug.Log(json);
        //WWWForm form = new WWWForm();
        //form.AddField("username", usernameInput.text);
        //form.AddField("password", passwordInput.text);
        using (UnityWebRequest www = UnityWebRequest.Post(URL + "/users/login", json, "application/json"))

        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                UserToken response =
    JsonUtility.FromJson<UserToken>(www.downloadHandler.text);
                Debug.Log(response.result);
                //String decoded = JwtDecoder.DecodePayload(response.userToken);
                UserTokenService.Instance.DecodePayload(response.result);
                Debug.Log(UserTokenService.Instance.selfPlayer);
                SceneManager.LoadScene("TestAPICallScene");

            }
        }
    }

}