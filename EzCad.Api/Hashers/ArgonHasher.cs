using System.Text;
using EzCad.Database.Entities;
using EzCad.Services.Interfaces;
using EzCad.Shared.Utils;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Identity;

namespace EzCad.Api.Hashers;

public class ArgonHasher : IPasswordHasher<User>
{
    private readonly IBackendConfigurationService _backendConfiguration;

    public ArgonHasher(IBackendConfigurationService backendConfiguration)
    {
        _backendConfiguration = backendConfiguration;
    }

    string IPasswordHasher<User>.HashPassword(User user, string password)
    {
        if (string.IsNullOrWhiteSpace(user.Salt)) user.Salt = RandomExtended.GetRandomString(50);

        var config = new Argon2Config
        {
            ClearPassword = true,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = Encoding.UTF8.GetBytes(user.Salt),
            ClearSecret = true,
            Secret = Encoding.UTF8.GetBytes(_backendConfiguration.Configuration.PasswordSecret)
        };

        return config.HashWithConfiguration();
    }

    PasswordVerificationResult IPasswordHasher<User>.VerifyHashedPassword(User user,
        string hashedPassword, string providedPassword)
    {
        var config = new Argon2Config
        {
            ClearPassword = true,
            Password = Encoding.UTF8.GetBytes(providedPassword),
            Salt = Encoding.UTF8.GetBytes(user.Salt),
            ClearSecret = true,
            Secret = Encoding.UTF8.GetBytes(_backendConfiguration.Configuration.PasswordSecret)
        };

        var calculatedHash = config.HashWithConfiguration();

        var check = calculatedHash == hashedPassword;

        return check ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}