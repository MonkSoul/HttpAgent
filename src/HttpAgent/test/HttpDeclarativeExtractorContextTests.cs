﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpDeclarativeExtractorContextTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeExtractorContext(null!, null!));

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        Assert.Throws<ArgumentNullException>(() => new HttpDeclarativeExtractorContext(method!, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        Assert.Equal([
            typeof(Action<HttpRequestBuilder>), typeof(Action<HttpMultipartFormDataBuilder>),
            typeof(HttpCompletionOption),
            typeof(CancellationToken)
        ], HttpDeclarativeExtractorContext._specialParameterTypes);

        var method1 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1));
        var context1 = new HttpDeclarativeExtractorContext(method1!, []);
        Assert.Equal(method1, context1.Method);
        Assert.Equal([], context1.Args);
        Assert.Empty(context1.Parameters);
        Assert.Equal(typeof(ReadOnlyDictionary<,>), context1.Parameters.GetType().GetGenericTypeDefinition());

        var method2 = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method2));
        var args2 = new object?[] { 1, "furion" };
        var context2 = new HttpDeclarativeExtractorContext(method2!, args2);
        Assert.Equal(method2, context2.Method);
        Assert.Equal(args2, context2.Args);
        Assert.Equal(2, context2.Parameters.Count);

        Assert.Equal("id", context2.Parameters.Keys.First().Name);
        Assert.Equal(1, context2.Parameters.Values.First());
        Assert.Equal("name", context2.Parameters.Keys.Last().Name);
        Assert.Equal("furion", context2.Parameters.Values.Last());
    }

    [Fact]
    public void FilterSpecialParameter_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpDeclarativeExtractorContext.FilterSpecialParameter(null!));

    [Fact]
    public void FilterSpecialParameter_ReturnOK()
    {
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Special))!;
        var parameters = method.GetParameters();

        Assert.True(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[0]));
        Assert.True(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[1]));
        Assert.False(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[2]));
        Assert.False(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[3]));
        Assert.False(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[4]));
        Assert.False(HttpDeclarativeExtractorContext.FilterSpecialParameter(parameters[5]));
    }
}