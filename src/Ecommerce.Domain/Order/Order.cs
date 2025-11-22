using Ecommerce.Domain.Order.Events;
using Ecommerce.Domain.Order.Services;
using Ecommerce.Domain.Shared;

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

    // Constructor usado SOLO internamente
    private Order(Guid id, Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Order must have a customer.");

        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Created;
    }

    public static Order Create(
        Customer customer,
        IEnumerable<OrderLine> lines,
        ICreditPolicy creditPolicyService)
    {
        if (customer.Id == Guid.Empty)
            throw new DomainException("Customer is required.");

        if (lines == null || !lines.Any())
            throw new DomainException("An order must contain at least one line.");

        // Calcular total (usando Money VO)
        var total = lines
            .Select(l => l.LineTotal())   // IEnumerable<Money>
            .Aggregate((a, b) => a.Add(b));   // Money + Money

        // Política de crédito (domain service)
        if (!creditPolicyService.CanPlaceOrder(customer, total))
            throw new DomainException("Insufficient credit for this order.");

        // Crear el agregado
        var order = new Order(Guid.NewGuid(), customer.Id);

        // Agregar líneas
        foreach (var line in lines)
            order.AddLine(line);

        // Verificar invariantes
        order.EnsureInvariants();

        // Generar evento de dominio
        order.AddEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public void AddLine(OrderLine line)
    {
        if (line is null)
            throw new DomainException("Order line cannot be null.");

        if (IsPaid)
            throw new DomainException("Cannot modify a paid order.");

        // Invariante: no duplicar productos
        if (_lines.Any(l => l.ProductId == line.ProductId))
            throw new DomainException($"Product {line.ProductId} is already in the order.");

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
        var total = Total();
        if (total.Amount <= 0)
            throw new DomainException("Order total must be greater than 0.");

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

    private void AddEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            throw new ArgumentNullException(nameof(domainEvent));

        _events.Add(domainEvent);
    }
}
