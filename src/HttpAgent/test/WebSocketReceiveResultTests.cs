// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketReceiveResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(WebSocketReceiveResult<string>).BaseType == typeof(WebSocketReceiveResult));

        var webSocketReceiveResult1 =
            new WebSocketReceiveResult<string>(10, WebSocketMessageType.Text, true) { Result = "Furion" };
        Assert.NotNull(webSocketReceiveResult1);
        Assert.Equal("Furion", webSocketReceiveResult1.Result);

        var webSocketReceiveResult2 =
            new WebSocketReceiveResult<byte[]>(10, WebSocketMessageType.Text, true) { Result = "Furion"u8.ToArray() };
        Assert.NotNull(webSocketReceiveResult2);
        Assert.Equal("Furion", Encoding.UTF8.GetString(webSocketReceiveResult2.Result));
    }
}