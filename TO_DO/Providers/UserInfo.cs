namespace TO_DO.Providers;

public class UserInfo
{
    public string Id { get; }
    public string Username { get; }

    public UserInfo(string id, string username)
    {
        Id = id;
        Username = username;
    }
}