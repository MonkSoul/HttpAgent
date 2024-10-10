// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class HttpContextForwardOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new HttpContextForwardOptions();
        Assert.True(options.ForwardStatusCode);
        Assert.True(options.ForwardResponseHeaders);
        Assert.True(options.ForwardContentDispositionHeader);
        Assert.Null(options.OnForwarding);

        options.ForwardStatusCode = false;
        options.ForwardResponseHeaders = false;
        options.OnForwarding = (_, _) =>
        {
        };
    }
}