﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class LongPollingManagerTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new LongPollingManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new LongPollingManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var longPollingManager = new LongPollingManager(httpRemoteService,
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost:5000")));

        Assert.NotNull(longPollingManager._httpLongPollingBuilder);
        Assert.NotNull(longPollingManager._httpRemoteService);
        Assert.NotNull(longPollingManager._dataChannel);
        Assert.Equal("UnboundedChannel`1", longPollingManager._dataChannel.GetType().Name);
        Assert.NotNull(longPollingManager.RequestBuilder);
        Assert.Null(longPollingManager.LongPollingEventHandler);
        Assert.Equal(0, longPollingManager.CurrentRetries);

        var longPollingManager2 = new LongPollingManager(httpRemoteService,
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost:5000")),
            builder => builder.SetTimeout(100));
        Assert.NotNull(longPollingManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), longPollingManager2.RequestBuilder.Timeout);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task ReceiveDataAsync_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetOnDataReceived(async _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });
        var longPollingManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        using var messageCancellationTokenSource = new CancellationTokenSource();
        var receiveDataTask = longPollingManager.ReceiveDataAsync(messageCancellationTokenSource.Token);

        for (var j = 0; j < 3; j++)
        {
            await longPollingManager._dataChannel.Writer.WriteAsync(
                new HttpResponseMessage());
        }

        await Task.Delay(200, messageCancellationTokenSource.Token);

        longPollingManager._dataChannel.Writer.Complete();

        await messageCancellationTokenSource.CancelAsync();
        await receiveDataTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ReceiveDataAsync_WithSetOnProgressChanged_Exception_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetOnDataReceived(async _ =>
            {
                i += 1;
                throw new Exception("Error");
#pragma warning disable CS0162 // 检测到不可到达的代码
                await Task.CompletedTask;
#pragma warning restore CS0162 // 检测到不可到达的代码
            });
        var longPollingManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        using var messageCancellationTokenSource = new CancellationTokenSource();
        var receiveDataTask = longPollingManager.ReceiveDataAsync(messageCancellationTokenSource.Token);

        for (var j = 0; j < 3; j++)
        {
            await longPollingManager._dataChannel.Writer.WriteAsync(
                new HttpResponseMessage());
        }

        await Task.Delay(200, messageCancellationTokenSource.Token);

        longPollingManager._dataChannel.Writer.Complete();

        await messageCancellationTokenSource.CancelAsync();
        await receiveDataTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HandleDataReceivedAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("https://furion.net"));
        var longPollingManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await longPollingManager.HandleDataReceivedAsync(null!));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HandleDataReceivedAsync_ReturnOK()
    {
        var customLongPollingEventHandler = new CustomLongPollingEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(longPollingEventHandler: customLongPollingEventHandler);
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("https://furion.net"));
        var longPollingManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        var httpResponseMessage =
            new HttpResponseMessage();
        await longPollingManager.HandleDataReceivedAsync(httpResponseMessage);

        var i = 0;
        httpLongPollingBuilder.SetOnDataReceived(async _ =>
        {
            i++;
            await Task.CompletedTask;
        });

        var longPollingManager2 = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);
        await longPollingManager2.HandleDataReceivedAsync(httpResponseMessage);

        Assert.Equal(1, i);
        Assert.Equal(0, customLongPollingEventHandler.counter);

        httpLongPollingBuilder.SetEventHandler<CustomLongPollingEventHandler>();
        var longPollingManager3 = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);
        await longPollingManager3.HandleDataReceivedAsync(httpResponseMessage);

        Assert.Equal(1, customLongPollingEventHandler.counter);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void ShouldTerminatePolling_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var longPollingManager = new LongPollingManager(httpRemoteService,
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost:5000")));

        Assert.Throws<ArgumentNullException>(() => longPollingManager.ShouldTerminatePolling(null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void ShouldTerminatePolling_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var longPollingManager = new LongPollingManager(httpRemoteService,
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost:5000")));

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        Assert.False(longPollingManager.ShouldTerminatePolling(httpResponseMessage));
        Assert.Equal(0, longPollingManager.CurrentRetries);

        var httpResponseMessage2 = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage2.Headers.TryAddWithoutValidation("X-End-Of-Stream", "1");
        Assert.True(longPollingManager.ShouldTerminatePolling(httpResponseMessage2));
        Assert.Equal(0, longPollingManager.CurrentRetries);

        var httpResponseMessage3 = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
        Assert.False(longPollingManager.ShouldTerminatePolling(httpResponseMessage3));
        Assert.Equal(1, longPollingManager.CurrentRetries);
    }

    [Fact]
    public async Task Start_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                });
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        longPollingManagerManager.Start();

        Assert.Equal(5, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                });
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            longPollingManagerManager.Start(cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_EventHandle_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        var i = 0;
        var customLongPollingEventHandler = new CustomLongPollingEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(longPollingEventHandler: customLongPollingEventHandler);
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                }).SetEventHandler<CustomLongPollingEventHandler>();
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        longPollingManagerManager.Start();

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_Exception_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            string message;

            await Task.Delay(50, context.RequestAborted);

            throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                }).SetPollingInterval(TimeSpan.FromMilliseconds(100)).SetMaxRetries(10);
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        longPollingManagerManager.Start();

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                });
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        await longPollingManagerManager.StartAsync();

        Assert.Equal(5, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                });
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await longPollingManagerManager.StartAsync(cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_EventHandle_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            var message = $"Message at {DateTime.UtcNow}\n\n";

            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync();

        var i = 0;
        var customLongPollingEventHandler = new CustomLongPollingEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(longPollingEventHandler: customLongPollingEventHandler);
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                }).SetEventHandler<CustomLongPollingEventHandler>();
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        await longPollingManagerManager.StartAsync();

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_Exception_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;

            string message;

            await Task.Delay(50, context.RequestAborted);

            throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetOnDataReceived(
                async data =>
                {
                    i++;
                    await Task.CompletedTask;
                }).SetPollingInterval(TimeSpan.FromMilliseconds(100)).SetMaxRetries(10);
        var longPollingManagerManager = new LongPollingManager(httpRemoteService, httpLongPollingBuilder);

        await longPollingManagerManager.StartAsync();

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}