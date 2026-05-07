using System.Text.RegularExpressions;
using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Tags;

public class Tag : Entity
{
    private const int MaxNameLength = 50;
    private static readonly Regex NamePattern =
        new(@"^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions.Compiled);

    private Tag() { }

    public static Tag From(RequiredString name, RequiredString userId)
    {
        string nameValue = ((string)name).Trim().ToLowerInvariant();
        string userIdValue = userId;

        ValidateName(nameValue);
        ValidateUserId(userIdValue);

        return new Tag(nameValue, userIdValue);
    }

    private Tag(string name, string userId)
    {
        Name = name;
        UserId = userId;
    }

    public string Name { get; private set; } = default!;

    public string UserId { get; private set; } = default!;

    public User User { get; init; } = default!;

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));
        }

        if (name.Length > MaxNameLength)
        {
            throw new ArgumentException(
                $"Tag name cannot exceed {MaxNameLength} characters.", nameof(name));
        }

        if (!NamePattern.IsMatch(name))
        {
            throw new ArgumentException(
                "Tag name must contain only lowercase letters, digits, and non-leading/trailing/consecutive hyphens.",
                nameof(name));
        }
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
    }
}
