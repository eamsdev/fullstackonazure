using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration;

public sealed class TestContainer : IDisposable
{
    private readonly IServiceScope _serviceScope;
    
    public HttpClient HttpClient { get; }
    public CustomWebApplicationFactory WebApplicationFactory { get; }
    
    public TestContainer()
    {
        WebApplicationFactory = new CustomWebApplicationFactory();
        _serviceScope = WebApplicationFactory.Services.CreateScope();
        HttpClient = WebApplicationFactory.CreateClient();
    }

    public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

    public void Dispose()
    {
        HttpClient.Dispose();
        _serviceScope.Dispose();
        WebApplicationFactory.Dispose();
    }
}