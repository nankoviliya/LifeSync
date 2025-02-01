namespace LifeSync.API.Features.Users.Models
{
    public record ModifyUserProfileDataDto
    {
        public string FirstName { get; init; } = default!;

        public string LastName { get; init; } = default!;

        public string LanguageId { get; init; } = default!;
    }
}
