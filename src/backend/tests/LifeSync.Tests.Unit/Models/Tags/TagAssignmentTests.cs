using FluentAssertions;
using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.Tags;
using LifeSync.Common.Required;

namespace LifeSync.Tests.Unit.Models.Tags;

public class TagAssignmentTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateAssignment()
    {
        Guid tagId = Guid.NewGuid();
        Guid entityId = Guid.NewGuid();
        EntityRef entity = new(entityId, EntityType.ExpenseTransaction);

        TagAssignment assignment = TagAssignment.From(
            tagId.ToRequiredStruct(),
            entity,
            "user-1".ToRequiredString());

        assignment.TagId.Should().Be(tagId);
        assignment.Entity.Should().Be(entity);
        assignment.UserId.Should().Be("user-1");
    }

    [Fact]
    public void From_WithEmptyTagId_ShouldThrow()
    {
        EntityRef entity = new(Guid.NewGuid(), EntityType.ExpenseTransaction);

        Action act = () => TagAssignment.From(
            Guid.Empty.ToRequiredStruct(),
            entity,
            "user-1".ToRequiredString());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithEmptyUserId_ShouldThrow()
    {
        EntityRef entity = new(Guid.NewGuid(), EntityType.ExpenseTransaction);

        Action act = () => TagAssignment.From(
            Guid.NewGuid().ToRequiredStruct(),
            entity,
            "".ToRequiredString());

        act.Should().Throw<ArgumentException>();
    }
}
