using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Orders;

public sealed class OrderLine
{
    public Guid ProductId { get; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; }

    public OrderLine(Guid productId, int quantity, Money unitPrice)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be > 0");
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Money LineTotal() => UnitPrice.Multiply(Quantity);
}
