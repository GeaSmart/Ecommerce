namespace Ecommerce.Domain.Shared;

public sealed record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (other.Currency != Currency) throw new DomainException("Different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public void EnsurePositive()
    {
        if (Amount <= 0) throw new DomainException("Amount must be positive.");
    }
}
