using System.Diagnostics;
using EzCad.Services;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace EzCad.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var configuration = new BackendConfigurationService(new NullLogger<BackendConfigurationService>());
        if (configuration is null) Assert.Fail("Configuration == null");

        Assert.Pass(configuration.Configuration.DatabaseConfiguration.BuildPostgresConnectionString());
    }
}