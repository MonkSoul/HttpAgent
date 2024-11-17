// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     WebSocket 接收的二进制消息的结果类
/// </summary>
public sealed class WebSocketBinaryReceiveResult : WebSocketReceiveResult
{
    /// <inheritdoc />
    public WebSocketBinaryReceiveResult(int count, bool endOfMessage)
        : base(count, WebSocketMessageType.Binary, endOfMessage)
    {
    }

    /// <inheritdoc />
    public WebSocketBinaryReceiveResult(int count, bool endOfMessage, WebSocketCloseStatus? closeStatus,
        string? closeStatusDescription)
        : base(count, WebSocketMessageType.Binary, endOfMessage, closeStatus, closeStatusDescription)
    {
    }

    /// <summary>
    ///     二进制消息
    /// </summary>
    public byte[] Message { get; internal init; } = default!;
}