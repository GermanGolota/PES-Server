namespace WebAPI.RequestModels;

public class UpdateCredentialRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string PesKey { get; set; }
}