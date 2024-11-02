﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpDeclarativeBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeBuilder(null!, null!));

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeBuilder(method!, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var builder = new HttpDeclarativeBuilder(method!, []);

        Assert.NotNull(builder.Method);
        Assert.NotNull(builder.Args);
        Assert.False(builder._hasLoadedExtractors);

        Dictionary<Type, IHttpDeclarativeExtractor> extractors = new()
        {
            { typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor() },
            { typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor() },
            { typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor() },
            { typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor() },
            { typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor() },
            { typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor() },
            {
                typeof(EnsureSuccessStatusCodeDeclarativeExtractor),
                new EnsureSuccessStatusCodeDeclarativeExtractor()
            },
            { typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor() },
            { typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor() },
            { typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor() },
            { typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor() },
            { typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor() },
            { typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor() },
            { typeof(MultipartBodyDeclarativeExtractor), new MultipartBodyDeclarativeExtractor() },
            { typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor() }
        };

        Assert.Equal(extractors.Keys, HttpDeclarativeBuilder._extractors.Keys);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.InvalidMethod));
        var builder = new HttpDeclarativeBuilder(method!, []);

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build(new HttpRemoteOptions()));
        Assert.Equal("No `[HttpMethod]` annotation was found in method `InvalidMethod`.", exception.Message);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var builder = new HttpDeclarativeBuilder(method!, []);

        var httpRequestBuilder = builder.Build(new HttpRemoteOptions());
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
        Assert.Equal("https://furion.net/", httpRequestBuilder.RequestUri?.ToString());
        Assert.True(builder._hasLoadedExtractors);
    }

    [Fact]
    public void Build_WithExtractors_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var builder = new HttpDeclarativeBuilder(method!, []);

        var httpRequestBuilder = builder.Build(new HttpRemoteOptions
        {
            HttpDeclarativeExtractors = [() => [new CustomHttpDeclarativeExtractor()]]
        });
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
        Assert.Equal("https://furion.net/", httpRequestBuilder.RequestUri?.ToString());
        Assert.True(builder._hasLoadedExtractors);

        Dictionary<Type, IHttpDeclarativeExtractor> extractors = new()
        {
            { typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor() },
            { typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor() },
            { typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor() },
            { typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor() },
            { typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor() },
            { typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor() },
            {
                typeof(EnsureSuccessStatusCodeDeclarativeExtractor),
                new EnsureSuccessStatusCodeDeclarativeExtractor()
            },
            { typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor() },
            { typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor() },
            { typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor() },
            { typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor() },
            { typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor() },
            { typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor() },
            { typeof(MultipartBodyDeclarativeExtractor), new MultipartBodyDeclarativeExtractor() },
            { typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor() },
            { typeof(CustomHttpDeclarativeExtractor), new CustomHttpDeclarativeExtractor() }
        };

        Assert.Equal(extractors.Keys, HttpDeclarativeBuilder._extractors.Keys);
    }
}