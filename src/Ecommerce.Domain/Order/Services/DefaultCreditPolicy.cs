using Ecommerce.Domain.Orders;
using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Order.Services;

// Domain Service
// Porque encapsula lógica de negocio (linea de credito) que no pertenece a una entidad o agregado específico.
public sealed class DefaultCreditPolicy : ICreditPolicy
{
    public bool CanPlaceOrder(Customer customer, Money orderTotal)
    {
        // Lógica pura del dominio
        return customer.AvailableCredit().Amount >= orderTotal.Amount;
    }
}