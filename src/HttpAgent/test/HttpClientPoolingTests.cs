// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpClientPoolingTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpClientPooling(null!, null!));

    [Fact]
    public void New_ReturnOK()
    {
        var httpClient = new HttpClient();

        var httpClientPooling = new HttpClientPooling(httpClient, null);
        Assert.NotNull(httpClientPooling.Instance);
        Assert.Null(httpClientPooling.Release);

        var i = 0;
        var httpClientPooling2 = new HttpClientPooling(httpClient, client =>
        {
            i++;
            client.Dispose();
        });

        Assert.NotNull(httpClientPooling2.Instance);
        Assert.NotNull(httpClientPooling2.Release);
        httpClientPooling2.Release.Invoke(httpClient);

        Assert.Equal(1, i);
    }
}