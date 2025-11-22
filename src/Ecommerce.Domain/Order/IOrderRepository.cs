namespace Ecommerce.Domain.Orders;

// Repositiory Interface
// Porque define el contrato para persistir y recuperar Order (aggregate root).
// Nota: la implementación concreta se encuentra en Ecommerce.Infrastructure.
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task AddAsync(Order order);
    Task SaveChangesAsync(); // opcional; algunos diseños prefieren UnitOfWork separado
}