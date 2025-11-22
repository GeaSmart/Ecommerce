using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Orders;

public sealed class Customer
{
    public Guid Id { get; }
    public Money CreditLimit { get; }
    public Money CurrentDebt { get; private set; }

    public Customer(Guid id, Money creditLimit)
    {
        Id = id;
        CreditLimit = creditLimit;
        CurrentDebt = new Money(0, creditLimit.Currency);
    }

    public Money AvailableCredit()
    {
        return new Money(
            CreditLimit.Amount - CurrentDebt.Amount,
            CreditLimit.Currency
        );
    }
}