namespace LifeSync.API.Models.Abstractions;

/// <summary>
/// A polymorphic reference to a domain entity — the pair <c>(EntityId, EntityType)</c> that
/// uniquely identifies one row across any <see cref="IDomainEntity"/> table. Used by features
/// like tags and links that need to point at "some entity, type known at runtime."
/// </summary>
public readonly record struct EntityRef
{
    public EntityRef(Guid entityId, EntityType entityType)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity ID cannot be empty.", nameof(entityId));
        }

        if (!Enum.IsDefined(typeof(EntityType), entityType))
        {
            throw new ArgumentException("Invalid entity type.", nameof(entityType));
        }

        EntityId = entityId;
        EntityType = entityType;
    }

    public Guid EntityId { get; }

    public EntityType EntityType { get; }

    public static EntityRef From(IDomainEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new EntityRef(((Entity)entity).Id, entity.EntityType);
    }
}
