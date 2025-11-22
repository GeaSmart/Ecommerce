using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Order.Events;

public sealed class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public OrderCreatedEvent(Guid orderId) => OrderId = orderId;
}
