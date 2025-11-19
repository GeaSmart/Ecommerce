using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Orders;

public sealed class OrderPaidEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public OrderPaidEvent(Guid orderId) => OrderId = orderId;
}