using Newtonsoft.Json;
using System;

[Serializable]
public class TokenPayloadClaim
{
    [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")]
    public int UserId;

    [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")]
    public string Email;

    [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")]
    public string Username;
}