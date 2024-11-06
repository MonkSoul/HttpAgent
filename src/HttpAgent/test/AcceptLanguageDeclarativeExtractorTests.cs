﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class AcceptLanguageDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(AcceptLanguageDeclarativeExtractor)));

        var extractor = new AcceptLanguageDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IAcceptLanguageDeclarativeExtractorTest1).GetMethod(
                nameof(IAcceptLanguageDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new AcceptLanguageDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.NotNull(httpRequestBuilder1.Headers);
        Assert.Single(httpRequestBuilder1.Headers);
        Assert.Equal("Accept-Language", httpRequestBuilder1.Headers.Keys.First());
        Assert.Null(httpRequestBuilder1.Headers["Accept-Language"].First());

        var method2 =
            typeof(IAcceptLanguageDeclarativeExtractorTest2).GetMethod(
                nameof(IAcceptLanguageDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new AcceptLanguageDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.Headers);
        Assert.Single(httpRequestBuilder2.Headers);
        Assert.Equal("Accept-Language", httpRequestBuilder2.Headers.Keys.First());
        Assert.Equal("zh-CN", httpRequestBuilder2.Headers["Accept-Language"].First());

        var method3 =
            typeof(IAcceptLanguageDeclarativeExtractorTest2).GetMethod(
                nameof(IAcceptLanguageDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new AcceptLanguageDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.Headers);
        Assert.Single(httpRequestBuilder3.Headers);
        Assert.Equal("Accept-Language", httpRequestBuilder3.Headers.Keys.First());
        Assert.Equal("en-US", httpRequestBuilder3.Headers["Accept-Language"].First());
    }
}

[AcceptLanguage(null)]
public interface IAcceptLanguageDeclarativeExtractorTest1 : IHttpDeclarativeExtractor
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[AcceptLanguage("zh-CN")]
public interface IAcceptLanguageDeclarativeExtractorTest2 : IHttpDeclarativeExtractor
{
    [Post("http://localhost:5000")]
    Task Test1();

    [AcceptLanguage("en-US")]
    [Post("http://localhost:5000")]
    Task Test2();
}