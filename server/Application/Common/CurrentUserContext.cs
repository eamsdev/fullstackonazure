namespace Application.Common;

public class CurrentUserContext
{
    public CurrentUserContext(string? userId)
    {
        UserId = userId;
    }

    public string? UserId { get; }
}