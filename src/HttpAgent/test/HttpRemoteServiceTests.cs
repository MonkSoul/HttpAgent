// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServiceTests(ITestOutputHelper output)
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddHttpClient("test", client =>
        {
            client.BaseAddress = new Uri("http://localhost/test/");
        });

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Logging>>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        var httpContentProcessorFactory = new HttpContentProcessorFactory(null);
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, null);

        Assert.Throws<ArgumentNullException>(() => new HttpRemoteService(null!, null!, null!, null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRemoteService(serviceProvider, null!, null!, null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRemoteService(serviceProvider, logger, null!, null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRemoteService(serviceProvider, logger, httpClientFactory, null!, null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRemoteService(serviceProvider, logger, httpClientFactory, httpContentProcessorFactory, null!,
                null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpRemoteService(serviceProvider, logger, httpClientFactory, httpContentProcessorFactory,
                httpContentConverterFactory, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.NotNull(httpRemoteService);
        Assert.NotNull(httpRemoteService.ServiceProvider);
        Assert.NotNull(httpRemoteService.RemoteOptions);
        Assert.NotNull(httpRemoteService._logger);
        Assert.NotNull(httpRemoteService._httpClientFactory);
        Assert.NotNull(httpRemoteService._httpContentProcessorFactory);
        Assert.NotNull(httpRemoteService._httpContentConverterFactory);

        serviceProvider.Dispose();
    }

    [Fact]
    public void CreateHttpClientWithDefaults_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = httpRemoteService.CreateHttpClientWithDefaults(null!);
        });

        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/")).SetHttpClientProvider(() =>
                (null!, null));

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = httpRemoteService.CreateHttpClientWithDefaults(httpRequestBuilder);
        });

        serviceProvider.Dispose();
    }

    [Fact]
    public void CreateHttpClientWithDefaults_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/"));

        var httpClientPooling = httpRemoteService.CreateHttpClientWithDefaults(httpRequestBuilder);
        var httpClient = httpClientPooling.Instance;

        Assert.NotNull(httpClient);
        Assert.Null(httpClientPooling.Release);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient.DefaultRequestHeaders.GetValues("User-Agent").First());

        var httpRequestBuilder2 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute))
                .SetHttpClientName("test");
        var httpClientPooling2 = httpRemoteService.CreateHttpClientWithDefaults(httpRequestBuilder2);
        var httpClient2 = httpClientPooling2.Instance;
        Assert.NotNull(httpClient2);
        Assert.NotNull(httpClient2.BaseAddress);
        Assert.Equal("http://localhost/test/", httpClient2.BaseAddress.ToString());
        Assert.Null(httpClientPooling2.Release);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient2.DefaultRequestHeaders.GetValues("User-Agent").First());

        var httpClient3 = new HttpClient { BaseAddress = new Uri("http://localhost/custom") };
        var httpRequestBuilder3 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute))
                .SetHttpClientProvider(() => (httpClient3, client =>
                {
                    client.Dispose();
                }));

        var httpClientPooling3 = httpRemoteService.CreateHttpClientWithDefaults(httpRequestBuilder3);
        var httpClient4 = httpClientPooling3.Instance;
        Assert.Equal(httpClient3, httpClient4);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient4.DefaultRequestHeaders.GetValues("User-Agent").First());
        Assert.NotNull(httpClientPooling3.Release);
        httpClientPooling3.Release(httpClient3);

        serviceProvider.Dispose();
    }

    [Fact]
    public void CreateHttpClientPooling_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = httpRemoteService.CreateHttpClientPooling(null!);
        });

        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/")).SetHttpClientProvider(() =>
                (null!, null));

        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = httpRemoteService.CreateHttpClientPooling(httpRequestBuilder);
        });

        serviceProvider.Dispose();
    }

    [Fact]
    public void CreateHttpClientPooling_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/"));

        var httpClientPooling = httpRemoteService.CreateHttpClientPooling(httpRequestBuilder);
        var httpClient = httpClientPooling.Instance;

        Assert.NotNull(httpClient);
        Assert.Null(httpClientPooling.Release);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient.DefaultRequestHeaders.GetValues("User-Agent").First());

        var httpRequestBuilder2 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute))
                .SetHttpClientName("test");
        var httpClientPooling2 = httpRemoteService.CreateHttpClientPooling(httpRequestBuilder2);
        var httpClient2 = httpClientPooling2.Instance;
        Assert.NotNull(httpClient2);
        Assert.NotNull(httpClient2.BaseAddress);
        Assert.Equal("http://localhost/test/", httpClient2.BaseAddress.ToString());
        Assert.Null(httpClientPooling2.Release);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient2.DefaultRequestHeaders.GetValues("User-Agent").First());

        var httpClient3 = new HttpClient { BaseAddress = new Uri("http://localhost/custom") };
        var httpRequestBuilder3 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute))
                .SetHttpClientProvider(() => (httpClient3, client =>
                {
                    client.Dispose();
                }));

        var httpClientPooling3 = httpRemoteService.CreateHttpClientPooling(httpRequestBuilder3);
        var httpClient4 = httpClientPooling3.Instance;
        Assert.Equal(httpClient3, httpClient4);
        Assert.Equal("HttpAgent/1.0.3.1", httpClient4.DefaultRequestHeaders.GetValues("User-Agent").First());
        Assert.NotNull(httpClientPooling3.Release);
        httpClientPooling3.Release(httpClient3);

        serviceProvider.Dispose();
    }

    [Fact]
    public void AddDefaultUserAgentHeader_ReturnOK()
    {
        var httpClient = new HttpClient();
        HttpRemoteService.AddDefaultUserAgentHeader(httpClient);

        Assert.Equal("HttpAgent/1.0.3.1", httpClient.DefaultRequestHeaders.GetValues("User-Agent").First());

        var httpClient2 = new HttpClient();
        httpClient2.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "furion");
        HttpRemoteService.AddDefaultUserAgentHeader(httpClient2);
        Assert.Equal("furion", httpClient2.DefaultRequestHeaders.GetValues("User-Agent").First());
    }

    [Fact]
    public void HandlePreSendRequest_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnPreSendRequest(_ => throw new Exception("出错了"));

        HttpRemoteService.HandlePreSendRequest(httpRequestBuilder, new CustomRequestEventHandler(),
            new HttpRequestMessage());
    }

    [Fact]
    public void HandlePostSendRequest_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnPostSendRequest(_ => throw new Exception("出错了"));

        HttpRemoteService.HandlePostSendRequest(httpRequestBuilder, new CustomRequestEventHandler(),
            new HttpResponseMessage());
    }

    [Fact]
    public void HandleSendRequestFailed_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetOnSendRequestFailed((_, _) => throw new Exception("出错了"));

        HttpRemoteService.HandleSendRequestFailed(httpRequestBuilder, new CustomRequestEventHandler(),
            new Exception("出错了"),
            new HttpResponseMessage());
    }

    [Fact]
    public async Task InvokeStatusCodeHandlersAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpRemoteService.InvokeStatusCodeHandlersAsync(null!, null!));

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpRemoteService.InvokeStatusCodeHandlersAsync(httpRequestBuilder, null!));
    }

    [Fact]
    public async Task InvokeStatusCodeHandlersAsync_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        await HttpRemoteService.InvokeStatusCodeHandlersAsync(httpRequestBuilder, httpResponseMessage);

        var i = 0;
        httpRequestBuilder.WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            })
            .WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            });

        await HttpRemoteService.InvokeStatusCodeHandlersAsync(httpRequestBuilder, httpResponseMessage);
        Assert.Equal(2, i);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };
        var j = 0;
        httpRequestBuilder2.WithStatusCodeHandler(200, (_, _) =>
        {
            j++;
            return Task.CompletedTask;
        });
        await HttpRemoteService.InvokeStatusCodeHandlersAsync(httpRequestBuilder2, httpResponseMessage2);
        Assert.Equal(0, j);
    }

    [Fact]
    public void InvokeStatusCodeHandlers_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpRemoteService.InvokeStatusCodeHandlers(null!, null!));

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() =>
            HttpRemoteService.InvokeStatusCodeHandlers(httpRequestBuilder, null!));
    }

    [Fact]
    public void InvokeStatusCodeHandlers_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        HttpRemoteService.InvokeStatusCodeHandlers(httpRequestBuilder, httpResponseMessage);

        var i = 0;
        httpRequestBuilder.WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                output.WriteLine(i.ToString());
                return Task.CompletedTask;
            })
            .WithStatusCodeHandler(200, (_, _) =>
            {
                i++;
                output.WriteLine(i.ToString());
                return Task.CompletedTask;
            });

        HttpRemoteService.InvokeStatusCodeHandlers(httpRequestBuilder, httpResponseMessage);
        // Assert.Equal(2, i);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };
        var j = 0;
        httpRequestBuilder2.WithStatusCodeHandler(200, (_, _) =>
        {
            j++;
            output.WriteLine(j.ToString());
            return Task.CompletedTask;
        });
        HttpRemoteService.InvokeStatusCodeHandlers(httpRequestBuilder2, httpResponseMessage2);
        // Assert.Equal(0, j);
    }

    [Fact]
    public async Task SendCoreAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await httpRemoteService.SendCoreAsync(null!, HttpCompletionOption.ResponseContentRead, null, null);
        });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await httpRemoteService.SendCoreAsync(new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/")),
                HttpCompletionOption.ResponseContentRead, null, null);
        });
        Assert.Equal("Both `sendAsyncMethod` and `sendMethod` cannot be null.", exception.Message);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));
        var (httpResponseMessage, requestDuration) = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.NotNull(httpResponseMessage);
        Assert.True(requestDuration > 0);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        var (httpResponseMessage2, requestDuration2) = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, default, (httpClient, httpRequestMessage, option, token) =>
                httpClient.Send(httpRequestMessage, option, token));

        Assert.NotNull(httpResponseMessage2);
        Assert.True(requestDuration2 > 0);
        Assert.True(httpResponseMessage2.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage2.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage2.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_HttpError_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));
        var (httpResponseMessage, requestDuration) = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.NotNull(httpResponseMessage);
        Assert.True(requestDuration > 0);
        Assert.False(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(500, (int)httpResponseMessage.StatusCode);
        Assert.StartsWith("System.Exception: Test exception", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_EnsureSuccessStatusCode_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .EnsureSuccessStatusCode();

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_Filter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .SetOnPreSendRequest(_ =>
            {
                i += 1;
            })
            .SetOnPostSendRequest(_ =>
            {
                i += 1;
            });

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.Equal(2, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_FilterException_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .SetOnPreSendRequest(_ =>
            {
                i += 1;
            })
            .SetOnSendRequestFailed((_, _) =>
            {
                i += 1;
            })
            .SetOnPostSendRequest(_ =>
            {
                i += 1;
            }).EnsureSuccessStatusCode();

        try
        {
            _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
        }
        catch
        {
            // ...
        }

        Assert.Equal(3, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_EventHandler_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        // 测试代码
        var customRequestEventHandler = new CustomRequestEventHandler();
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService(customRequestEventHandler);
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .SetOnPreSendRequest(_ =>
            {
                i += 1;
            })
            .SetOnSendRequestFailed((_, _) =>
            {
                i += 1;
            })
            .SetOnPostSendRequest(_ =>
            {
                i += 1;
            }).SetEventHandler<CustomRequestEventHandler>().EnsureSuccessStatusCode();

        try
        {
            _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
        }
        catch
        {
            // ...
        }

        Assert.Equal(3, i);
        Assert.Equal(3, customRequestEventHandler.counter);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_HttpClientRelease_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .SetHttpClientProvider(() => (new HttpClient(), client =>
            {
                i += 1;
                client.Dispose();
            }));

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.Equal(1, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_HttpClientRelease_HttpClientPoolingEnabled_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", () =>
        {
            throw new Exception("Test exception");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello World!";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
            .SetHttpClientProvider(() => (new HttpClient(), client =>
            {
                i += 1;
                client.Dispose();
            })).UseHttpClientPool();

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_Timeout_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(500);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetTimeout(200);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
        });

        // 超时为 0
        var httpRequestBuilder2 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetTimeout(0);
        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder2,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_UnknownServer_ReturnOK()
    {
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("https://test-unknown-server.com/test"));

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var (httpResponseMessage, _) = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
            Assert.Null(httpResponseMessage);
        });

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_UnknownServer_EnsureSuccessStatusCode_ReturnOK()
    {
        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("https://test-unknown-server.com/test"))
                .EnsureSuccessStatusCode();

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var (httpResponseMessage, _) = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default);
            Assert.Null(httpResponseMessage);
        });

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(500);
            return "Hello World!";
        });

        await app.StartAsync();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(200);

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                    httpClient.SendAsync(httpRequestMessage, option, token), default, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_HandleDisposables_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async (IFormFile file) =>
        {
            await Task.Delay(50);
            return file.FileName;
        });

        await app.StartAsync();

        // 测试代码
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test")).SetMultipartContent(
                mBuilder =>
                {
                    mBuilder.AddFileStream(fileFullName, "file");
                });

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.NotNull(httpRequestBuilder.Disposables);
        Assert.Empty(httpRequestBuilder.Disposables);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_MessagePack_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapPost("/test", async context =>
        {
            // 获取请求体中的内容
            context.Request.Body.Position = 0;

            using var memoryStream = new MemoryStream();

            await context.Request.Body.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();

            // 反序列化为 MyClass 对象
            var obj = MessagePackSerializer.Deserialize<MessagePackModel1>(bytes);

            await context.Response.WriteAsync($"{obj.Id} {obj.Name}");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test")).SetRawContent(
                    new MessagePackModel1 { Id = 1, Name = "Furion" }, "application/msgpack")
                .AddHttpContentProcessors(() => [new MessagePackContentProcessor()]).EnsureSuccessStatusCode();

        var result = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        var str = await result.ResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 Furion", str);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_WithStatusCodeHandlers_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).WithStatusCodeHandler(200,
                (r, t) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, (httpClient, httpRequestMessage, option, token) =>
                httpClient.SendAsync(httpRequestMessage, option, token), default);

        Assert.Equal(1, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendCoreAsync_WithStatusCodeHandlers2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).WithStatusCodeHandler(200,
                (r, t) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        _ = await httpRemoteService.SendCoreAsync(httpRequestBuilder,
            HttpCompletionOption.ResponseContentRead, default, (httpClient, httpRequestMessage, option, token) =>
                httpClient.Send(httpRequestMessage, option, token));

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void DynamicCreateHttpRemoteResult_Invalid_Parameters()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            HttpRemoteService.DynamicCreateHttpRemoteResult(typeof(string), null!, null, 100));

        Assert.Equal(
            $"`{typeof(string)}` type is not assignable from `{typeof(HttpRemoteResult<>)}`. (Parameter 'httpRemoteResultType')",
            exception.Message);
    }

    [Fact]
    public void DynamicCreateHttpRemoteResult_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content = new StringContent("Hello World!");

        var httpRemoteResult = HttpRemoteService.DynamicCreateHttpRemoteResult(typeof(HttpRemoteResult<string>),
            httpResponseMessage, "Hello World!", 50) as HttpRemoteResult<string>;

        Assert.NotNull(httpRemoteResult);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
    }

    [Fact]
    public void CheckContentLengthWithinLimit_Invalid_Parameters()
    {
        var httpRequestBuilder =
            HttpRequestBuilder.Get("http://localhost:5000/test").SetMaxResponseContentBufferSize(10);

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content = new StringContent("Hello World!");

        var exception = Assert.Throws<HttpRequestException>(() =>
            HttpRemoteService.CheckContentLengthWithinLimit(httpRequestBuilder, httpResponseMessage));
        Assert.Equal("Cannot write more bytes to the buffer than the configured maximum buffer size: 10.",
            exception.Message);
    }

    [Fact]
    public void CheckContentLengthWithinLimit_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.Get("http://localhost:5000/test");
        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content = new StringContent("Hello World!");

        HttpRemoteService.CheckContentLengthWithinLimit(httpRequestBuilder, httpResponseMessage);

        var httpRequestBuilder2 =
            HttpRequestBuilder.Get("http://localhost:5000/test").SetMaxResponseContentBufferSize(30);
        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage2.Content = new StringContent("Hello World!");

        HttpRemoteService.CheckContentLengthWithinLimit(httpRequestBuilder2, httpResponseMessage2);
    }

    [Fact]
    public async Task SendAsString_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.SendAsString(HttpRequestBuilder.Get($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.SendAsString(HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsStream_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream =
            httpRemoteService.SendAsStream(HttpRequestBuilder.Get($"http://localhost:{port}/test"));
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.SendAsStream(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsByteArray_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.SendAsByteArray(HttpRequestBuilder.Get($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.SendAsByteArray(HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsStringAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.SendAsStringAsync(HttpRequestBuilder.Get($"http://localhost:{port}/test"));
        var str2 = await httpRemoteService.SendAsStringAsync(HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsStreamAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream =
            await httpRemoteService.SendAsStreamAsync(HttpRequestBuilder.Get($"http://localhost:{port}/test"));
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.SendAsStreamAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsByteArrayAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.SendAsByteArrayAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"));
        var bytes2 = await httpRemoteService.SendAsByteArrayAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"),
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}