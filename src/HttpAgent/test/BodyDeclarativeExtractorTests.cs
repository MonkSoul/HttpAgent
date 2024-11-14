﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class BodyDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(BodyDeclarativeExtractor)));

        var extractor = new BodyDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        var method3 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test3))!;
        var context = new HttpDeclarativeExtractorContext(method3, ["str1", "str2"]);
        var httpRequestBuilder = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new BodyDeclarativeExtractor().Extract(httpRequestBuilder, context));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.RawContent);
        Assert.Null(httpRequestBuilder1.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder1.ContentEncoding?.BodyName);

        var method2 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, ["str"]);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.Equal("str", httpRequestBuilder2.RawContent);
        Assert.Null(httpRequestBuilder2.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder2.ContentEncoding?.BodyName);

        var method4 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test4))!;
        var context4 = new HttpDeclarativeExtractorContext(method4, ["str"]);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder4, context4);
        Assert.Equal("str", httpRequestBuilder4.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder4.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder4.ContentEncoding?.BodyName);

        var method5 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test5))!;
        var context5 = new HttpDeclarativeExtractorContext(method5, ["str"]);
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder5, context5);
        Assert.Equal("str", httpRequestBuilder5.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder5.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder5.ContentEncoding?.BodyName);

        var method6 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test6))!;
        var context6 = new HttpDeclarativeExtractorContext(method6, ["str"]);
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder6, context6);
        Assert.Equal("str", httpRequestBuilder6.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder6.ContentType);
        Assert.Equal("utf-32", httpRequestBuilder6.ContentEncoding?.BodyName);

        var method7 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test7))!;
        var context7 = new HttpDeclarativeExtractorContext(method7, ["str"]);
        var httpRequestBuilder7 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder7, context7);
        Assert.Equal("str", httpRequestBuilder7.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder7.ContentType);
        Assert.Equal("utf-32", httpRequestBuilder7.ContentEncoding?.BodyName);

        var method8 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test8))!;
        var context8 = new HttpDeclarativeExtractorContext(method8, [new { }]);
        var httpRequestBuilder8 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder8, context8);
        Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder8.ContentType);
        Assert.NotNull(httpRequestBuilder8.HttpContentProcessorProviders);
        Assert.Single(httpRequestBuilder8.HttpContentProcessorProviders);
    }
}

public interface IBodyDeclarativeTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2([Body] string body);

    [Post("http://localhost:5000")]
    Task Test3([Body] string body, [Body] string body2);

    [Post("http://localhost:5000")]
    Task Test4([Body("text/plain")] string body);

    [Post("http://localhost:5000")]
    Task Test5([Body("text/plain", "utf-8")] string body);

    [Post("http://localhost:5000")]
    Task Test6([Body("text/plain; charset=utf-32")] string body);

    [Post("http://localhost:5000")]
    Task Test7([Body("text/plain; charset=utf-8", "utf-32")] string body);

    [Post("http://localhost:5000")]
    Task Test8([Body("application/x-www-form-urlencoded; charset=utf-8", UseStringContent = true)] object body);
}