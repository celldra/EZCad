using System.Security.Cryptography;
using System.Text;

namespace EzCad.Shared.Utils;

public static class RandomExtended
{
    private static readonly char[] Chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public static string GetRandomString(int size)
    {
        var data = new byte[4 * size];
        using var crypto = RandomNumberGenerator.Create();
        crypto.GetBytes(data);
        StringBuilder result = new(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % Chars.Length;

            result.Append(Chars[idx]);
        }

        return result.ToString();
    }
}