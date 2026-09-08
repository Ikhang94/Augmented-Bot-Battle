using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Singleton HTTP client to communicate with the ASP.NET backend on Azure.
/// Usage: ApiManager.Instance.GetAllPlayers(onSuccess, onError)
/// </summary>
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [Tooltip("Backend base URL (no trailing slash)")]
    public string baseUrl = "https://ca-bb-dev-we-01.ambitiouswater-eeea4dd2.francecentral.azurecontainerapps.io";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── Players ──────────────────────────────────────────────────────────────

    public void GetAllPlayers(Action<PlayerResponse[]> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(Get<PlayerResponse[]>("/api/users/getall", onSuccess, onError));
    }

    public void CreatePlayer(CreatePlayerRequest body, Action<PlayerResponse> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(Post<PlayerResponse>("/api/users", body, onSuccess, onError));
    }

    // ─── Robots ───────────────────────────────────────────────────────────────

    public void GetAllRobots(Action<RobotResponse[]> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(Get<RobotResponse[]>("/api/robots", onSuccess, onError));
    }

    public void CreateRobot(CreateRobotRequest body, Action<RobotResponse> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(Post<RobotResponse>("/api/robots", body, onSuccess, onError));
    }

    // ─── Generic HTTP helpers ─────────────────────────────────────────────────

    private IEnumerator Get<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
    {
        using var req = UnityWebRequest.Get(baseUrl + endpoint);
        req.SetRequestHeader("Accept", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[ApiManager] GET {endpoint} failed: {req.error}");
            onError?.Invoke(req.error);
            yield break;
        }

        T result = JsonUtility.FromJson<T>(WrapArray(req.downloadHandler.text));
        onSuccess?.Invoke(result);
    }

    private IEnumerator Post<T>(string endpoint, object body, Action<T> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(body);
        using var req = new UnityWebRequest(baseUrl + endpoint, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[ApiManager] POST {endpoint} failed: {req.error} — {req.downloadHandler.text}");
            onError?.Invoke(req.error);
            yield break;
        }

        T result = JsonUtility.FromJson<T>(req.downloadHandler.text);
        onSuccess?.Invoke(result);
    }

    /// JsonUtility ne désérialise pas les tableaux JSON racine directement.
    /// On enveloppe dans un objet wrapper si nécessaire.
    private string WrapArray(string json)
    {
        if (json.TrimStart().StartsWith("["))
            return $"{{\"items\":{json}}}";
        return json;
    }
}

// ─── DTOs ─────────────────────────────────────────────────────────────────────

[Serializable]
public class PlayerResponse
{
    public int id;
    public string userName;
    public string email;
}

[Serializable]
public class CreatePlayerRequest
{
    public string userName;
    public string email;
    public string password;
}

[Serializable]
public class RobotResponse
{
    public int id;
    public string robotName;
    public int playerId;
}

[Serializable]
public class CreateRobotRequest
{
    public string robotName;
    public int playerId;
}

// Wrappers pour JsonUtility (ne supporte pas les tableaux racine)
[Serializable] public class PlayerResponseWrapper { public PlayerResponse[] items; }
[Serializable] public class RobotResponseWrapper   { public RobotResponse[]  items; }
