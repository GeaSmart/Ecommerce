using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Order.Events;

// Domain Event
// Porque representa un hecho relevante en el dominio (se puede usar para facturación, notificaciones, integraciones, etc.)
public sealed class OrderPaidEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public OrderPaidEvent(Guid orderId) => OrderId = orderId;
}