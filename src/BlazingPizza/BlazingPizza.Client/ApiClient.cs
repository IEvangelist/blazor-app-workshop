// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Http.Json;

namespace BlazingPizza.Client;

internal sealed class ApiClient(HttpClient client)
{
    public async Task<List<PizzaSpecial>> GetSpecialsAsync()
    {
        return await client.GetFromJsonAsync<List<PizzaSpecial>>(
            "api/pizzas/specials") ?? [];
    }

    public async Task<List<Topping>> GetToppingsAsync()
    {
        return await client.GetFromJsonAsync<List<Topping>>(
            "api/pizzas/toppings") ?? [];
    }
}
