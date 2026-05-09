namespace LifeSync.API.Models.Abstractions;

/// <summary>
/// A polymorphic reference to a domain entity — the pair <c>(EntityId, EntityType)</c> that
/// uniquely identifies one row across any <see cref="IDomainEntity"/> table. Used by features
/// like tags and links that need to point at "some entity, type known at runtime."
/// </summary>
public readonly record struct EntityRef
{
    private EntityRef(Guid entityId, EntityType entityType)
    {
        EntityId = entityId;
        EntityType = entityType;
    }

    public Guid EntityId { get; }

    public EntityType EntityType { get; }

    public static EntityRef From(IDomainEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var entityId = ((Entity)entity).Id;
        var entityType = entity.EntityType;

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity ID cannot be empty.", nameof(entity));
        }

        if (!Enum.IsDefined(typeof(EntityType), entityType))
        {
            throw new ArgumentException("Invalid entity type.", nameof(entity));
        }

        return new EntityRef(entityId, entityType);
    }

    // Rehydration path for persistence — values were already validated when the row was first written,
    // so we skip re-validation here (mirrors the Money.FromPersistence pattern).
    internal static EntityRef From(Guid entityId, EntityType entityType)
    {
        return new EntityRef(entityId, entityType);
    }
}
