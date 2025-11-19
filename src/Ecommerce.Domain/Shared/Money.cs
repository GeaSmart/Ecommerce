namespace Ecommerce.Domain.Shared;

// Value object representing money with amount and currency
// Porque Money es inmutable y comparado por valor
public sealed record Money(decimal Amount, string Currency)
{
    public Money Add(Money other)
    {
        if (other.Currency != Currency) throw new DomainException("Different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        return new Money(Amount * factor, Currency);
    }

    public void EnsurePositive()
    {
        if (Amount <= 0) throw new DomainException("Amount must be positive.");
    }
}
