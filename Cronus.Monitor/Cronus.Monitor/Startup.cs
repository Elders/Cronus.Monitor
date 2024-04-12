using Cronus.Monitor.Api;
using Elders.Cronus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cronus.Monitor;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions();
        services.AddLogging();

        services.AddControllers();

        services.AddCronus(Configuration);
        services.AddSingleton<MonitorContainer>();

        services.AddHostedService<Worker>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.ConfigureCors();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        });
    }
}

internal static class CorsExtensions
{
    public static IApplicationBuilder ConfigureCors(this IApplicationBuilder app)
    {
        IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        if (configuration.GetValue<bool>("cors_enabled") == false) return app;

        var headers = GetArrayConfiguration(configuration, "cors_headers");
        var methods = GetArrayConfiguration(configuration, "cors_methods");
        var origins = GetArrayConfiguration(configuration, "cors_origins");

        app.UseCors(builder =>
        {
            if (headers is null == false) builder.WithHeaders(headers);
            if (methods is null == false) builder.WithMethods(methods);
            if (origins is null == false) builder.WithOrigins(origins);
        });

        return app;
    }

    private static string[] GetArrayConfiguration(IConfiguration configuration, string parameter)
    {
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var result = new List<string>();

        string values = configuration[parameter];
        if (string.IsNullOrEmpty(values) == false)
        {
            var singleValue = values
                .Replace("[", "").Replace("]", "").Replace("\"", "")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(@string =>
                {

                    return @string;
                })
                .Distinct();

            result.AddRange(singleValue);
        }

        return result.ToArray();
    }
}

