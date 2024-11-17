// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketClientEventsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var webSocketClient = new WebSocketClient("ws://localhost:12345");
        var events = new string[10];

        webSocketClient.Connecting += (s, e) =>
        {
            events[0] = nameof(webSocketClient.Connecting);
        };
        webSocketClient.Connected += (s, e) =>
        {
            events[1] = nameof(webSocketClient.Connected);
        };
        webSocketClient.Reconnecting += (s, e) =>
        {
            events[2] = nameof(webSocketClient.Reconnecting);
        };
        webSocketClient.Reconnected += (s, e) =>
        {
            events[3] = nameof(webSocketClient.Reconnected);
        };
        webSocketClient.Closing += (s, e) =>
        {
            events[4] = nameof(webSocketClient.Closing);
        };
        webSocketClient.Closed += (s, e) =>
        {
            events[5] = nameof(webSocketClient.Closed);
        };
        webSocketClient.ReceivingStarted += (s, e) =>
        {
            events[6] = nameof(webSocketClient.ReceivingStarted);
        };
        webSocketClient.ReceivingStopped += (s, e) =>
        {
            events[7] = nameof(webSocketClient.ReceivingStopped);
        };
        webSocketClient.TextReceived += (s, e) =>
        {
            events[8] = nameof(webSocketClient.TextReceived);
        };
        webSocketClient.BinaryReceived += (s, e) =>
        {
            events[9] = nameof(webSocketClient.BinaryReceived);
        };

        webSocketClient.OnConnecting();
        webSocketClient.OnConnected();
        webSocketClient.OnReconnecting();
        webSocketClient.OnReconnected();
        webSocketClient.OnClosing();
        webSocketClient.OnClosed();
        webSocketClient.OnReceivingStarted();
        webSocketClient.OnReceivingStopped();
        webSocketClient.OnTextReceived(new WebSocketTextReceiveResult(0, true, WebSocketCloseStatus.Empty, null));
        webSocketClient.OnBinaryReceived(new WebSocketBinaryReceiveResult(0, true, WebSocketCloseStatus.Empty, null));

        Assert.Equal(
        [
            "Connecting", "Connected", "Reconnecting", "Reconnected", "Closing", "Closed", "ReceivingStarted",
            "ReceivingStopped", "TextReceived", "BinaryReceived"
        ], events);
    }
}