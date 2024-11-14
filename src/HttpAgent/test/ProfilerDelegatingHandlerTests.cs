// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ProfilerDelegatingHandlerTests
{
    [Fact]
    public void ProfilerDelegatingHandler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Logging>>();

        var handler = new ProfilerDelegatingHandler(logger);
        Assert.NotNull(handler);
    }

    [Fact]
    public void LogRequestHeaders_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Logging>>();

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        ProfilerDelegatingHandler.LogRequestHeaders(logger, httpRequestMessage);
    }

    [Fact]
    public void LogResponseHeadersAndSummary_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Logging>>();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var httpResponseMessage =
            new HttpResponseMessage { RequestMessage = httpRequestMessage, StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        ProfilerDelegatingHandler.LogResponseHeadersAndSummary(logger, httpResponseMessage, 200);
    }

    [Fact]
    public void Log_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ProfilerDelegatingHandler.Log(null!, null!));

    [Fact]
    public void Log_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Logging>>();

        ProfilerDelegatingHandler.Log(logger, null);
        ProfilerDelegatingHandler.Log(logger, string.Empty);
        ProfilerDelegatingHandler.Log(logger, " ");
        ProfilerDelegatingHandler.Log(logger, "Furion.HttpRemote.Tests");
    }

    [Fact]
    public void IsEnabled_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        Assert.True(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));

        httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY), "value");
        Assert.True(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));

        httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY), "TRUE");
        Assert.False(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));
    }

    [Fact]
    public void ExtractSocketsHttpHandler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TryAddTransient<ProfilerDelegatingHandler>();
        services.AddHttpClient(string.Empty).AddProfilerDelegatingHandler();
        using var provider = services.BuildServiceProvider();
        var handler = provider.GetRequiredService<ProfilerDelegatingHandler>();
        Assert.Null(handler.ExtractSocketsHttpHandler());
    }

    [Fact]
    public void LogCookieContainer_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<Logging>>();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));

        ProfilerDelegatingHandler.LogCookieContainer(logger, httpRequestMessage, null);

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Uri("http://localhost"), new Cookie("cookieName", "cookieValue"));
        ProfilerDelegatingHandler.LogCookieContainer(logger, httpRequestMessage,
            new SocketsHttpHandler { CookieContainer = cookieContainer, UseCookies = true });
    }
}