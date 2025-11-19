namespace Ecommerce.Domain.Orders;

public enum OrderStatus
{
    Created = 1,
    Paid = 2,
    Shipped = 3,
    Completed = 4,
    Cancelled = 5
}