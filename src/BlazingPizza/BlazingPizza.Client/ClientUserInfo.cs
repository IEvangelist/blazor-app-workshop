namespace BlazingPizza.Client;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class ClientUserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
}
