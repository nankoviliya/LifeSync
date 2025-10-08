namespace LifeSync.API.Models.Abstractions;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Entity Id cannot be empty.", nameof(id));
        }

        Id = id;
    }

    protected Entity()
    {
    }

    public Guid Id { get; init; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// Marks the entity as deleted (soft delete).
    /// Sets IsDeleted to true and records the deletion timestamp.
    /// The UpdatedAt timestamp will be automatically set by the database on save.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when entity is already marked as deleted</exception>
    public void MarkAsDeleted()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Entity is already marked as deleted.");
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores a soft-deleted entity.
    /// Sets IsDeleted to false and clears the deletion timestamp.
    /// The UpdatedAt timestamp will be automatically set by the database on save.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when entity is not deleted</exception>
    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new InvalidOperationException("Entity is not deleted.");
        }

        IsDeleted = false;
        DeletedAt = null;
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}