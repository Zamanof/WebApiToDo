namespace TO_DO.Auth;

public class JwtConfig
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Auidience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; }
}
