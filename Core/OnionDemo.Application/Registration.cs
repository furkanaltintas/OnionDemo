﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OnionDemo.Application.Bases;
using OnionDemo.Application.Beheviors;
using OnionDemo.Application.Exceptions;
using System.Globalization;
using System.Reflection;

namespace OnionDemo.Application;

public static class Registration
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddTransient<ExceptionMiddleware>();
        services.AddRulesFromAssemblyContaining(assembly, typeof(BaseRules));

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("tr-TR");

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehevior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RedisCacheBehevior<,>));
    }

    private static IServiceCollection AddRulesFromAssemblyContaining(
        this IServiceCollection services, Assembly assembly, Type type)
    {
        List<Type> types = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
            services.AddTransient(item);
        return services;
    }
}
