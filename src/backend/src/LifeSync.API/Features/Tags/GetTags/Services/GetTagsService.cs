using LifeSync.API.Features.Tags.Shared.Models;
using LifeSync.API.Models.Tags;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Tags.GetTags.Services;

public class GetTagsService : BaseService, IGetTagsService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<GetTagsService> _logger;

    public GetTagsService(ApplicationDbContext db, ILogger<GetTagsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DataResult<IReadOnlyList<TagDto>>> GetUserTagsAsync(
        RequiredString userId,
        string? prefix,
        CancellationToken cancellationToken)
    {
        string userIdValue = userId;

        IQueryable<Tag> query = _db.Tags.Where(t => t.UserId == userIdValue);

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            string normalized = prefix.Trim().ToLowerInvariant();
            query = query.Where(t => t.Name.StartsWith(normalized));
        }

        List<TagDto> tags = await query
            .OrderBy(t => t.Name)
            .Select(t => new TagDto(t.Id, t.Name))
            .ToListAsync(cancellationToken);

        return Success<IReadOnlyList<TagDto>>(tags);
    }
}
