using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazingPizza.Client;

public class PizzaAuthenticationState : RemoteAuthenticationState // Nope!
{
    public Order? Order { get; set; }
}
