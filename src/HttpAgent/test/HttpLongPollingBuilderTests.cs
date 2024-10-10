// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpLongPollingBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpLongPollingBuilder(null!, null));

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, null!);
        Assert.NotNull(builder);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.NotNull(builder2);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(HttpMethod.Get, builder2.Method);
        Assert.Equal(TimeSpan.FromSeconds(5), builder2.PollingInterval);
        Assert.Equal(100, builder2.MaxRetries);
        Assert.Null(builder2.OnDataReceived);
        Assert.Null(builder2.LongPollingEventHandlerType);
    }

    [Fact]
    public void SetPollingInterval_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetPollingInterval(TimeSpan.Zero));
        Assert.Equal("Polling interval must be greater than 0. (Parameter 'pollingInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetPollingInterval(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Polling interval must be greater than 0. (Parameter 'pollingInterval')", exception2.Message);
    }

    [Fact]
    public void SetPollingInterval_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetPollingInterval(TimeSpan.FromMilliseconds(6000));
        Assert.Equal(TimeSpan.FromMilliseconds(6000), builder.PollingInterval);
    }

    [Fact]
    public void SetMaxRetries_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(-1));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(0));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception2.Message);
    }

    [Fact]
    public void SetMaxRetries_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetMaxRetries(5);
        Assert.Equal(5, builder.MaxRetries);
    }

    [Fact]
    public void SetOnDataReceived_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnDataReceived(null!));
    }

    [Fact]
    public void SetOnDataReceived_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetOnDataReceived(async _ => await Task.CompletedTask);
        Assert.NotNull(builder.OnDataReceived);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetEventHandler(typeof(NotLongPollingEventHandler)));
        Assert.Equal(
            $"`{typeof(NotLongPollingEventHandler)}` type is not assignable from `{typeof(IHttpLongPollingEventHandler)}`.",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetEventHandler(typeof(CustomLongPollingEventHandler));

        Assert.Equal(typeof(CustomLongPollingEventHandler), builder.LongPollingEventHandlerType);

        var builder2 = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder2.SetEventHandler<CustomLongPollingEventHandler>();

        Assert.Equal(typeof(CustomLongPollingEventHandler), builder2.LongPollingEventHandlerType);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost")).Build(null!));

    [Fact]
    public void Build_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var httpLongPollingBuilder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var httpRequestBuilder = httpLongPollingBuilder.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);

        var httpRequestBuilder2 = httpLongPollingBuilder.SetEventHandler<CustomLongPollingEventHandler2>()
            .Build(httpRemoteOptions,
                builder =>
                {
                    builder.SetTimeout(100);
                });

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }
}