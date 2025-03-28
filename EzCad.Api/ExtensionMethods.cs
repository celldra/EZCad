using FuzzySharp;
using Microsoft.AspNetCore.Identity;

namespace EzCad.Api;

public static class ExtensionMethods
{
    public static IEnumerable<T> YieldFuzzySearchResults<T>(this IEnumerable<T> list, string query)
    {
        foreach (var item in list)
            if (!string.IsNullOrWhiteSpace(query))
            {
                if (item?.ToString()?.FuzzyCompare(query) == true)
                    continue;

                yield return item;
            }
            else
            {
                yield return item;
            }
    }

    public static bool FuzzyCompare(this string data, string query)
    {
        return Fuzz.PartialRatio(data.ToLower(), query.ToLower()) <= 55;
    }

    public static string Convert(this SignInResult value)
    {
        return value switch
        {
            {IsLockedOut: true} =>
                "The account has been locked as there has been too many failed sign in attempts.",
            {IsNotAllowed: true} => "You need to verify your email to sign in.",
            {Succeeded: false} => "Invalid username or password.",
            {RequiresTwoFactor: true} => "Multi-factor authentication is required.",
            _ => "Unhandled sign-in result state."
        };
    }

    public static string Convert(this IdentityResult value)
    {
        return value.Errors.Aggregate(string.Empty, (current, error) => current + $"{error.Description}, ")
            .TrimEnd(' ')
            .TrimEnd(',');
    }
}