using LifeSync.API.Models.Abstractions;

namespace LifeSync.API.Infrastructure.DomainEvents;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent);
}