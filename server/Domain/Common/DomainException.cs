namespace Domain.Common;

public class InvalidDomainOperationException : Exception
{
    public InvalidDomainOperationException(string? message = null) : base(message)
    { }
}