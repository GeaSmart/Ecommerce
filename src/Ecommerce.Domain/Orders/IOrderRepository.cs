namespace Ecommerce.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
    Task SaveChangesAsync(); // opcional; algunos diseños prefieren UnitOfWork separado
}