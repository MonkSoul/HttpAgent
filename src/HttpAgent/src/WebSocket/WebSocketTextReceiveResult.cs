// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     WebSocket 接收的文本消息的结果类
/// </summary>
public sealed class WebSocketTextReceiveResult : WebSocketReceiveResult
{
    /// <inheritdoc />
    public WebSocketTextReceiveResult(int count, bool endOfMessage)
        : base(count, WebSocketMessageType.Text, endOfMessage)
    {
    }

    /// <inheritdoc />
    public WebSocketTextReceiveResult(int count, bool endOfMessage, WebSocketCloseStatus? closeStatus,
        string? closeStatusDescription)
        : base(count, WebSocketMessageType.Text, endOfMessage, closeStatus, closeStatusDescription)
    {
    }

    /// <summary>
    ///     文本消息
    /// </summary>
    public string Message { get; internal init; } = default!;
}