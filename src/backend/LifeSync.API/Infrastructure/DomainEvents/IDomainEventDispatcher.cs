using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents);
}