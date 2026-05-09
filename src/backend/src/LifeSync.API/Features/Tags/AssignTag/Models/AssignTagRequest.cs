using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Features.Tags.AssignTag.Models;

public record AssignTagRequest
{
    public required string TagName { get; init; }

    public required Guid EntityId { get; init; }

    public required EntityType EntityType { get; init; }
}
