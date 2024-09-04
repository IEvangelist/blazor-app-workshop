// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BlazingPizza.Data;
using BlazingPizza.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Pizzas;

internal static class PizzaEndpoints
{
    internal static WebApplication MapPizzaEndpoints(this WebApplication app)
    {
        var api = app.MapGroup("api");

        // Subscribe to notifications
        api.MapPut("/notifications/subscribe", OnSubscribeToNotificationsAsync)
            .Produces<NotificationSubscription>(200)
            .Produces<UnauthorizedHttpResult>(401);

        var pizzas = api.MapGroup("pizzas");

        // Get api/pizzas/specials
        pizzas.MapGet("/specials", OnGetSpecialAsync)
            .Produces<PizzaSpecial[]>(200);

        // Get api/pizzas/toppings
        pizzas.MapGet("/toppings", OnGetToppingsAsync)
            .Produces<Topping[]>(200);

        return app;
    }

    [Authorize]
    static async Task<IResult> OnSubscribeToNotificationsAsync(
        HttpContext context,
        PizzaStoreContext db,
        NotificationSubscription subscription)
    {
        // We're storing at most one subscription per user, so delete old ones.
        // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
        var userId = context.GetUserId();

        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var oldSubscriptions = db.NotificationSubscriptions.Where(e => e.UserId == userId);
        db.NotificationSubscriptions.RemoveRange(oldSubscriptions);

        // Store new subscription
        subscription = subscription with { UserId = userId };

        db.NotificationSubscriptions.Attach(subscription);

        await db.SaveChangesAsync();

        return Results.Ok(subscription);
    }

    static async Task<IResult> OnGetSpecialAsync(PizzaStoreContext db)
    {
        var specials = await db.Specials.ToListAsync();

        return Results.Ok(specials);
    }

    static async Task<IResult> OnGetToppingsAsync(PizzaStoreContext db)
    {
        var toppings = await db.Toppings.OrderBy(t => t.Name).ToListAsync();

        return Results.Ok(toppings);
    }
}
