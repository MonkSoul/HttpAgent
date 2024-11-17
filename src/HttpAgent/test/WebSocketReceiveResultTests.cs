// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketReceiveResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(WebSocketTextReceiveResult).BaseType == typeof(WebSocketReceiveResult));

        var webSocketTextReceiveResult =
            new WebSocketTextReceiveResult(10, true) { Message = "Furion" };
        Assert.NotNull(webSocketTextReceiveResult);
        Assert.Equal("Furion", webSocketTextReceiveResult.Message);

        Assert.True(typeof(WebSocketBinaryReceiveResult).BaseType == typeof(WebSocketReceiveResult));

        var webSocketBinaryReceiveResult =
            new WebSocketBinaryReceiveResult(10, true) { Message = "Furion"u8.ToArray() };
        Assert.NotNull(webSocketBinaryReceiveResult);
        Assert.Equal("Furion", Encoding.UTF8.GetString(webSocketBinaryReceiveResult.Message));
    }
}