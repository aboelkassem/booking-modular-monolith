﻿using System.Collections.Generic;
using System.Reflection;
using BuildingBlocks.Caching;
using BuildingBlocks.Domain;
using BuildingBlocks.EFCore;
using BuildingBlocks.Mapster;
using FluentValidation;
using Identity.Data;
using Identity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModules(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env = null)
    {
        services.AddCustomDbContext<IdentityContext>(nameof(Identity), configuration);
        services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        
        services.AddTransient<IEventMapper, EventMapper>();
        services.AddIdentityServer(env);

        services.AddValidatorsFromAssembly(typeof(IdentityRoot).Assembly);
        services.AddCustomMapster(typeof(IdentityRoot).Assembly);

        services.AddCachingRequest(new List<Assembly> {typeof(IdentityRoot).Assembly});

        return services;
    }

    public static IApplicationBuilder UseIdentityModules(this IApplicationBuilder app)
    {
        app.UseIdentityServer();
        app.UseMigrationsAsync<IdentityContext>().GetAwaiter().GetResult();
        return app;
    }
}