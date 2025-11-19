using Ecommerce.Domain.Shared;
using static System.Net.Mime.MediaTypeNames;

namespace Ecommerce.Domain.Orders;

// Aggregate Root
// Porque es la raíz del agregado Order y controla su consistencia, es decir controla invariantes y genera domain events
// Es la unica entidad accesible desde fuera del agregado.
// Nota: la publicación real de eventos la orquesta Application.
public sealed class Order
{
    public Guid Id { get; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

    public Guid CustomerId { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.Created;
    public bool IsPaid => Status == OrderStatus.Paid;

    private readonly List<IDomainEvent> _events = new();
    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    public Order(Guid id, Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Order must have a customer.");

        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Created;
    }

    public void AddLine(OrderLine line)
    {
        if (IsPaid)
            throw new DomainException("Cannot modify a paid order.");

        // Invariante: no duplicar productos
        if (_lines.Any(l => l.ProductId == line.ProductId))
            throw new DomainException("Product already added to the order.");

        _lines.Add(line);
        EnsureInvariants();
    }

    public Money Total()
    {
        var amount = _lines.Sum(l => l.LineTotal().Amount);
        return new Money(amount, "USD");
    }

    private void EnsureInvariants()
    {
        // Total > 0  
        var total = Total();
        if (total.Amount <= 0)
            throw new DomainException("Order total must be greater than 0.");

        // No permitir estado inválido sin líneas
        if (Status != OrderStatus.Created && !_lines.Any())
            throw new DomainException("Order cannot transition without items.");
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.Created)
            throw new DomainException("Only created orders can be paid.");

        if (!_lines.Any())
            throw new DomainException("Cannot pay an empty order.");

        Status = OrderStatus.Paid;
        _events.Add(new OrderPaidEvent(Id));
    }

    public void MarkShipped()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException("Order must be paid before shipping.");

        Status = OrderStatus.Shipped;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainException("Order must be shipped before completing.");

        Status = OrderStatus.Completed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new DomainException("Completed orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void ClearEvents() => _events.Clear();
}
