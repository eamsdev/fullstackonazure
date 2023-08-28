using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class TestContainer : IDisposable
{
    private readonly IServiceScope _serviceScope;
    private readonly CustomWebApplicationFactory _webApplicationFactory;

    public TestContainer()
    {
        _webApplicationFactory = new CustomWebApplicationFactory();
        _serviceScope = _webApplicationFactory.Services.CreateScope();
    }

    public IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;
    
    public HttpClient CreateClient() => _webApplicationFactory.CreateClient();

    public void Dispose()
    {
        _serviceScope.Dispose();
        _webApplicationFactory.Dispose();
    }
}