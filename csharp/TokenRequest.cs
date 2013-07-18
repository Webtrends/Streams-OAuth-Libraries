using System.Security.Cryptography
using RestSharp // available via nuget
using Newtonsoft.Json // available via nuget
using Newtonsoft.Json.Linq // available via nuget

/// <summary>
/// Use to get a token from Webtrends auth service
/// </summary>
public class TokenRequest
{
    private readonly string audience = "auth.webtrends.com";
    private readonly string scope = "sapi.webtrends.com";
    private readonly string authUrl = "https://sauth.webtrends.com/v1";
    private readonly string clientId;
    private readonly string clientSecret;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRequest" /> class.
    /// </summary>
    public TokenRequest(string clientId, string clientSecret)
    {
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }

    /// <summary>
    /// Retrieves a new token from Webtrends auth service
    /// </summary>
    public string Execute()
    {
        var builder = new JWTBuilder();
        var header = new JWTHeader
        {
            Type = "JWT",
            Algorithm = "HS256"
        };
        var claimSet = new JWTClaimSet
        {
            Issuer = clientId,
            Principal = clientId,
            Audience = audience,
            Expiration = DateTime.Now.AddSeconds(30),
            Scope = scope
        };

        string assertion = builder.BuildAssertion(header, claimSet, clientSecret);
        var client = new RestClient(authUrl);
        var request = new RestRequest("token/", Method.POST);
        request.AddParameter("client_id", clientId);
        request.AddParameter("client_assertion", assertion);
        request.AddParameter("grant_type", "client_credentials");
        request.AddParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");

        var response = client.Execute(request).Content;
        return (string)JObject.Parse(response)["access_token"];
    }
}

/// <summary>
/// Class for building assertions for authentication requests.
/// </summary>
internal class JWTBuilder
{
    /// <summary>
    /// Computes the Client Assertion portion of the JWT
    /// </summary>
    /// <param name="header">The JSON Web Token header</param>
    /// <param name="claimSet">The JSON Web Token claim set</param>
    /// <param name="clientSecret">The client's secret</param>
    /// <returns>The assertion</returns>
    public string BuildAssertion(JWTHeader header, JWTClaimSet claimSet, string clientSecret)
    {
        // Serialize the header and claimSet
        string serializedHeader = JsonConvert.SerializeObject(header);
        string serializedClaimSet = JsonConvert.SerializeObject(claimSet);

        // Base64Encode the header and claimSet
        string encodedHeader = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(serializedHeader));
        string encodedClaimSet = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(serializedClaimSet));

        // Concatenate the header and the claims separated with a '.': [header].[claims]
        string message = string.Join(".", encodedHeader, encodedClaimSet);

        // Apply an HMAC/SHA-256 hash* to the concatenated content using the Client Secret as the key
        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Base64Encode the result of the hash
        string signature = Convert.ToBase64String(hash);

        // Combine the encoded elements as follows [header].[claims].[hash]
        string assertion = string.Join(".", encodedHeader, encodedClaimSet, signature);
        return assertion;
    }
}

/// <summary>
/// Data Transfer Object for JSON Web Token Header.
/// </summary>
internal class JWTHeader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JWTHeader" /> class.
    /// </summary>
    public JWTHeader()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JWTHeader" /> class.
    /// </summary>
    /// <param name="type">The type. Always "JWT"</param>
    /// <param name="algorithm">The algorithm used to sign the content.  The initial implementation will only support HMAC SHA256 as required by the JWT specification</param>
    public JWTHeader(string type, string algorithm)
    {
        Type = type;
        Algorithm = algorithm;
    }

    /// <summary>
    /// Gets or sets the type. Always "JWT"
    /// </summary>
    [JsonProperty("typ")]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the algorithm used to sign the content.  The initial implementation will only support HMAC SHA256 as required by the JWT specification
    /// </summary>
    [JsonProperty("alg")]
    public string Algorithm { get; set; }
}

/// <summary>
/// Data Transfer Object for JSON Web Token Claim Sets.
/// </summary>
internal class JWTClaimSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JWTClaimSet" /> class.
    /// </summary>
    public JWTClaimSet()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JWTClaimSet" /> class.
    /// </summary>
    /// <param name="issuer">The issuer of the signed token. In this case it is self-issued so the Client Id is used.</param>
    /// <param name="principal">The principal. The id of the client making the request. Also the Client Id in our case.</param>
    /// <param name="audience">The audience. The authentication service intended to process the assertion, expressed as a uri.  This is set to auth.webtrends.com for now but will change to sauth.webtrends.com at some point.</param>
    /// <param name="expiration">The expiration date/time for the JWT</param>
    /// <param name="scope">The requested access for the token</param>
    /// <example>
    /// { "iss":"537e9878e83a41f19c80f144c2f2a31a", "prn":"8924dd87e83c41a10c80a456c5c2b617", "aud":"auth.webtrends.com", "exp":1343433600, "scope": "sapi.webtrends.com" }
    /// </example>
    public JWTClaimSet(string issuer, string principal, string audience, DateTime expiration, string scope)
    {
        Issuer = issuer;
        Principal = principal;
        Audience = audience;
        Expiration = expiration;
        Scope = scope;
    }

    /// <summary>
    /// Gets or sets the issuer of the signed token. In this case it is self-issued so the Client Id is used.
    /// </summary>
    [JsonProperty("iss")]
    public string Issuer { get; set; }

    /// <summary>
    /// Gets or sets the principal. The id of the client making the request. Also the Client Id in our case.
    /// </summary>
    [JsonProperty("prn")]
    public string Principal { get; set; }

    /// <summary>
    /// Gets or sets the audience. The authentication service intended to process the assertion, expressed as a uri.  This is set to auth.webtrends.com for now but will change to sauth.webtrends.com at some point.
    /// </summary>
    [JsonProperty("aud")]
    public string Audience { get; set; }

    /// <summary>
    /// Gets or sets the expiration date/time for the JWT
    /// </summary>
    [JsonProperty("exp")]
    [JsonConverter(typeof(EpochDateTimeConverter))]
    public DateTime Expiration { get; set; }

    /// <summary>
    /// Gets or sets the requested access for the token
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }
}

    /// <summary>
    /// Converts a <see cref="DateTime" /> to epoch time when serializing json
    /// </summary>
    internal class EpochDateTimeConverter : JsonConverter
    {
        /// <summary>
        /// CanRead always returns false because ReadJson is not implemented
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

    /// <summary>
    /// Determines whether this instance can convert the specified object type
    /// </summary>
    /// <param name="type">Type of the object</param>
    /// <returns>True if the specified object type is DateTime; otherwise, false.</returns>
    public override bool CanConvert(Type type)
    {
        return type == typeof(DateTime);
    }

    /// <summary>
    /// Writes the JSON representation of the DateTime as epoch time
    /// </summary>
    /// <param name="writer">The JsonWriter to write to</param>
    /// <param name="value">The value</param>
    /// <param name="serializer">The parameter is not used.</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(((DateTime)value).ToEpochTime());
    }

    /// <summary>
    /// Not Implemented
    /// </summary>
    /// <param name="reader">The parameter is not used.</param>
    /// <param name="objectType">The parameter is not used.</param>
    /// <param name="existingValue">The parameter is not used.</param>
    /// <param name="serializer">The parameter is not used.</param>
    /// <returns>Throws a new NotImplementedException</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Contains extension methods
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Returns the number of seconds since the UNIX epoch 1970-01-01
    /// </summary>
    public static long ToEpochTime(this DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date - epoch).TotalSeconds);
    }
}
