using FluentAssertions;
using LifeSync.API.Models.Abstractions;

namespace LifeSync.Tests.Unit.Models.Abstractions;

public class EntityTests
{
    private class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id) { }
        public TestEntity() : base() { }
    }

    [Fact]
    public void Constructor_WithValidId_ShouldCreateEntity()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id);

        entity.Id.Should().Be(id);
        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmptyGuid_ShouldThrowArgumentException()
    {
        var emptyId = Guid.Empty;

        Action act = () => new TestEntity(emptyId);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Entity Id cannot be empty.*")
            .WithParameterName("id");
    }

    [Fact]
    public void MarkAsDeleted_WhenNotDeleted_ShouldSetIsDeletedAndDeletedAt()
    {
        var entity = new TestEntity(Guid.NewGuid());
        var beforeDelete = DateTime.UtcNow;

        entity.MarkAsDeleted();
        var afterDelete = DateTime.UtcNow;

        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeOnOrAfter(beforeDelete);
        entity.DeletedAt.Should().BeOnOrBefore(afterDelete);
    }

    [Fact]
    public void MarkAsDeleted_WhenAlreadyDeleted_ShouldThrowInvalidOperationException()
    {
        var entity = new TestEntity(Guid.NewGuid());
        entity.MarkAsDeleted();

        Action act = () => entity.MarkAsDeleted();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Entity is already marked as deleted.");
    }

    [Fact]
    public void Restore_WhenDeleted_ShouldClearIsDeletedAndDeletedAt()
    {
        var entity = new TestEntity(Guid.NewGuid());
        entity.MarkAsDeleted();

        entity.Restore();

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Restore_WhenNotDeleted_ShouldThrowInvalidOperationException()
    {
        var entity = new TestEntity(Guid.NewGuid());

        Action act = () => entity.Restore();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Entity is not deleted.");
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.Equals(entity2).Should().BeTrue();
        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        entity1.Equals(entity2).Should().BeFalse();
        (entity1 == entity2).Should().BeFalse();
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var entity = new TestEntity(Guid.NewGuid());

        entity.Equals(null).Should().BeFalse();
        (entity == null).Should().BeFalse();
        (entity != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_BothNull_ShouldReturnTrue()
    {
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        (entity1 == entity2).Should().BeTrue();
        (entity1 != entity2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameReference_ShouldReturnTrue()
    {
        var entity = new TestEntity(Guid.NewGuid());

        entity.Equals(entity).Should().BeTrue();
        (entity == entity).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new AnotherTestEntity(entity1.Id);

        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.GetHashCode().Should().Be(entity2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentId_ShouldReturnDifferentHashCode()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
    }

    [Fact]
    public void CreatedAt_ShouldBeSettableViaReflection()
    {
        var entity = new TestEntity(Guid.NewGuid());
        var timestamp = DateTime.UtcNow;

        // Using reflection to set private setter (simulating what EF Core does)
        var property = typeof(Entity).GetProperty("CreatedAt")!;
        var setter = property.GetSetMethod(nonPublic: true)!;
        setter.Invoke(entity, new object[] { timestamp });

        entity.CreatedAt.Should().Be(timestamp);
    }

    [Fact]
    public void UpdatedAt_ShouldBeSettableViaReflection()
    {
        var entity = new TestEntity(Guid.NewGuid());
        var timestamp = DateTime.UtcNow;

        // Using reflection to set private setter (simulating what EF Core does)
        var property = typeof(Entity).GetProperty("UpdatedAt")!;
        var setter = property.GetSetMethod(nonPublic: true)!;
        setter.Invoke(entity, new object[] { timestamp });

        entity.UpdatedAt.Should().Be(timestamp);
    }

    private class AnotherTestEntity : Entity
    {
        public AnotherTestEntity(Guid id) : base(id) { }
    }
}
