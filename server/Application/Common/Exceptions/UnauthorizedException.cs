namespace Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string? message = null) : base(message)
    { }
}