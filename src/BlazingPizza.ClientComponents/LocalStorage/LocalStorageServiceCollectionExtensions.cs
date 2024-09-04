// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using BlazingPizza.ClientComponents;

namespace Microsoft.Extensions.DependencyInjection;

public static class LocalStorageServiceCollectionExtensions
{
    public static IServiceCollection AddLocalStorageJSInterop(this IServiceCollection services)
    {
        services.AddScoped<LocalStorageJSInterop>();

        return services;
    }
}
