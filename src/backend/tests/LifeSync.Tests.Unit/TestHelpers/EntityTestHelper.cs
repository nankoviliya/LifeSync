using LifeSync.API.Models.Abstractions;
using System.Reflection;

namespace LifeSync.UnitTests.TestHelpers;

/// <summary>
/// Helper class for setting up Entity instances in tests.
/// Uses reflection to set the Id property which is init-only in production code.
/// </summary>
public static class EntityTestHelper
{
    /// <summary>
    /// Sets the Id property on an Entity instance for testing purposes.
    /// </summary>
    public static void SetId<TEntity>(TEntity entity, Guid id) where TEntity : Entity
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var idProperty = typeof(Entity).GetProperty("Id");

        if (idProperty is null)
            throw new InvalidOperationException("Could not find Id property on Entity base class.");

        idProperty.SetValue(entity, id);
    }

    /// <summary>
    /// Sets a unique Id on an Entity instance for testing purposes.
    /// </summary>
    public static void SetUniqueId<TEntity>(TEntity entity) where TEntity : Entity
    {
        SetId(entity, Guid.NewGuid());
    }

    /// <summary>
    /// Creates two entities with different IDs to ensure they are not equal.
    /// </summary>
    public static (TEntity first, TEntity second) CreateTwoWithDifferentIds<TEntity>(
        Func<TEntity> factory) where TEntity : Entity
    {
        var first = factory();
        var second = factory();

        SetId(first, Guid.NewGuid());
        SetId(second, Guid.NewGuid());

        return (first, second);
    }

    /// <summary>
    /// Creates two entities with the same ID to ensure they are equal.
    /// </summary>
    public static (TEntity first, TEntity second) CreateTwoWithSameId<TEntity>(
        Func<TEntity> factory) where TEntity : Entity
    {
        var first = factory();
        var second = factory();

        var sharedId = Guid.NewGuid();
        SetId(first, sharedId);
        SetId(second, sharedId);

        return (first, second);
    }
}
