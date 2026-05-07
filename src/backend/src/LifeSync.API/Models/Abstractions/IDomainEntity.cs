namespace LifeSync.API.Models.Abstractions;

/// <summary>
/// Marks an entity that other features can reference polymorphically — tags, links, attachments, and the like.
/// Implementers expose an <see cref="EntityType"/> so a single reference table can point at any of them
/// using <c>(EntityId, EntityType)</c>.
/// </summary>
/// <remarks>
/// Only opt in entities that users actually act on. Infrastructure records (refresh tokens, language lookups)
/// stay out — keeping <see cref="EntityType"/> a small, meaningful list rather than a mirror of every
/// <see cref="Entity"/> subclass.
/// </remarks>
public interface IDomainEntity
{
    /// <summary>The discriminator that identifies this entity's type in cross-entity reference tables.</summary>
    EntityType EntityType { get; }
}
