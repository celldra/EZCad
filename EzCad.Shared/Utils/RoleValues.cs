using System.Reflection;

namespace EzCad.Shared.Utils;

public static class RoleValues
{
    public const string Administrator = "Administrator";
    public const string Moderator = "Moderator";
    public const string Medical = "Medical";
    public const string Police = "Police";
    public const string ArmedPolice = "ArmedPolice";
    public const string Privileged = "Privileged";
    public const string Banned = "Banned";

    private static string?[] _defaultRoles = Array.Empty<string>();

    public static string GetRoleString(params string[] roles)
    {
        return roles.Aggregate(string.Empty, (c, r) => c + $"{r},");
    }

    private static IReadOnlyList<FieldInfo> GetConstants(IReflect type)
    {
        var fieldInfos = type.GetFields(BindingFlags.Public |
                                        BindingFlags.Static | BindingFlags.FlattenHierarchy);

        return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
    }

    public static IReadOnlyList<string?> GetAllDefaultRoles()
    {
        if (_defaultRoles.Length > 0) return _defaultRoles;

        var constants = GetConstants(typeof(RoleValues));

        _defaultRoles = constants.Select(c => c.GetValue(null)?.ToString())
            .ToArray();

        return constants.Select(c => c.GetValue(null)?.ToString())
            .ToArray();
    }
}