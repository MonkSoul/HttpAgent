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
        builder.Services.AddHttpClient(string.Empty)
            .AddProfilerDelegatingHandler(() => builder.Environment.EnvironmentName == "Production");
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Empty(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
    }

    [Fact]
    public void PerformanceOptimization_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteExtensions.PerformanceOptimization(null!));

    [Fact]
    public void PerformanceOptimization_ReturnOK()
    {
        using var httpClient = new HttpClient();
        httpClient.PerformanceOptimization();

        Assert.NotEmpty(httpClient.DefaultRequestHeaders);
        Assert.Equal("*/*", httpClient.DefaultRequestHeaders.Accept.ToString());
        Assert.Equal("gzip, deflate, br", httpClient.DefaultRequestHeaders.AcceptEncoding.ToString());
        Assert.False(httpClient.DefaultRequestHeaders.ConnectionClose);
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
            "Request Headers: \r\n\tAccept:              application/json\r\n\tAccept-Encoding:     gzip, deflate\r\n\tContent-Type:        application/json; charset=utf-8",
            httpRequestMessage.ProfilerHeaders());
        Assert.Equal(
            "Accept:              application/json\r\nAccept-Encoding:     gzip, deflate\r\nContent-Type:        application/json; charset=utf-8",
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
    public async Task LogHttpContentAsync_ReturnOK()
    {
        Assert.Null(await HttpRemoteExtensions.ProfilerAsync(null));

        var stringContent = new StringContent("Hello World");
        Assert.Equal("Request Body (StringContent): \r\n\tHello World", await stringContent.ProfilerAsync());

        var jsonContent = JsonContent.Create(new { id = 1, name = "furion" });
        Assert.Equal("Request Body (JsonContent): \r\n\t{\"id\":1,\"name\":\"furion\"}",
            await jsonContent.ProfilerAsync());

        var byteArrayContent = new ByteArrayContent("Hello World"u8.ToArray());
        Assert.Equal("Request Body (ByteArrayContent): \r\n\tHello World", await byteArrayContent.ProfilerAsync());

        var formUrlEncodedContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("id", "1"), new KeyValuePair<string, string>("name", "Furion")
        ]);
        Assert.Equal("Request Body (FormUrlEncodedContent): \r\n\tid=1&name=Furion",
            await formUrlEncodedContent.ProfilerAsync());

        var streamStream = new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));
        Assert.Equal("Request Body (StreamContent): \r\n\t﻿测试文件内容", await streamStream.ProfilerAsync());

        var readOnlyMemoryContent = new ReadOnlyMemoryContent(new ReadOnlyMemory<byte>("Hello World"u8.ToArray()));
        Assert.Equal("Request Body (ReadOnlyMemoryContent): \r\n\tHello World",
            await readOnlyMemoryContent.ProfilerAsync());

        var multipartFormDataContent = new MultipartFormDataContent("--------------------------");
        multipartFormDataContent.Add(new StringContent("Hello World"), "text");
        multipartFormDataContent.Add(
            new StreamContent(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt"))), "file");
        Assert.Equal(
            "Request Body (MultipartFormDataContent): \r\n\t----------------------------\r\n  Content-Type: text/plain; charset=utf-8\r\n  Content-Disposition: form-data; name=text\r\n  \r\n  Hello World\r\n  ----------------------------\r\n  Content-Disposition: form-data; name=file\r\n  \r\n  ﻿测试文件内容\r\n  ------------------------------\r\n  ",
            await multipartFormDataContent.ProfilerAsync());

        var stringContent2 = new StringContent("Hello World");
        Assert.Equal("Response Body (StringContent): \r\n\tHello World",
            await stringContent2.ProfilerAsync("Response Body"));
    }
}