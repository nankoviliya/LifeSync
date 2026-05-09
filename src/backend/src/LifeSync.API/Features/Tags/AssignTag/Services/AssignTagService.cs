using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.Tags;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Tags.AssignTag.Services;

public class AssignTagService : BaseService, IAssignTagService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AssignTagService> _logger;

    private readonly Dictionary<EntityType, Func<Guid, string, CancellationToken, Task<bool>>> _ownershipChecks;

    public AssignTagService(ApplicationDbContext db, ILogger<AssignTagService> logger)
    {
        _db = db;
        _logger = logger;

        _ownershipChecks = new Dictionary<EntityType, Func<Guid, string, CancellationToken, Task<bool>>>
        {
            [EntityType.ExpenseTransaction] = (entityId, userId, ct) =>
                _db.ExpenseTransactions.AnyAsync(x => x.Id == entityId && x.UserId == userId, ct),
            [EntityType.IncomeTransaction] = (entityId, userId, ct) =>
                _db.IncomeTransactions.AnyAsync(x => x.Id == entityId && x.UserId == userId, ct),
        };
    }

    public async Task<DataResult<Guid>> AssignTagAsync(
        RequiredString userId,
        RequiredString tagName,
        RequiredStruct<Guid> entityId,
        EntityType entityType,
        CancellationToken cancellationToken)
    {
        string userIdValue = userId;
        string tagNameValue = ((string)tagName).Trim().ToLowerInvariant();
        Guid entityIdValue = entityId;

        if (!_ownershipChecks.TryGetValue(entityType, out var ownershipCheck))
        {
            return Failure<Guid>("Entity type does not support tagging.");
        }

        bool ownsEntity = await ownershipCheck(entityIdValue, userIdValue, cancellationToken);
        if (!ownsEntity)
        {
            return Failure<Guid>("Entity not found or access denied.");
        }

        (Tag? tag, string? error) = await GetOrCreateTagAsync(userId, tagName, cancellationToken);
        if (tag is null)
        {
            return Failure<Guid>(error!);
        }

        TagAssignment? existing = await _db.TagAssignments
            .FirstOrDefaultAsync(
                a => a.TagId == tag.Id
                  && a.EntityId == entityIdValue
                  && a.EntityType == entityType,
                cancellationToken);

        if (existing is not null)
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Success(existing.Id);
        }

        TagAssignment assignment = TagAssignment.From(
            tag.Id.ToRequiredStruct(),
            EntityRef.From(entityIdValue, entityType),
            userId);

        _db.TagAssignments.Add(assignment);

        await _db.SaveChangesAsync(cancellationToken);

        return Success(assignment.Id);
    }

    private async Task<(Tag? Tag, string? Error)> GetOrCreateTagAsync(
        RequiredString userId,
        RequiredString tagName,
        CancellationToken cancellationToken)
    {
        string userIdValue = userId;
        string tagNameValue = ((string)tagName).Trim().ToLowerInvariant();

        Tag? tag = await _db.Tags
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserId == userIdValue && x.Name == tagNameValue, cancellationToken);

        if (tag is null)
        {
            try
            {
                tag = Tag.From(tagName, userId);
            }
            catch (ArgumentException ex)
            {
                return (null, ex.Message);
            }

            _db.Tags.Add(tag);
            return (tag, null);
        }

        if (tag.IsDeleted)
        {
            tag.Restore();
        }

        return (tag, null);
    }
}
