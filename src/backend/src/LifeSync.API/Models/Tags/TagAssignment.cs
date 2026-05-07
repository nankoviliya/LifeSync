using LifeSync.API.Models.Abstractions;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Tags;

public class TagAssignment : Entity
{
    private TagAssignment() { }

    public static TagAssignment From(
        RequiredStruct<Guid> tagId,
        EntityRef entity,
        RequiredString userId)
    {
        Guid tagIdValue = tagId;
        string userIdValue = userId;

        if (tagIdValue == Guid.Empty)
        {
            throw new ArgumentException("Tag ID cannot be empty.", nameof(tagId));
        }

        if (string.IsNullOrWhiteSpace(userIdValue))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        return new TagAssignment(tagIdValue, entity, userIdValue);
    }

    private TagAssignment(Guid tagId, EntityRef entity, string userId)
    {
        TagId = tagId;
        Entity = entity;
        UserId = userId;
    }

    public Guid TagId { get; private set; }

    public Tag Tag { get; init; } = default!;

    public EntityRef Entity { get; private set; }

    public string UserId { get; private set; } = default!;
}
