using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Tags.UnassignTag.Services;

public interface IUnassignTagService
{
    Task<DataResult<bool>> UnassignTagAsync(
        RequiredString userId,
        RequiredStruct<Guid> assignmentId,
        CancellationToken cancellationToken);
}
