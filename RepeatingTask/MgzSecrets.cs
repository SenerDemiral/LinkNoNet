namespace RepeatingTask;

public class MgzSecrets
{
    public int MTid { get; set; }
    public string Uri { get; set; }
    public string? Client_Id { get; set; }
    public string? Client_Secret { get; set; }
    public DateTime Expires_At { get; set; }
    public string? Access_Token { get; set; }
    public string? Refresh_Token { get; set; }
    public DateTime Last_EXD { get; set; }
}
