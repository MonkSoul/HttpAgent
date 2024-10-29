// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MultipartBodyDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(
                typeof(MultipartBodyDeclarativeExtractor)));

        var extractor = new MultipartBodyDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        Action<HttpMultipartFormDataBuilder> builder = builder =>
        {
            builder.Add(new StringContent(""), "str", "text/plain");
        };
        var method1 =
            typeof(IMultipartBodyDeclarativeExtractorTest1).GetMethod(
                nameof(IMultipartBodyDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [builder, builder]);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new MultipartBodyDeclarativeExtractor().Extract(httpRequestBuilder1, context1));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        Action<HttpMultipartFormDataBuilder> builder = builder =>
        {
            builder.Add(new StringContent(""), "str", "text/plain");
        };

        var method1 =
            typeof(IMultipartBodyDeclarativeExtractorTest2).GetMethod(
                nameof(IMultipartBodyDeclarativeExtractorTest2.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new MultipartBodyDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Timeout);

        var method2 =
            typeof(IMultipartBodyDeclarativeExtractorTest2).GetMethod(
                nameof(IMultipartBodyDeclarativeExtractorTest2.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [builder]);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new MultipartBodyDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.MultipartFormDataBuilder);
    }
}

public interface IMultipartBodyDeclarativeExtractorTest1 : IHttpDeclarativeExtractor
{
    [Post("http://localhost:5000")]
    Task Test1(Action<HttpMultipartFormDataBuilder> configure, Action<HttpMultipartFormDataBuilder> configure2);
}

public interface IMultipartBodyDeclarativeExtractorTest2 : IHttpDeclarativeExtractor
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2(Action<HttpMultipartFormDataBuilder> configure);
}