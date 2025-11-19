using Ecommerce.Domain.Shared;

namespace Ecommerce.Domain.Billing;

// Domain Service
// Porque encapsula lógica de negocio (cálculo de impuestos) que no pertenece a una entidad o agregado específico.
public class StandardTaxCalculator : ITaxCalculator
{
    public Money CalculateTax(Money amount, string countryCode)
    {
        return amount.Multiply(countryCode == "US" ? 0.07m : 0.18m);
    }
}