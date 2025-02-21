// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteAnalyzerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var analyzer = new HttpRemoteAnalyzer();
        Assert.Null(analyzer._cachedData);
        Assert.NotNull(analyzer._dataBuffer);
        Assert.Empty(analyzer.Data);
        Assert.NotNull(analyzer._cachedData);
    }

    [Fact]
    public void AppendData_ReturnOK()
    {
        var analyzer = new HttpRemoteAnalyzer();
        analyzer.AppendData("test");
        Assert.Null(analyzer._cachedData);
        Assert.Equal("test", analyzer.Data);
        Assert.NotNull(analyzer._cachedData);

        Assert.Equal("test", analyzer.ToString());
    }
}