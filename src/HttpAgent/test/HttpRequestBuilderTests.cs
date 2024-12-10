﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
        {
            _ = new HttpRequestBuilder(null!, null!);
        });

    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(HttpRequestBuilder._stringContentForFormUrlEncodedContentProcessorInstance);

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
        Assert.Null(httpRequestBuilder.RequestUri);
        Assert.False(httpRequestBuilder._isAddedStringContentForFormUrlEncodedContentProcessor);
        Assert.NotNull(httpRequestBuilder._lock);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());
    }

    [Fact]
    public void BuildFinalRequestUri_Invalid_Parameters()
    {
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute));

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = httpRequestBuilder.BuildFinalRequestUri(null);
        });
    }

    [Fact]
    public void BuildFinalRequestUri_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/{id}/{name}"));
        httpRequestBuilder.SetFragment("furion").WithQueryParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { id = 10, name = "furion" });

        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        Assert.Equal("http://localhost/10/furion?id=10&name=furion#furion", finalRequestUri);

        var httpRequestBuilder2 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("{id}/{name}", UriKind.RelativeOrAbsolute));
        httpRequestBuilder2.SetFragment("furion").WithQueryParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { id = 10, name = "furion" });

        var finalRequestUri2 = httpRequestBuilder2.BuildFinalRequestUri(new Uri("http://localhost"));
        Assert.Equal("http://localhost/10/furion?id=10&name=furion#furion", finalRequestUri2);

        var httpRequestBuilder3 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("https://furion.net/{id}/{name}", UriKind.RelativeOrAbsolute));
        httpRequestBuilder3.SetFragment("furion").WithQueryParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { id = 10, name = "furion" });

        var finalRequestUri3 = httpRequestBuilder3.BuildFinalRequestUri(new Uri("http://localhost"));
        Assert.Equal("https://furion.net/10/furion?id=10&name=furion#furion", finalRequestUri3);

        var httpRequestBuilder4 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("{url}", UriKind.RelativeOrAbsolute));
        httpRequestBuilder4.WithPathParameters(new { url = "http://localhost/id=10" });

        var finalRequestUri4 = httpRequestBuilder4.BuildFinalRequestUri(new Uri("http://localhost"));
        Assert.Equal("http://localhost/id=10", finalRequestUri4);

        var httpRequestBuilder5 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder5.SetFragment("#furion");

        var finalRequestUri5 = httpRequestBuilder5.BuildFinalRequestUri(null);
        Assert.Equal("http://localhost/#furion", finalRequestUri5);

        // With BassAddress
        var httpRequestBuilder6 =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("/api/test", UriKind.RelativeOrAbsolute));
        httpRequestBuilder6.SetBaseAddress("http://localhost");

        var finalRequestUri6 = httpRequestBuilder6.BuildFinalRequestUri(null);
        Assert.Equal("http://localhost/api/test", finalRequestUri6);

        var finalRequestUri7 = httpRequestBuilder6.BuildFinalRequestUri(new Uri("https://furion.net"));
        Assert.Equal("http://localhost/api/test", finalRequestUri7);
    }

    [Fact]
    public void AppendFragment_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var uriBuilder = new UriBuilder(httpRequestBuilder.RequestUri!);

        httpRequestBuilder.AppendFragment(uriBuilder);
        Assert.Empty(uriBuilder.Fragment);

        httpRequestBuilder.SetFragment("fragment");
        httpRequestBuilder.AppendFragment(uriBuilder);
        Assert.Equal("#fragment", uriBuilder.Fragment);
    }

    [Fact]
    public void AppendQueryParameters_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost?v=1"));
        var uriBuilder = new UriBuilder(httpRequestBuilder.RequestUri!);

        httpRequestBuilder.AppendQueryParameters(uriBuilder);
        Assert.Equal("?v=1", uriBuilder.Query);

        httpRequestBuilder.WithQueryParameters(new { id = 10, name = "furion" })
            .WithQueryParameters(new { name = "monksoul" });

        httpRequestBuilder.AppendQueryParameters(uriBuilder);
        Assert.Equal("?v=1&id=10&name=furion&name=monksoul", uriBuilder.Query);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var uriBuilder2 = new UriBuilder(httpRequestBuilder2.RequestUri!);

        httpRequestBuilder2.AppendQueryParameters(uriBuilder2);
        Assert.Empty(uriBuilder2.Query);

        httpRequestBuilder2.WithQueryParameters(new { id = 10, name = "furion" })
            .WithQueryParameters(new { name = "monksoul" });

        httpRequestBuilder2.AppendQueryParameters(uriBuilder2);
        Assert.Equal("?id=10&name=furion&name=monksoul", uriBuilder2.Query);

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost?v=1&name=10&c=20"))
            .WithQueryParameters(new { id = 10 }).RemoveQueryParameters("C");
        var uriBuilder3 = new UriBuilder(httpRequestBuilder3.RequestUri!);

        httpRequestBuilder3.AppendQueryParameters(uriBuilder3);
        Assert.Equal("?v=1&name=10&id=10", uriBuilder3.Query);
    }

    [Fact]
    public void ReplacePathPlaceholders_ReturnOK()
    {
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/{id}/{name}?v=1&id={id}"));
        var uriBuilder = new UriBuilder(httpRequestBuilder.RequestUri!);

        var newUri = httpRequestBuilder.ReplacePathPlaceholders(uriBuilder.Uri.ToString());
        Assert.Equal("http://localhost/{id}/{name}?v=1&id={id}", newUri);

        httpRequestBuilder.WithPathParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { name = "monksoul" });
        var newUri2 = httpRequestBuilder.ReplacePathPlaceholders(uriBuilder.Uri.ToString());
        Assert.Equal("http://localhost/10/monksoul?v=1&id=10", newUri2);

        // Object
        var httpRequestBuilder2 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("http://localhost/{user.id}/{author.name}/{unknown.test}?v=1&id={id}"));
        var uriBuilder2 = new UriBuilder(httpRequestBuilder2.RequestUri!);
        httpRequestBuilder2.WithPathParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { name = "monksoul" })
            .WithPathParameters(new { id = 10 }, "user")
            .WithPathParameters(new { name = "furion" }, "author");
        var newUri3 = httpRequestBuilder2.ReplacePathPlaceholders(uriBuilder2.Uri.ToString());
        Assert.Equal("http://localhost/10/furion/{unknown.test}?v=1&id=10", newUri3);

        var httpRequestBuilder3 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("http://localhost/{user.id}"));
        httpRequestBuilder3.WithPathParameters(null!, "user");
        var uriBuilder3 = new UriBuilder(httpRequestBuilder3.RequestUri!);
        var newUri4 = httpRequestBuilder3.ReplacePathPlaceholders(uriBuilder3.Uri.ToString());
        Assert.Equal("http://localhost/", newUri4);

        var httpRequestBuilder4 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("http://localhost/?test={test}"));
        var uriBuilder4 = new UriBuilder(httpRequestBuilder4.RequestUri!);
        httpRequestBuilder4.WithPathParameter("test", new[] { "furion", "monksoul" });
        var newUri5 = httpRequestBuilder4.ReplacePathPlaceholders(uriBuilder4.Uri.ToString());
        Assert.Equal("http://localhost/?test=furion,monksoul", newUri5);

        var httpRequestBuilder5 =
            new HttpRequestBuilder(HttpMethod.Get,
                new Uri("http://localhost/?id={user.id}&name={user.name}"));
        var uriBuilder5 = new UriBuilder(httpRequestBuilder5.RequestUri!);
        httpRequestBuilder5.WithPathParameters(new { id = 1, name = "Furion" }, "user");
        var newUri6 = httpRequestBuilder5.ReplacePathPlaceholders(uriBuilder5.Uri.ToString());
        Assert.Equal("http://localhost/?id=1&name=Furion", newUri6);
    }

    [Fact]
    public void AppendHeaders_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Headers);

        httpRequestBuilder.WithHeaders(new { id = 10, name = "furion" }).WithHeaders(new { name = "monksoul" });
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.Equal(2, httpRequestMessage.Headers.Count());
        Assert.Equal("10", httpRequestMessage.Headers.GetValues("id").First());
        Assert.Equal(["furion", "monksoul"], httpRequestMessage.Headers.GetValues("name"));

        httpRequestBuilder.SetTraceIdentifier("furion");
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.Equal(3, httpRequestMessage.Headers.Count());
        Assert.Equal("furion", httpRequestMessage.Headers.GetValues("X-Trace-ID").First());

        httpRequestBuilder.AddBasicAuthentication("furion", "q1w2e3");
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        var base64Credentials = Convert.ToBase64String("furion:q1w2e3"u8.ToArray());

        Assert.NotNull(httpRequestMessage.Headers.Authorization);
        Assert.Equal("Basic " + base64Credentials, httpRequestMessage.Headers.Authorization.ToString());
        Assert.Equal("Basic", httpRequestMessage.Headers.Authorization.Scheme);

        httpRequestBuilder.DisableCache();
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.NotNull(httpRequestMessage.Headers.CacheControl);
        Assert.True(httpRequestMessage.Headers.CacheControl.NoCache);
        Assert.True(httpRequestMessage.Headers.CacheControl.NoStore);
        Assert.True(httpRequestMessage.Headers.CacheControl.MustRevalidate);

        httpRequestBuilder.WithHeader("null", null);
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.Equal([""], httpRequestMessage.Headers.GetValues("null"));

        httpRequestBuilder.AutoSetHostHeader();
        httpRequestBuilder.AppendHeaders(httpRequestMessage);
        Assert.Equal("localhost:80", httpRequestMessage.Headers.Host);
    }

    [Fact]
    public void AppendAuthentication_ReturnOK()
    {
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/")).AddBasicAuthentication(
                "admin", "a123456789");
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.AppendAuthentication(httpRequestMessage);
        Assert.Equal("Basic YWRtaW46YTEyMzQ1Njc4OQ==",
            httpRequestMessage.Headers.GetValues("Authorization").FirstOrDefault());
    }

    [Fact]
    public async Task AppendAuthentication_WithDigest_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.WWWAuthenticate =
                "Digest qop=\"auth\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", stale=\"FALSE\"";

            await context.Response.CompleteAsync();
        });

        await app.StartAsync();

        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).AddDigestAuthentication(
                "admin", "a123456789");
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.AppendAuthentication(httpRequestMessage);

        Assert.Contains(
            "Digest username=\"admin\", realm=\"IP Camera(K7151)\", nonce=\"613134303a38303236313662363ae1b0b8bde54893eab8c0846d38665ab9\", uri=\"/test\", algorithm=MD5, qop=auth, nc=00000001",
            httpRequestMessage.Headers.GetValues("Authorization").FirstOrDefault());

        await app.StopAsync();
    }

    [Fact]
    public void AppendCookies_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.AppendCookies(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Headers);

        httpRequestBuilder.WithCookies(new { id = 10, name = "furion" });
        httpRequestBuilder.AppendCookies(httpRequestMessage);

        Assert.Single(httpRequestMessage.Headers);
        Assert.Equal("id=10; name=furion", httpRequestMessage.Headers.GetValues("Cookie").First());
    }

    [Fact]
    public void RemoveCookies_ReturnOK()
    {
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"))
                .WithCookies(new { id = 10, name = "furion", age = 30, address = "广东省" }).RemoveCookies("age", "id");
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);
        httpRequestBuilder.AppendCookies(httpRequestMessage);
        httpRequestBuilder.RemoveCookies(httpRequestMessage);

        Assert.Single(httpRequestMessage.Headers);
        Assert.Equal("name=furion; address=%E5%B9%BF%E4%B8%9C%E7%9C%81",
            httpRequestMessage.Headers.GetValues("Cookie").First());
    }

    [Fact]
    public void RemoveHeaders_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.RemoveHeaders(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Headers);

        httpRequestBuilder.WithHeaders(new { id = 10, name = "furion" }).RemoveHeaders("name").RemoveHeaders("unknown")
            .AppendHeaders(httpRequestMessage);

        httpRequestBuilder.RemoveHeaders(httpRequestMessage);
        Assert.Single(httpRequestMessage.Headers);
        Assert.Equal("10", httpRequestMessage.Headers.GetValues("id").First());

        httpRequestBuilder.RemoveHeaders("ID");

        httpRequestBuilder.RemoveHeaders(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Headers);
    }

    [Fact]
    public void BuildAndSetContent_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteOptions = new HttpRemoteOptions();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);
        var httpContentProcessorFactory = new HttpContentProcessorFactory(serviceProvider, []);

        httpRequestBuilder.BuildAndSetContent(httpRequestMessage, httpContentProcessorFactory,
            httpRemoteOptions);
        Assert.Null(httpRequestBuilder.ContentType);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Null(httpRequestMessage.Content);

        httpRequestBuilder.SetContent(new { id = 10, name = "Furion" });
        httpRequestBuilder.BuildAndSetContent(httpRequestMessage, httpContentProcessorFactory,
            httpRemoteOptions);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.NotNull(httpRequestMessage.Content);
        Assert.Equal(typeof(StringContent), httpRequestMessage.Content.GetType());

        httpRequestBuilder.SetContent(new { id = 10, name = "Furion" }).SetOnPreSetContent(content =>
        {
            content?.Headers.TryAddWithoutValidation("test", "furion");
        });
        httpRequestBuilder.BuildAndSetContent(httpRequestMessage, httpContentProcessorFactory,
            httpRemoteOptions);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.NotNull(httpRequestMessage.Content);
        Assert.Equal(typeof(StringContent), httpRequestMessage.Content.GetType());
        Assert.Equal("furion", httpRequestMessage.Content.Headers.GetValues("test").First());

        httpRequestBuilder.SetContent(new { id = 10, name = "Furion" }).SetMultipartContent(builder =>
        {
            builder.AddObject(new { id = 10, name = "Furion" });
        });
        httpRequestBuilder.BuildAndSetContent(httpRequestMessage, httpContentProcessorFactory,
            httpRemoteOptions);
        Assert.Equal("multipart/form-data", httpRequestBuilder.ContentType);
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.NotNull(httpRequestMessage.Content);
        Assert.Equal(typeof(MultipartFormDataContent), httpRequestMessage.Content.GetType());
    }

    [Fact]
    public void AppendProperties_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, new Uri("http://localhost"));

        httpRequestBuilder.AppendProperties(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Options);

        httpRequestBuilder.WithProperties(new { id = 10, name = "furion" });
        httpRequestBuilder.AppendProperties(httpRequestMessage);
        Assert.Equal(2, httpRequestMessage.Options.Count());

        httpRequestBuilder.Profiler(false);
        httpRequestBuilder.AppendProperties(httpRequestMessage);
        Assert.Equal(3, httpRequestMessage.Options.Count());
        Assert.True(httpRequestMessage.Options.TryGetValue(
            new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY), out var value));
        Assert.Equal("TRUE", value);
    }

    [Fact]
    public void SetDefaultContentType_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetContentType(MediaTypeNames.Text.Plain);
        httpRequestBuilder.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Text.Plain, httpRequestBuilder.ContentType);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.SetContent(new { });
        httpRequestBuilder2.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Text.Plain, httpRequestBuilder2.ContentType);

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder3.SetContent(Array.Empty<byte>());
        httpRequestBuilder3.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder3.ContentType);

        using var stream = new MemoryStream();
        var httpRequestBuilder4 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder4.SetContent(stream);
        httpRequestBuilder4.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder4.ContentType);

        var httpRequestBuilder5 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder5.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Text.Plain, httpRequestBuilder5.ContentType);

        var httpRequestBuilder6 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder6.SetContent(new ByteArrayContent([]));
        httpRequestBuilder6.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder6.ContentType);

        var httpRequestBuilder7 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder7.SetContent(new StreamContent(stream));
        httpRequestBuilder7.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder7.ContentType);

        var httpRequestBuilder8 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder8.SetContent(new FormUrlEncodedContent(Array.Empty<KeyValuePair<string, string>>()));
        httpRequestBuilder8.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Application.FormUrlEncoded, httpRequestBuilder8.ContentType);

        var httpRequestBuilder9 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder9.SetContent(new MultipartContent());
        httpRequestBuilder9.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Multipart.FormData, httpRequestBuilder9.ContentType);

        var httpRequestBuilder10 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder10.SetContent(new StringContent(""));
        httpRequestBuilder10.SetDefaultContentType(MediaTypeNames.Text.Plain);
        Assert.Equal(MediaTypeNames.Text.Plain, httpRequestBuilder10.ContentType);

        var httpRequestBuilder11 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder11.SetContent(new StringContent(""));
        httpRequestBuilder11.SetDefaultContentType(MediaTypeNames.Application.Json);
        Assert.Equal(MediaTypeNames.Application.Json, httpRequestBuilder11.ContentType);

        var httpRequestBuilder12 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder12.SetContent(JsonContent.Create(new { }));
        httpRequestBuilder12.SetDefaultContentType(MediaTypeNames.Application.Json);
        Assert.Equal(MediaTypeNames.Application.Json, httpRequestBuilder12.ContentType);

        var httpRequestBuilder13 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder13.SetContent(new ReadOnlyMemoryContent(Array.Empty<byte>()));
        httpRequestBuilder13.SetDefaultContentType(MediaTypeNames.Application.Octet);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder13.ContentType);

        var httpRequestBuilder14 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder14.SetContent(new ReadOnlyMemory<byte>([]));
        httpRequestBuilder14.SetDefaultContentType(MediaTypeNames.Application.Octet);
        Assert.Equal(MediaTypeNames.Application.Octet, httpRequestBuilder14.ContentType);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.Build(null!, null!, null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.Build(new HttpRemoteOptions(), null!, null!);
        });
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteOptions = new HttpRemoteOptions();

        var httpRequestMessage = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/{id}/{name}"))
            .SetContent(new { })
            .WithQueryParameters(new { id = 10, name = "furion" })
            .WithPathParameters(new { id = 10, name = "furion" })
            .WithCookies(new { id = 10, name = "furion" })
            .WithHeaders(new { id = 10, name = "furion" })
            .RemoveHeaders("name")
            .Profiler(false)
            .Build(httpRemoteOptions, new HttpContentProcessorFactory(serviceProvider, []), null);

        Assert.NotNull(httpRequestMessage);
        Assert.NotNull(httpRequestMessage.RequestUri);
        Assert.Equal("http://localhost/10/furion?id=10&name=furion", httpRequestMessage.RequestUri.ToString());
        Assert.Equal(2, httpRequestMessage.Headers.Count());
        Assert.Equal("id=10; name=furion", httpRequestMessage.Headers.GetValues("Cookie").First());
        Assert.NotNull(httpRequestMessage.Content);
        Assert.Equal(typeof(StringContent), httpRequestMessage.Content.GetType());
        Assert.Equal("text/plain", httpRequestMessage.Content.Headers.ContentType!.MediaType);
        Assert.Null(httpRequestMessage.Content.Headers.ContentType!.CharSet);
        Assert.Single(httpRequestMessage.Options);
        Assert.True(httpRequestMessage.Options.TryGetValue(
            new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY), out var value));
        Assert.Equal("TRUE", value);
    }

    [Fact]
    public void Build_WithBaseUri_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteOptions = new HttpRemoteOptions();

        var httpRequestMessage =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("{id}/{name}", UriKind.RelativeOrAbsolute))
                .SetContent(new { })
                .WithQueryParameters(new { id = 10, name = "furion" })
                .WithPathParameters(new { id = 10, name = "furion" })
                .WithCookies(new { id = 10, name = "furion" })
                .Build(httpRemoteOptions, new HttpContentProcessorFactory(serviceProvider, []),
                    new Uri("http://localhost"));

        Assert.NotNull(httpRequestMessage);
        Assert.NotNull(httpRequestMessage.RequestUri);
        Assert.Equal("http://localhost/10/furion?id=10&name=furion", httpRequestMessage.RequestUri.ToString());
        Assert.Single(httpRequestMessage.Headers);
        Assert.Equal("id=10; name=furion", httpRequestMessage.Headers.GetValues("Cookie").First());
        Assert.NotNull(httpRequestMessage.Content);
        Assert.Equal(typeof(StringContent), httpRequestMessage.Content.GetType());
        Assert.Equal("text/plain", httpRequestMessage.Content.Headers.ContentType!.MediaType);
        Assert.Null(httpRequestMessage.Content.Headers.ContentType!.CharSet);
    }

    [Fact]
    public void EnablePerformanceOptimization_ReturnOK()
    {
        var httpRequestBuilder =
            new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost/"));
        var finalRequestUri = httpRequestBuilder.BuildFinalRequestUri(null);
        var httpRequestMessage = new HttpRequestMessage(httpRequestBuilder.Method!, finalRequestUri);

        httpRequestBuilder.EnablePerformanceOptimization(httpRequestMessage);
        Assert.Empty(httpRequestMessage.Headers);

        httpRequestBuilder.PerformanceOptimization();
        httpRequestBuilder.EnablePerformanceOptimization(httpRequestMessage);
        Assert.NotEmpty(httpRequestMessage.Headers);
        Assert.Equal("*/*", httpRequestMessage.Headers.Accept.ToString());
        Assert.Equal("gzip, deflate, br", httpRequestMessage.Headers.AcceptEncoding.ToString());
        Assert.False(httpRequestMessage.Headers.ConnectionClose);
    }
}