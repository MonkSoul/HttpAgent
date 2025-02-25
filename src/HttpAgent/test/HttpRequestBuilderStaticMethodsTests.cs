// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderStaticMethodsTests
{
    [Fact]
    public void Get_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Get((string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Get((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Get(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Put_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Put((string)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Put("http://localhost");
        Assert.Equal(HttpMethod.Put, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Put((Uri)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Put(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Put, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Post_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Post((string)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Post("http://localhost");
        Assert.Equal(HttpMethod.Post, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Post((Uri)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Post(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Post, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Delete_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Delete((string)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Delete("http://localhost");
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Delete((Uri)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Delete(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Head_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Head((string)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Head("http://localhost");
        Assert.Equal(HttpMethod.Head, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Head((Uri)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Head(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Head, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Options_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Options((string)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Options("http://localhost");
        Assert.Equal(HttpMethod.Options, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Options((Uri)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Options(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Options, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Trace_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Trace((string)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Trace("http://localhost");
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Trace((Uri)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Trace(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Patch_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Patch((string)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Patch("http://localhost");
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Patch((Uri)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Patch(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder4.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Create_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((HttpMethod)null!, (Uri)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((string)null!, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(string.Empty, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(" ", (string)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((string)null!, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(string.Empty, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(" ", null!, null);
        });
    }

    [Fact]
    public void Create_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Create(HttpMethod.Get, (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.Method);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Create(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
        Assert.NotNull(httpRequestBuilder2.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Create(HttpMethod.Get, (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.Method);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Create(HttpMethod.Get, "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.Method);
        Assert.NotNull(httpRequestBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri.ToString());

        var httpRequestBuilder5 = HttpRequestBuilder.Create("GET", (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder5.Method);
        Assert.Null(httpRequestBuilder5.RequestUri);

        var httpRequestBuilder6 = HttpRequestBuilder.Create("GET", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder6.Method);
        Assert.NotNull(httpRequestBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder6.RequestUri.ToString());

        var httpRequestBuilder7 = HttpRequestBuilder.Create("get", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder7.Method);
        Assert.NotNull(httpRequestBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder7.RequestUri.ToString());

        var httpRequestBuilder8 = HttpRequestBuilder.Create("GET", (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder8.Method);
        Assert.Null(httpRequestBuilder8.RequestUri);

        var httpRequestBuilder9 = HttpRequestBuilder.Create("GET", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder9.Method);
        Assert.NotNull(httpRequestBuilder9.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder9.RequestUri.ToString());

        var httpRequestBuilder10 = HttpRequestBuilder.Create("get", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder10.Method);
        Assert.NotNull(httpRequestBuilder10.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder10.RequestUri.ToString());

        var httpRequestBuilder11 = HttpRequestBuilder.Create("Furion", (string)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder11.Method);

        var httpRequestBuilder12 = HttpRequestBuilder.Create("Furion", (Uri)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder12.Method);

        var httpRequestBuilder13 = HttpRequestBuilder.Create(HttpMethod.Post, (string)null!, _ =>
        {
        });
        Assert.Equal(HttpMethod.Post, httpRequestBuilder13.Method);

        var httpRequestBuilder14 = HttpRequestBuilder.Create("Furion", null!, null);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder14.Method);

        var httpRequestBuilder15 = HttpRequestBuilder.Create(HttpMethod.Post, (Uri)null!, null);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder15.Method);
    }

    [Fact]
    public void DownloadFile_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.DownloadFile(null!, null, null, null));

    [Fact]
    public void DownloadFile_ReturnOK()
    {
        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(HttpMethod.Post, new Uri("http://localhost"), null);

        Assert.NotNull(httpFileDownloadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder.Method);
        Assert.NotNull(httpFileDownloadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder.RequestUri.ToString());

        var httpFileDownloadBuilder2 = HttpRequestBuilder.DownloadFile(HttpMethod.Post, null, null);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder2.Method);
        Assert.Null(httpFileDownloadBuilder2.RequestUri);

        var httpFileDownloadBuilder3 = HttpRequestBuilder.DownloadFile((string)null!, null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder3.Method);
        Assert.Null(httpFileDownloadBuilder3.RequestUri);

        var httpFileDownloadBuilder4 = HttpRequestBuilder.DownloadFile("http://localhost", null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder4.Method);
        Assert.NotNull(httpFileDownloadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder4.RequestUri.ToString());

        var httpFileDownloadBuilder5 = HttpRequestBuilder.DownloadFile((Uri)null!, null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder5.Method);
        Assert.Null(httpFileDownloadBuilder5.RequestUri);

        var httpFileDownloadBuilder6 = HttpRequestBuilder.DownloadFile(new Uri("http://localhost"), null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder6.Method);
        Assert.NotNull(httpFileDownloadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void UploadFile_ReturnOK()
    {
        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(HttpMethod.Post, new Uri("http://localhost"), @"C:\Workspaces\furion.html");

        Assert.NotNull(httpFileUploadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder.Method);
        Assert.NotNull(httpFileUploadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder.FilePath);
        Assert.Equal("file", httpFileUploadBuilder.Name);

        var httpFileUploadBuilder2 = HttpRequestBuilder.UploadFile(HttpMethod.Post, null, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder2.Method);
        Assert.Null(httpFileUploadBuilder2.RequestUri);

        var httpFileUploadBuilder3 = HttpRequestBuilder.UploadFile((string)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder3.Method);
        Assert.Null(httpFileUploadBuilder3.RequestUri);

        var httpFileUploadBuilder4 = HttpRequestBuilder.UploadFile("http://localhost", @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder4.Method);
        Assert.NotNull(httpFileUploadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder4.RequestUri.ToString());

        var httpFileUploadBuilder5 = HttpRequestBuilder.UploadFile((Uri)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder5.Method);
        Assert.Null(httpFileUploadBuilder5.RequestUri);

        var httpFileUploadBuilder6 =
            HttpRequestBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder6.Method);
        Assert.NotNull(httpFileUploadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder6.RequestUri.ToString());

        var httpFileUploadBuilder7 =
            HttpRequestBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html", "fileinfo");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder7.Method);
        Assert.NotNull(httpFileUploadBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder7.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder7.FilePath);
        Assert.Equal("fileinfo", httpFileUploadBuilder7.Name);
    }

    [Fact]
    public void ServerSentEvents_ReturnOK()
    {
        var httpServerSentEventsBuilder =
            HttpRequestBuilder.ServerSentEvents(new Uri("http://localhost"), _ => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder);
        Assert.NotNull(httpServerSentEventsBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder.RequestUri.ToString());

        var httpServerSentEventsBuilder2 = HttpRequestBuilder.ServerSentEvents((Uri)null!, _ => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder2.RequestUri);

        var httpServerSentEventsBuilder3 =
            HttpRequestBuilder.ServerSentEvents("http://localhost", _ => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder3);
        Assert.NotNull(httpServerSentEventsBuilder3.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder3.RequestUri.ToString());

        var httpServerSentEventsBuilder4 = HttpRequestBuilder.ServerSentEvents((string)null!, _ => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder4.RequestUri);

        var httpServerSentEventsBuilder5 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, null!, _ => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder5.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder5.Method);
    }

    [Fact]
    public void StressTestHarness_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.StressTestHarness(null!, null));

    [Fact]
    public void StressTestHarness_ReturnOK()
    {
        var httpStressTestHarnessBuilder =
            HttpRequestBuilder.StressTestHarness(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpStressTestHarnessBuilder);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder.Method);
        Assert.NotNull(httpStressTestHarnessBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder.RequestUri.ToString());

        var httpStressTestHarnessBuilder2 = HttpRequestBuilder.StressTestHarness(HttpMethod.Post, null, 500);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder2.Method);
        Assert.Null(httpStressTestHarnessBuilder2.RequestUri);
        Assert.Equal(500, httpStressTestHarnessBuilder2.NumberOfRequests);

        var httpStressTestHarnessBuilder3 = HttpRequestBuilder.StressTestHarness((string)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder3.Method);
        Assert.Null(httpStressTestHarnessBuilder3.RequestUri);

        var httpStressTestHarnessBuilder4 = HttpRequestBuilder.StressTestHarness("http://localhost");
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder4.Method);
        Assert.NotNull(httpStressTestHarnessBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder4.RequestUri.ToString());

        var httpStressTestHarnessBuilder5 = HttpRequestBuilder.StressTestHarness((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder5.Method);
        Assert.Null(httpStressTestHarnessBuilder5.RequestUri);

        var httpStressTestHarnessBuilder6 = HttpRequestBuilder.StressTestHarness(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder6.Method);
        Assert.NotNull(httpStressTestHarnessBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void LongPolling_ReturnOK()
    {
        var httpLongPollingBuilder =
            HttpRequestBuilder.LongPolling(HttpMethod.Get, new Uri("http://localhost"), _ => Task.CompletedTask);

        Assert.NotNull(httpLongPollingBuilder);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder.Method);
        Assert.NotNull(httpLongPollingBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder.RequestUri.ToString());

        var httpLongPollingBuilder2 = HttpRequestBuilder.LongPolling(HttpMethod.Get, null, _ => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder2.Method);
        Assert.Null(httpLongPollingBuilder2.RequestUri);

        var httpLongPollingBuilder3 = HttpRequestBuilder.LongPolling((string)null!, _ => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder3.Method);
        Assert.Null(httpLongPollingBuilder3.RequestUri);

        var httpLongPollingBuilder4 = HttpRequestBuilder.LongPolling("http://localhost", _ => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder4.Method);
        Assert.NotNull(httpLongPollingBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder4.RequestUri.ToString());

        var httpLongPollingBuilder5 = HttpRequestBuilder.LongPolling((Uri)null!, _ => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder5.Method);
        Assert.Null(httpLongPollingBuilder5.RequestUri);

        var httpLongPollingBuilder6 =
            HttpRequestBuilder.LongPolling(new Uri("http://localhost"), _ => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder6.Method);
        Assert.NotNull(httpLongPollingBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void Declarative_ReturnOK()
    {
        var method = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder = HttpRequestBuilder.Declarative(method, []);

        Assert.NotNull(httpDeclarativeBuilder);
        Assert.Equal(method, httpDeclarativeBuilder.Method);
        Assert.Equal([], httpDeclarativeBuilder.Args);
    }
}