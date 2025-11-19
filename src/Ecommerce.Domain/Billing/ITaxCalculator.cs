using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Billing;

public interface ITaxCalculator
{
    Money CalculateTax(Money netAmount, string countryCode);
}