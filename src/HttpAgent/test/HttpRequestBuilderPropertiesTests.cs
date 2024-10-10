// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderPropertiesTests
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.Method);
        Assert.Null(httpRequestBuilder.RequestUri);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.NotNull(httpRequestBuilder2.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri!.ToString());
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.Method);
        Assert.Null(httpRequestBuilder2.TraceIdentifier);
        Assert.Null(httpRequestBuilder2.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder2.ContentEncoding);
        Assert.Null(httpRequestBuilder2.RawContent);
        Assert.Null(httpRequestBuilder2.Headers);
        Assert.Null(httpRequestBuilder2.Fragment);
        Assert.Null(httpRequestBuilder2.Timeout);
        Assert.Null(httpRequestBuilder2.QueryParameters);
        Assert.Null(httpRequestBuilder2.PathParameters);
        Assert.Null(httpRequestBuilder2.Cookies);
        Assert.Null(httpRequestBuilder2.HttpClientFactoryName);
        Assert.Null(httpRequestBuilder2.HttpClientProvider);
        Assert.Null(httpRequestBuilder2.HttpContentProcessorProviders);
        Assert.Null(httpRequestBuilder2.HttpContentConverterProviders);
        Assert.Null(httpRequestBuilder2.OnPreSetContent);
        Assert.Null(httpRequestBuilder2.OnPreSendRequest);
        Assert.Null(httpRequestBuilder2.OnPostSendRequest);
        Assert.Null(httpRequestBuilder2.OnSendRequestFailed);
        Assert.Null(httpRequestBuilder2.AuthenticationHeader);
        Assert.Null(httpRequestBuilder2.MultipartFormDataBuilder);
        Assert.False(httpRequestBuilder2.EnsureSuccessStatusCodeEnabled);
        Assert.False(httpRequestBuilder2.DisableCacheEnabled);
        Assert.Null(httpRequestBuilder2.RequestEventHandlerType);
        Assert.Null(httpRequestBuilder2.Disposables);
        Assert.Null(httpRequestBuilder2.HttpClientPooling);
        Assert.False(httpRequestBuilder2.HttpClientPoolingEnabled);
    }
}