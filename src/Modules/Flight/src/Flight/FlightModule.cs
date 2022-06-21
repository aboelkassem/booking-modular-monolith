﻿using System.Collections.Generic;
using System.Reflection;
using BuildingBlocks.Caching;
using BuildingBlocks.Domain;
using BuildingBlocks.EFCore;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Mapster;
using Flight.Data;
using Flight.Data.Seed;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flight;

public static class FlightModule
{
    public static IServiceCollection AddFlightModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCustomDbContext<FlightDbContext>(nameof(Flight), configuration);
        services.AddScoped<IDataSeeder, FlightDataSeeder>();
        services.AddTransient<IEventMapper, EventMapper>();
        SnowFlakIdGenerator.Configure(1);

        services.AddValidatorsFromAssembly(typeof(FlightRoot).Assembly);
        services.AddCustomMapster(typeof(FlightRoot).Assembly);
        services.AddCachingRequest(new List<Assembly> {typeof(FlightRoot).Assembly});

        return services;
    }

    public static IApplicationBuilder UseFlightModules(this IApplicationBuilder app)
    {
        app.UseMigrationsAsync<FlightDbContext>().GetAwaiter().GetResult();
        return app;
    }
}