using Ecommerce.Domain.Orders;
using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Order.Services;

public interface ICreditPolicy
{
    bool CanPlaceOrder(Customer customer, Money orderTotal);
}