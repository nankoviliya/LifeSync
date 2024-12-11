using PersonalFinances.API.Models.Abstractions;

namespace PersonalFinances.API.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents);
}