using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Models.Languages
{
    public class Language : Entity
    {
        public string Name { get; init; } = default!;

        public string Code { get; init; } = default!;
    }
}
