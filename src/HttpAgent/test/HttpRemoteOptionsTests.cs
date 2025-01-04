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
        Assert.Equal(LogLevel.Warning, httpRemoteOptions.ProfilerLogLevel);
        Assert.True(httpRemoteOptions.AllowAutoRedirect);
        Assert.Equal(50, httpRemoteOptions.MaximumAutomaticRedirections);
        Assert.False(httpRemoteOptions.IsLoggingRegistered);
        Assert.Null(httpRemoteOptions.Configuration);

        Assert.True(HttpRemoteOptions.JsonSerializerOptionsDefault.PropertyNameCaseInsensitive);
        Assert.Equal(JsonNamingPolicy.CamelCase, HttpRemoteOptions.JsonSerializerOptionsDefault.PropertyNamingPolicy);
        Assert.Equal(JsonNumberHandling.AllowReadingFromString,
            HttpRemoteOptions.JsonSerializerOptionsDefault.NumberHandling);

        Assert.NotNull(httpRemoteOptions.JsonSerializerOptions);
        Assert.NotEqual(HttpRemoteOptions.JsonSerializerOptionsDefault, httpRemoteOptions.JsonSerializerOptions);
    }
}