using PersonalFinances.API.Models.Abstractions;

namespace PersonalFinances.API.Infrastructure.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent);
}