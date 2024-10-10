// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpFileUploadBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpFileUploadBuilder(null!, null, null!, null!));
        Assert.Throws<ArgumentNullException>(() => new HttpFileUploadBuilder(HttpMethod.Post, null, null!, null!));
        Assert.Throws<ArgumentException>(
            () => new HttpFileUploadBuilder(HttpMethod.Post, null, string.Empty, null!));
        Assert.Throws<ArgumentException>(
            () => new HttpFileUploadBuilder(HttpMethod.Post, null, " ", null!));
        Assert.Throws<ArgumentNullException>(
            () => new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", null!));
        Assert.Throws<ArgumentException>(
            () => new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", string.Empty));
        Assert.Throws<ArgumentException>(
            () => new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", " "));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", "file");
        Assert.Equal(HttpMethod.Post, builder.Method);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Equal(HttpMethod.Post, builder2.Method);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\index.html", builder2.FileFullName);
        Assert.Null(builder2.ContentType);
        Assert.Equal(TimeSpan.FromSeconds(1), builder2.ProgressInterval);
        Assert.Null(builder2.OnTransferStarted);
        Assert.Null(builder2.OnTransferCompleted);
        Assert.Null(builder2.OnTransferFailed);
        Assert.Null(builder2.OnProgressChanged);
        Assert.Null(builder2.FileTransferEventHandlerType);
    }

    [Fact]
    public void SetContentType_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.SetContentType(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetContentType(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetContentType(" ");
        });

        Assert.Throws<FormatException>(() =>
        {
            builder.SetContentType("unknown");
        });
    }

    [Fact]
    public void SetContentType_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetContentType("text/plain");
        Assert.Equal("text/plain", builder.ContentType);

        builder.SetContentType("text/html;charset=utf-8");
        Assert.Equal("text/html", builder.ContentType);

        builder.SetContentType("text/html; charset=unicode");
        Assert.Equal("text/html", builder.ContentType);
    }

    [Fact]
    public void SetOnProgressChanged_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() => builder.SetOnProgressChanged(null!));
    }

    [Fact]
    public void SetOnProgressChanged_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Null(builder.OnProgressChanged);

        builder.SetOnProgressChanged(async _ =>
        {
            await Task.Delay(100);
        });

        Assert.NotNull(builder.OnProgressChanged);
    }

    [Fact]
    public void SetProgressInterval_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        var exception = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.Zero));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception2.Message);
    }

    [Fact]
    public void SetProgressInterval_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetProgressInterval(TimeSpan.FromSeconds(1));

        Assert.Equal(TimeSpan.FromSeconds(1), builder.ProgressInterval);
    }

    [Fact]
    public void SetOnTransferStarted_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferStarted(null!));
    }

    [Fact]
    public void SetOnTransferStarted_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferStarted(() => { });

        Assert.NotNull(builder.OnTransferStarted);
    }

    [Fact]
    public void SetOnTransferCompleted_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferCompleted(null!));
    }

    [Fact]
    public void SetOnTransferCompleted_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferCompleted(_ => { });

        Assert.NotNull(builder.OnTransferCompleted);
    }

    [Fact]
    public void SetOnTransferFailed_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferFailed(null!));
    }

    [Fact]
    public void SetOnTransferFailed_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferFailed(_ => { });

        Assert.NotNull(builder.OnTransferFailed);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetEventHandler(typeof(NotImplementFileDownloadEventHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementFileDownloadEventHandler)}` type is not assignable from `{typeof(IHttpFileTransferEventHandler)}`.",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetEventHandler(typeof(CustomFileTransferEventHandler));

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder.FileTransferEventHandlerType);

        var builder2 = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder2.SetEventHandler<CustomFileTransferEventHandler>();

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder2.FileTransferEventHandlerType);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        var httpRemoteOptions = new HttpRemoteOptions();

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!, null!));
        Assert.Throws<ArgumentNullException>(() => builder.Build(httpRemoteOptions, null!));
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            fileFullName, "file");

        var httpRemoteOptions = new HttpRemoteOptions();
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();

        var httpRequestBuilder = httpFileUploadBuilder.Build(httpRemoteOptions, progressChannel);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder.Method);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
        Assert.Single(httpRequestBuilder.MultipartFormDataBuilder._partContents);

        var item = httpRequestBuilder.MultipartFormDataBuilder._partContents[0];
        Assert.True(item.RawContent is ProgressFileStream);
        Assert.Equal("test.txt", item.FileName);
        Assert.Equal("application/octet-stream", item.ContentType);
        Assert.Equal("file", item.Name);

        var httpRequestBuilder2 = httpFileUploadBuilder.SetEventHandler<CustomFileTransferEventHandler2>().Build(
            httpRemoteOptions, progressChannel,
            builder =>
            {
                builder.SetTimeout(100);
            });

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }
}