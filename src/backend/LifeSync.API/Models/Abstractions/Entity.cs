namespace LifeSync.API.Models.Abstractions;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {

    }

    public Guid Id { get; init; }

    // public DateTime CreatedAt { get; set; }
    //
    // public DateTime UpdatedAt { get; set; }
}