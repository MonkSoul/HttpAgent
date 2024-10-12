// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     WebSocket 客户端配置选项
/// </summary>
public sealed class WebSocketClientOptions
{
    /// <summary>
    ///     <inheritdoc cref="WebSocketClientOptions" />
    /// </summary>
    /// <param name="serverUri">服务器地址</param>
    public WebSocketClientOptions(string serverUri)
        : this(new Uri(serverUri ?? throw new ArgumentNullException(nameof(serverUri))))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="WebSocketClientOptions" />
    /// </summary>
    /// <param name="serverUri">服务器地址</param>
    public WebSocketClientOptions(Uri serverUri)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serverUri);

        ServerUri = serverUri;
    }

    /// <summary>
    ///     服务器地址
    /// </summary>
    public Uri ServerUri { get; }

    /// <summary>
    ///     重新连接的间隔时间（毫秒）
    /// </summary>
    /// <remarks>默认值为 2000 毫秒。</remarks>
    public int ReconnectInterval { get; set; } = 2000;

    /// <summary>
    ///     最大重连次数
    /// </summary>
    /// <remarks>默认最大重连次数为 10。</remarks>
    public int MaxReconnectRetries { get; set; } = 10;

    /// <summary>
    ///     超时时间
    /// </summary>
    public TimeSpan? Timeout { get; set; }
}