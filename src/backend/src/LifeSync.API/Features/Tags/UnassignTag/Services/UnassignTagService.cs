using LifeSync.API.Models.Tags;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Tags.UnassignTag.Services;

public class UnassignTagService : BaseService, IUnassignTagService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<UnassignTagService> _logger;

    public UnassignTagService(ApplicationDbContext db, ILogger<UnassignTagService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DataResult<bool>> UnassignTagAsync(
        RequiredString userId,
        RequiredStruct<Guid> assignmentId,
        CancellationToken cancellationToken)
    {
        string userIdValue = userId;
        Guid assignmentIdValue = assignmentId;

        TagAssignment? assignment = await _db.TagAssignments
            .FirstOrDefaultAsync(a => a.Id == assignmentIdValue, cancellationToken);

        if (assignment is null || assignment.UserId != userIdValue)
        {
            return Failure<bool>("Assignment not found or access denied.");
        }

        Guid tagId = assignment.TagId;
        _db.TagAssignments.Remove(assignment);

        bool anyOther = await _db.TagAssignments
            .AnyAsync(a => a.TagId == tagId && a.Id != assignmentIdValue, cancellationToken);

        if (!anyOther)
        {
            Tag? tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);
            tag?.MarkAsDeleted();
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Success(true);
    }
}
