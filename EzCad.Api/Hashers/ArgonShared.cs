using Isopoh.Cryptography.Argon2;

namespace EzCad.Api.Hashers;

public static class ArgonShared
{
    public static string HashWithConfiguration(this Argon2Config config)
    {
        using var argon = new Argon2(config);

        using var passwordBytes = argon.Hash();
        return config.EncodeString(passwordBytes.Buffer);
    }
}