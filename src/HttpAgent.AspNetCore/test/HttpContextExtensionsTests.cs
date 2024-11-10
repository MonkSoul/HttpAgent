// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextExtensionsTests
{
    [Fact]
    public async Task GetFullRequestUrl_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        string[] urls = ["--urls", $"http://localhost:{port}"];
        var builder = WebApplication.CreateBuilder(urls);

        await using var app = builder.Build();

        app.MapGet("/test", async httpContext =>
        {
            var urlAddress = httpContext.Request.GetFullRequestUrl();
            Assert.Equal($"http://localhost:{port}/test", urlAddress);

            await Task.CompletedTask;
        });

        await app.StartAsync();

        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(nameof(HttpContextExtensionsTests));

        var httpResponseMessage = await httpClient.GetAsync($"http://localhost:{port}/test");
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CreateRequestBuilderAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            var httpRequestBuilder =
                await context.CreateRequestBuilderAsync(httpMethod, requestUri,
                    u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder.Timeout);
            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(2, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());


            var httpRequestBuilder2 =
                await context.CreateRequestBuilderAsync(requestUri, u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder2.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder2.Timeout);
            Assert.NotNull(httpRequestBuilder2.Headers);
            Assert.Equal(2, httpRequestBuilder2.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder2.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder2.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder2.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder2.Headers.ElementAt(1).Value.First());

            var httpRequestBuilder3 =
                await context.CreateRequestBuilderAsync(HttpMethod.Get, $"http://localhost:{port}",
                    u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder3.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder3.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder3.Timeout);
            Assert.NotNull(httpRequestBuilder3.Headers);
            Assert.Equal(2, httpRequestBuilder3.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder3.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder3.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder3.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder3.Headers.ElementAt(1).Value.First());

            var httpRequestBuilder4 =
                await context.CreateRequestBuilderAsync($"http://localhost:{port}",
                    u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder4.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder4.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder4.Timeout);
            Assert.NotNull(httpRequestBuilder4.Headers);
            Assert.Equal(2, httpRequestBuilder4.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder4.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder4.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder4.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder4.Headers.ElementAt(1).Value.First());

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task CreateRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        builder.Services.AddHttpClient();
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            var httpMethod = Helpers.ParseHttpMethod(context.Request.Method);
            var requestUri = new Uri($"http://localhost:{port}");
            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder = context.CreateRequestBuilder(httpMethod, requestUri,
                u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder.Timeout);
            Assert.NotNull(httpRequestBuilder.Headers);
            Assert.Equal(2, httpRequestBuilder.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder.Headers.ElementAt(1).Value.First());

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder2 =
                context.CreateRequestBuilder(requestUri, u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder2.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder2.Timeout);
            Assert.NotNull(httpRequestBuilder2.Headers);
            Assert.Equal(2, httpRequestBuilder2.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder2.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder2.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder2.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder2.Headers.ElementAt(1).Value.First());

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder3 = context.CreateRequestBuilder(HttpMethod.Get, $"http://localhost:{port}",
                u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder3.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder3.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder3.Timeout);
            Assert.NotNull(httpRequestBuilder3.Headers);
            Assert.Equal(2, httpRequestBuilder3.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder3.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder3.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder3.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder3.Headers.ElementAt(1).Value.First());

            // ReSharper disable once MethodHasAsyncOverload
            var httpRequestBuilder4 = context.CreateRequestBuilder($"http://localhost:{port}",
                u => u.SetTimeout(TimeSpan.FromSeconds(5)));

            Assert.Equal(HttpMethod.Get, httpRequestBuilder4.Method);
            Assert.Equal($"http://localhost:{port}/", httpRequestBuilder4.RequestUri?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(5), httpRequestBuilder4.Timeout);
            Assert.NotNull(httpRequestBuilder4.Headers);
            Assert.Equal(2, httpRequestBuilder4.Headers.Count);
            Assert.Equal("X-Original-URL", httpRequestBuilder4.Headers.ElementAt(0).Key);
            Assert.Equal($"http://localhost:{port}/test", httpRequestBuilder4.Headers.ElementAt(0).Value.First());
            Assert.Equal("Host", httpRequestBuilder4.Headers.ElementAt(1).Key);
            Assert.Equal($"localhost:{port}", httpRequestBuilder4.Headers.ElementAt(1).Value.First());

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsAsync<string>(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsAsync<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str2 = await context.ForwardAsAsync<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str3 = await context.ForwardAsAsync<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var str4 = await context.ForwardAsAsync<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Contains(httpResponseMessage.Headers, x => x.Key == "Framework");
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var model1 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var model2 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var model3 = await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var model4 =
                await context.ForwardAsAsync<HttpRemoteAspNetCoreModel1>(
                    $"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " + model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " + model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var str1 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var str2 = await context.ForwardAsAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var str3 = await context.ForwardAsAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var str4 =
                await context.ForwardAsAsync<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public void ForwardAs_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.ForwardAs<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().ForwardAs<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = context.ForwardAs<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str2 = context.ForwardAs<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var str3 = context.ForwardAs<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var str4 = context.ForwardAs<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(str + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var model1 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var model2 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var model3 = context.ForwardAs<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var model4 =
                context.ForwardAs<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " + model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " + model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var str1 = context.ForwardAs<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var str2 = context.ForwardAs<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var str3 = context.ForwardAs<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var str4 =
                context.ForwardAs<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_Redirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var str = await context.ForwardAsAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            await context.Response.WriteAsync(str!);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Redirect", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsAsync_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async context =>
        {
            var stream = await context.ForwardAsAsync<Stream>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            await stream!.CopyToAsync(context.Response.Body);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        Assert.True(stream.CanRead);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsync<string>(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsync<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var result1 = await context.ForwardAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var result2 = await context.ForwardAsync<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var result3 = await context.ForwardAsync<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var result4 = await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(result1.Result + " " + result2.Result + " " + result3.Result + " " +
                                              result4.Result);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var result1 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var result2 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var result3 = await context.ForwardAsync<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var result4 =
                await context.ForwardAsync<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(result1.Result!.Id + " " + result1.Result.Name + " " +
                                              result2.Result!.Id + " " + result2.Result.Name +
                                              " " + result3.Result!.Id + " " + result3.Result.Name + " " +
                                              result4.Result!.Id + " " +
                                              result4.Result.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var result1 = await context.ForwardAsync<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var result2 = await context.ForwardAsync<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var result3 = await context.ForwardAsync<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var result4 =
                await context.ForwardAsync<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(result1.Result + " " + result2.Result + " " + result3.Result + " " +
                                              result4.Result);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public void Forward_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.Forward<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().Forward<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task Forward_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var result1 = context.Forward<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var result2 = context.Forward<string>(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var result3 = context.Forward<string>(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var result4 = context.Forward<string>($"http://localhost:{port}/HttpRemote/Request1");

            await context.Response.WriteAsync(result1.Result + " " + result2.Result + " " + result3.Result + " " +
                                              result4.Result);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task Forward_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var result1 = context.Forward<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var result2 = context.Forward<HttpRemoteAspNetCoreModel1>(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var result3 = context.Forward<HttpRemoteAspNetCoreModel1>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var result4 =
                context.Forward<HttpRemoteAspNetCoreModel1>($"http://localhost:{port}/HttpRemote/Request2");

            await context.Response.WriteAsync(result1.Result!.Id + " " + result1.Result.Name + " " +
                                              result2.Result!.Id + " " + result2.Result.Name +
                                              " " + result3.Result!.Id + " " + result3.Result.Name + " " +
                                              result4.Result!.Id + " " +
                                              result4.Result.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task Forward_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var result1 = context.Forward<string>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var result2 = context.Forward<string>(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var result3 = context.Forward<string>(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var result4 =
                context.Forward<string>($"http://localhost:{port}/HttpRemote/Request6");

            await context.Response.WriteAsync(result1.Result + " " + result2.Result + " " + result3.Result + " " +
                                              result4.Result);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_Redirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var result1 = await context.ForwardAsync<string>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            await context.Response.WriteAsync(result1.Result!);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Redirect", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async context =>
        {
            var result1 = await context.ForwardAsync<Stream>(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            await result1.Result!.CopyToAsync(context.Response.Body);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        Assert.True(stream.CanRead);

        await app.StopAsync();
    }


    [Fact]
    public async Task ForwardAsync_HttpResponse_Invalid_Parameters()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await HttpContextExtensions.ForwardAsync(null!, null!, (Uri?)null));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new DefaultHttpContext().ForwardAsync(null!, (Uri?)null));
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var httpResponseMessage2 =
                await context.ForwardAsync(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            var httpResponseMessage4 = await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request1");

            var str1 = await httpResponseMessage1.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));

            var httpResponseMessage2 = await context.ForwardAsync(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            var httpResponseMessage4 =
                await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request2");

            var model1 = await httpResponseMessage1.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model2 = await httpResponseMessage2.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model3 = await httpResponseMessage3.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model4 = await httpResponseMessage4.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " +
                                              model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " +
                                              model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));

            var httpResponseMessage2 = await context.ForwardAsync(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            var httpResponseMessage3 = await context.ForwardAsync(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            var httpResponseMessage4 =
                await context.ForwardAsync($"http://localhost:{port}/HttpRemote/Request6");

            var str1 = await httpResponseMessage1.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public void Forward_HttpResponse_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            HttpContextExtensions.Forward<string>(null!, null!, (Uri?)null));
        Assert.Throws<ArgumentNullException>(() =>
            new DefaultHttpContext().Forward<string>(null!, (Uri?)null));
    }

    [Fact]
    public async Task Forward_HttpResponse_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage1 = context.Forward(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(new Uri($"http://localhost:{port}/HttpRemote/Request1"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Get,
                $"http://localhost:{port}/HttpRemote/Request1");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 = context.Forward($"http://localhost:{port}/HttpRemote/Request1");

            var str1 = await httpResponseMessage1.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Hello World Hello World Hello World Hello World", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task Forward_HttpResponse_NonMultipartFormData_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, HttpRemoteAspNetCoreModel1 model) =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage1 = context.Forward(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(
                new Uri($"http://localhost:{port}/HttpRemote/Request2"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request2");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 =
                context.Forward($"http://localhost:{port}/HttpRemote/Request2");

            var model1 = await httpResponseMessage1.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model2 = await httpResponseMessage2.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model3 = await httpResponseMessage3.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();
            var model4 = await httpResponseMessage4.Content.ReadFromJsonAsync<HttpRemoteAspNetCoreModel1>();

            await context.Response.WriteAsync(model1!.Id + " " + model1.Name + " " +
                                              model2!.Id + " " + model2.Name +
                                              " " + model3!.Id + " " + model3.Name + " " +
                                              model4!.Id + " " +
                                              model4.Name + "");
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://localhost:{port}/test"));
        httpRequestMessage.Content =
            new StringContent(JsonSerializer.Serialize(new HttpRemoteAspNetCoreModel1 { Id = 1, Name = "furion" }),
                Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1 furion 1 furion 1 furion 1 furion", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task Forward_HttpResponse_MultipartFormData_ReturnOK()
    {
        var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async (HttpContext context, [FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage1 = context.Forward(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage2 = context.Forward(
                new Uri($"http://localhost:{port}/HttpRemote/Request6"));
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage3 = context.Forward(HttpMethod.Post,
                $"http://localhost:{port}/HttpRemote/Request6");
            // ReSharper disable once MethodHasAsyncOverload
            var httpResponseMessage4 =
                context.Forward($"http://localhost:{port}/HttpRemote/Request6");

            var str1 = await httpResponseMessage1.Content.ReadAsStringAsync();
            var str2 = await httpResponseMessage2.Content.ReadAsStringAsync();
            var str3 = await httpResponseMessage3.Content.ReadAsStringAsync();
            var str4 = await httpResponseMessage4.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1 + " " + str2 + " " + str3 + " " + str4);
        }).DisableAntiforgery();

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post,
            new Uri($"http://localhost:{port}/test"));

        var multipartFormDataContent = new MultipartFormDataContent(boundary);
        multipartFormDataContent.Add(new StringContent("1"), "Id");
        multipartFormDataContent.Add(new StringContent("Furion"), "Name");
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var bytes = await File.ReadAllBytesAsync(fileFullName);
        multipartFormDataContent.Add(new ByteArrayContent(bytes), "File", "test.txt");

        httpRequestMessage.Content = multipartFormDataContent;

        var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage);
        httpResponseMessage.EnsureSuccessStatusCode();

        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt 1;Furion;test.txt", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_Redirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async context =>
        {
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request8"));

            var str1 = await httpResponseMessage1.Content.ReadAsStringAsync();

            await context.Response.WriteAsync(str1);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("Redirect", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_HttpResponse_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async context =>
        {
            var httpResponseMessage1 = await context.ForwardAsync(HttpMethod.Post,
                new Uri($"http://localhost:{port}/HttpRemote/Request9"));

            var stream = await httpResponseMessage1.Content.ReadAsStreamAsync();

            await stream.CopyToAsync(context.Response.Body);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
        Assert.True(stream.CanRead);

        await app.StopAsync();
    }

    [Fact]
    public void ForwardResponseMessageToContext_ReturnOK()
    {
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        httpResponseMessage.Headers.TryAddWithoutValidation("Framework", "furion");
        httpResponseMessage.Headers.TryAddWithoutValidation("Transfer-Encoding", "chunked");
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.txt" };

        var defaultHttpContext = new DefaultHttpContext();
        HttpContextExtensions.ForwardResponseMessageToContext(defaultHttpContext, httpResponseMessage, null);
        Assert.Equal(500, defaultHttpContext.Response.StatusCode);
        Assert.Equal("furion", defaultHttpContext.Response.Headers["Framework"]);
        Assert.DoesNotContain(defaultHttpContext.Response.Headers, h => h.Key == "Transfer-Encoding");
        Assert.Equal("attachment; filename=test.txt", defaultHttpContext.Response.Headers.ContentDisposition);

        var defaultHttpContext2 = new DefaultHttpContext();
        HttpContextExtensions.ForwardResponseMessageToContext(defaultHttpContext2, httpResponseMessage,
            new HttpContextForwardOptions { ForwardStatusCode = false, ForwardResponseHeaders = false });
        Assert.Equal(200, defaultHttpContext2.Response.StatusCode);
        Assert.DoesNotContain(defaultHttpContext2.Response.Headers, u => u.Key == "Framework");

        var defaultHttpContext3 = new DefaultHttpContext();
        HttpContextExtensions.ForwardResponseMessageToContext(defaultHttpContext3, httpResponseMessage,
            new HttpContextForwardOptions
            {
                ForwardStatusCode = false,
                ForwardResponseHeaders = false,
                OnForwarding =
                    (ctx, res) =>
                    {
                        ctx.Response.ContentLength = 10;
                    }
            });
        Assert.Equal(200, defaultHttpContext3.Response.StatusCode);
        Assert.DoesNotContain(defaultHttpContext3.Response.Headers, u => u.Key == "Framework");
        Assert.Equal(10, defaultHttpContext3.Response.Headers.ContentLength);
    }

    [Fact]
    public void IgnoreResponseHeaders_ReturnOK() =>
        Assert.Equal(["Transfer-Encoding"], HttpContextExtensions._ignoreResponseHeaders);

    [Fact]
    public async Task ForwardAsAsync_WithQueryParameters_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async (HttpContext context, [FromQuery] int number) =>
        {
            var str = await context.ForwardAsAsync<int>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request10"));

            await context.Response.WriteAsync(str.ToString());
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test?number=10")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("11", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAs_WithQueryParameters_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapGet("/test", async (HttpContext context, [FromQuery] int number) =>
        {
            var str = context.ForwardAs<int>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request10"));

            await context.Response.WriteAsync(str.ToString());
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,
                new Uri($"http://localhost:{port}/test?number=10")));
        httpResponseMessage.EnsureSuccessStatusCode();
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Equal("11", str);

        await app.StopAsync();
    }

    [Fact]
    public async Task ForwardAsync_WithException_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();
        app.Use(async (ctx, next) =>
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;

            await next.Invoke();
        });

        app.MapControllers();

        app.MapPost("/test", async context =>
        {
            var actionResult = await context.ForwardAsync<IActionResult>(HttpMethod.Get,
                new Uri($"http://localhost:{port}/HttpRemote/Request11"), hbuilder =>
                {
                    hbuilder.AddHttpContentConverters(() => [new IActionResultContentConverter()]);
                });

            var contentResult = actionResult.Result as ContentResult;

            await context.Response.WriteAsync(contentResult?.Content ?? string.Empty);
        });

        await app.StartAsync();

        var httpClient = app.Services.GetRequiredService<IHttpClientFactory>().CreateClient();
        var httpResponseMessage =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                new Uri($"http://localhost:{port}/test")));
        // httpResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.InternalServerError, httpResponseMessage.StatusCode);
        var str = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.Contains("出错了", str);

        await app.StopAsync();
    }
}