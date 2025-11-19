using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
    public Guid CustomerId { get; }
    public bool IsPaid { get; private set; }

    // Domain events (in-mem list) — publicación real la hace Application
    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    public Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    public void AddLine(OrderLine line)
    {
        if (IsPaid) throw new DomainException("Cannot modify a paid order.");
        _lines.Add(line);
        EnsureInvariants();
    }

    public Money Total()
    {
        var total = _lines
            .Select(l => l.LineTotal().Amount)
            .Sum();
        return new Money(total, "USD");
    }

    private void EnsureInvariants()
    {
        var total = Total();
        if (total.Amount <= 0) throw new DomainException("Order total must be > 0.");
    }

    public void MarkPaid()
    {
        if (IsPaid) throw new DomainException("Order already paid.");
        IsPaid = true;
        _events.Add(new OrderPaidEvent(Id));
    }

    // For tests / application to clear events after publishing
    public void ClearEvents() => _events.Clear();
}