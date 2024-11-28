// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        Assert.Equal("text/plain", httpRemoteOptions.DefaultContentType);
        Assert.Null(httpRemoteOptions.DefaultFileDownloadDirectory);
        Assert.Null(httpRemoteOptions.HttpDeclarativeExtractors);

        Assert.True(HttpRemoteOptions.JsonSerializerOptionsDefault.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNamingPolicy.CamelCase, HttpRemoteOptions.JsonSerializerOptionsDefault.PropertyNamingPolicy);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString,
            HttpRemoteOptions.JsonSerializerOptionsDefault.NumberHandling);

        Assert.NotNull(httpRemoteOptions.JsonSerializerOptions);
        Assert.Equal(HttpRemoteOptions.JsonSerializerOptionsDefault, httpRemoteOptions.JsonSerializerOptions);
    }
}