// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteExtensionsTests
{
    [Fact]
    public void AddProfilerDelegatingHandler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty);

        using var serviceProvider = services.BuildServiceProvider();
        var httpClientFactoryOptions = serviceProvider.GetService<IOptions<HttpClientFactoryOptions>>()?.Value;
        Assert.NotNull(httpClientFactoryOptions);
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);

        var services2 = new ServiceCollection();
        services2.AddHttpClient(string.Empty).AddProfilerDelegatingHandler();
        Assert.Contains(services2, u => u.ServiceType == typeof(ProfilerDelegatingHandler));

        using var serviceProvider2 = services2.BuildServiceProvider();
        var httpClientFactoryOptions2 = serviceProvider2.GetService<IOptions<HttpClientFactoryOptions>>()?.Value;
        Assert.NotNull(httpClientFactoryOptions2);
        Assert.Single(httpClientFactoryOptions2.HttpMessageHandlerBuilderActions);

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        builder.Services.AddHttpClient(string.Empty).AddProfilerDelegatingHandler(true);
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal("Accept:              application/json\r\nAccept-Encoding:     gzip, deflate",
            httpRequestMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerHeaders_HttpRequestMessage_WithContent_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Content = new StringContent("Furion", Encoding.UTF8, "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        Assert.Equal(
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\nContent Headers (StringContent): \r\n  Content-Type:     application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent Headers (StringContent): \r\n  Content-Type:     application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerHeaders_HttpResponseMessage_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        Assert.Equal(
            "Response Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
            httpResponseMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent-Type:        application/json",
            httpResponseMessage.ProfilerHeaders(null));
    }

    [Fact]
    public void ProfilerGeneralAndHeaders_Invalid_Parameters()
    {
        var httpResponseMessage = new HttpResponseMessage();
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.ProfilerGeneralAndHeaders(null!));
        Assert.Throws<ArgumentNullException>(() => httpResponseMessage.ProfilerGeneralAndHeaders());
    }

    [Fact]
    public void ProfilerGeneralAndHeaders_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var httpResponseMessage =
            new HttpResponseMessage { RequestMessage = httpRequestMessage, StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        Assert.Equal(
            "General: \r\n\tRequest URL:      http://localhost\r\n\tHTTP Method:      GET\r\n\tStatus Code:      200 OK\r\n\tHTTP Content:     \r\nResponse Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders());

        Assert.Equal(
            "General: \r\n\tRequest URL:          http://localhost\r\n\tHTTP Method:          GET\r\n\tStatus Code:          200 OK\r\n\tHTTP Content:         \r\n\tRequest Duration:     200ms\r\nResponse Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json",
            httpResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
                [new KeyValuePair<string, IEnumerable<string>>("Request Duration", ["200ms"])]));
    }

    [Fact]
    public void GetHostEnvironmentName_ReturnOK()
    {
        var services = new ServiceCollection();
        Assert.Null(HttpRemoteExtensions.GetHostEnvironmentName(services));

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
        Assert.Equal("Development", HttpRemoteExtensions.GetHostEnvironmentName(builder.Services));

        var builder2 = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
        Assert.Equal("Production", HttpRemoteExtensions.GetHostEnvironmentName(builder2.Services));
    }
}