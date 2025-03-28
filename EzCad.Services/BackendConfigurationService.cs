using System.Text.Json;
using EzCad.Services.Interfaces;
using EzCad.Shared.Models;
using Microsoft.Extensions.Logging;

namespace EzCad.Services;

public class BackendConfigurationService : IBackendConfigurationService
{
    private readonly ILogger<BackendConfigurationService> _logger;

    public BackendConfigurationService(ILogger<BackendConfigurationService> logger)
    {
        _logger = logger;

        // Load the default configuration
        LoadConfiguration();
    }

    public AppConfiguration Configuration { get; private set; } = null!;

    public void LoadConfiguration()
    {
        CreateIfMissing();

        var fileContents = File.ReadAllText("configuration.json");
        var json = JsonSerializer.Deserialize<AppConfiguration>(fileContents);

        if (json is null)
        {
            _logger.LogCritical(
                "The configuration file could not be parsed correctly, please make sure that it is a valid JSON file!");

            Environment.Exit(0);
            return;
        }

        Configuration = json;
    }

    private void CreateIfMissing()
    {
        if (File.Exists("configuration.json")) return;

        var defaultConf = new AppConfiguration();
        var json = JsonSerializer.Serialize(defaultConf, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        File.WriteAllText("configuration.json", json);

        _logger.LogCritical(
            "The configuration file could not be detected, therefore we've created a fresh one for you. Be sure to modify 'configuration.json' to customize the CAD to your liking");

        // This realistically should not be called, but there's no better way to do it due to dependency injection, it will be
        // most likely randomly calling the constructor and therefore this method
        Environment.Exit(0);
    }
}