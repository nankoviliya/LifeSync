using LifeSync.API.Features.Tags.Shared.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Tags.GetTags.Services;

public interface IGetTagsService
{
    Task<DataResult<IReadOnlyList<TagDto>>> GetUserTagsAsync(
        RequiredString userId,
        string? prefix,
        CancellationToken cancellationToken);
}
