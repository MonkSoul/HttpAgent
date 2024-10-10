// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteCasesTests
{
    [Fact]
    public async Task SendJson_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendJson")
                    .SetJsonContent(new HttpRemoteModel { Id = 1, Name = "Furion" }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent(new HttpRemoteModel { Id = 1, Name = "Furion" }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent(new HttpRemoteModel { Id = 1, Name = "Furion" }, useStringContent: true));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFormUrlEncoded3_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<HttpRemoteModel>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFormUrlEncoded")
                    .SetFormUrlEncodedContent("id=1&name=Furion", useStringContent: true));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal(1, httpRemoteResult.Result.Id);
        Assert.Equal("Furion", httpRemoteResult.Result.Name);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileStream(fileFullName, "file");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFile2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFile")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileBytes(fileFullName, "file");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFiles_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileFullName2 = Path.Combine(AppContext.BaseDirectory, "test2.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFiles")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileStream(fileFullName, "files");
                        mBuilder.AddFileStream(fileFullName2, "files");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("test.txt;test2.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendFiles2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileFullName2 = Path.Combine(AppContext.BaseDirectory, "test2.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendFiles")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddFileBytes(fileFullName, "files");
                        mBuilder.AddFileBytes(fileFullName2, "files");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("test.txt;test2.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddJson(new { id = 1, name = "furion" });
                        mBuilder.AddFileStream(fileFullName, "file");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart2_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddJsonProperty(1, "id");
                        mBuilder.AddJsonProperty("furion", "name");
                        mBuilder.AddFileStream(fileFullName, "file");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task SendMultipart3_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/SendMultipart")
                    .SetMultipartContent(mBuilder =>
                    {
                        mBuilder.AddJson("{\"id\":1,\"name\":\"furion\"}");
                        mBuilder.AddFileStream(fileFullName, "file");
                    }));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("1;furion;test.txt", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task Send_IncludeRedirect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(
                HttpRequestBuilder.Get($"http://localhost:{port}/HttpRemote/RedirectTo"));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Equal("Redirect", httpRemoteResult.Result);

        await app.StopAsync();
    }

    [Fact]
    public async Task Send_GetFile_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);

        builder.Services.AddControllers()
            .AddApplicationPart(typeof(HttpRemoteController).Assembly);
        builder.Services.AddHttpRemote();

        await using var app = builder.Build();

        app.MapControllers();

        await app.StartAsync();

        var httpRemoteService = app.Services.GetRequiredService<IHttpRemoteService>();

        var httpRemoteResult =
            await httpRemoteService.SendAsync<Stream>(
                HttpRequestBuilder.Post($"http://localhost:{port}/HttpRemote/GetFile"));

        Assert.Equal(HttpStatusCode.OK, httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.True(httpRemoteResult.Result.CanRead);

        await app.StopAsync();
    }
}