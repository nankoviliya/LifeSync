using LifeSync.API.Models.Abstractions;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Tags.AssignTag.Services;

public interface IAssignTagService
{
    Task<DataResult<Guid>> AssignTagAsync(
        RequiredString userId,
        RequiredString tagName,
        RequiredStruct<Guid> entityId,
        EntityType entityType,
        CancellationToken cancellationToken);
}
