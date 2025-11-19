namespace Ecommerce.Domain.Shared;

// Domain Exception
// Porque representa errores específicos del dominio que deben ser manejados adecuadamente, es decir no son errores técnicos sino de lógica de negocio.
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
