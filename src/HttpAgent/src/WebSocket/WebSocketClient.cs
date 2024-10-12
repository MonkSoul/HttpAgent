// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     WebSocket 客户端
/// </summary>
public sealed partial class WebSocketClient : IDisposable
{
    /// <inheritdoc cref="ClientWebSocket" />
    internal ClientWebSocket? _clientWebSocket;

    /// <summary>
    ///     取消接收服务器消息标记
    /// </summary>
    internal CancellationTokenSource? _messageCancellationTokenSource;

    /// <summary>
    ///     接收服务器消息任务
    /// </summary>
    internal Task? _receiveMessagesTask;

    /// <summary>
    ///     <inheritdoc cref="WebSocketClient" />
    /// </summary>
    /// <param name="serverUri">服务器地址</param>
    public WebSocketClient(string serverUri)
        : this(new WebSocketClientOptions(serverUri))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="WebSocketClient" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="WebSocketClientOptions" />
    /// </param>
    public WebSocketClient(WebSocketClientOptions options)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(options);

        Options = options;
    }

    /// <summary>
    ///     <see cref="WebSocketClientOptions" />
    /// </summary>
    internal WebSocketClientOptions Options { get; }

    /// <summary>
    ///     当前重连次数
    /// </summary>
    internal int CurrentReconnectRetries { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        // 释放 ClientWebSocket 实例
        _clientWebSocket?.Dispose();
        _clientWebSocket = null;

        // 等待接收服务器消息任务完成
        _messageCancellationTokenSource?.Cancel();
        _messageCancellationTokenSource?.Dispose();
        _messageCancellationTokenSource = null;

        _receiveMessagesTask?.Wait();
    }

    /// <summary>
    ///     连接到服务器
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        // 初始化 ClientWebSocket 实例
        _clientWebSocket ??= new ClientWebSocket();

        // 检查连接是否处于正在连接或打开状态，如果是则跳过
        if (_clientWebSocket.State is WebSocketState.Connecting or WebSocketState.Open)
        {
            return;
        }

        // 创建关联的超时 Token 标识
        using var timeoutCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // 设置连接超时时间控制
        if (Options.Timeout is not null && Options.Timeout.Value != TimeSpan.Zero)
        {
            timeoutCancellationTokenSource.CancelAfter(Options.Timeout.Value);
        }

        // 触发开始连接事件
        var onConnecting = OnConnecting;
        onConnecting.TryInvoke();

        try
        {
            // 连接到服务器
            await _clientWebSocket.ConnectAsync(Options.ServerUri, timeoutCancellationTokenSource.Token);

            // 重置当前重连次数
            CurrentReconnectRetries = 0;

            // 触发连接成功事件
            var onConnected = OnConnected;
            onConnected.TryInvoke();

            // 初始化接收服务器消息任务
            _receiveMessagesTask = ReceiveMessagesAsync(cancellationToken);
        }
        catch (Exception e) when (cancellationToken.IsCancellationRequested || e is OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);

            // 检查是否达到了最大重连次数
            if (CurrentReconnectRetries < Options.MaxReconnectRetries)
            {
                // 触发开始重新连接事件
                var onReconnecting = OnReconnecting;
                onReconnecting.TryInvoke();

                // 重新连接到服务器
                await ReconnectAsync(cancellationToken);
            }
            else
            {
                throw;
            }
        }
    }

    /// <summary>
    ///     重新连接到服务器
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task ReconnectAsync(CancellationToken cancellationToken = default)
    {
        // 递增当前重连次数
        CurrentReconnectRetries++;

        // 根据配置的重连的间隔时间延迟重新开始连接
        await Task.Delay(Options.ReconnectInterval, cancellationToken);

        // 重新连接到服务器
        await ConnectAsync(cancellationToken);

        // 触发重新连接成功事件
        var onReconnected = OnReconnected;
        onReconnected.TryInvoke();
    }

    /// <summary>
    ///     接收服务器消息
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(_clientWebSocket);

        // 创建关联的取消接收服务器消息 Token 标识
        _messageCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // 触发开始接收消息事件
        var onStartedReceiving = OnStartedReceivingMessages;
        onStartedReceiving.TryInvoke();

        // 初始化缓冲区大小
        var buffer = new byte[1024 * 4];

        try
        {
            // 循环读取服务器消息直到取消请求或连接处于非打开状态
            while (!cancellationToken.IsCancellationRequested && _clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    // 获取接收到的数据
                    var received =
                        await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    // 如果接收到关闭帧，则退出循环
                    if (received.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (received.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            // 解码接收到的文本消息
                            var message = Encoding.UTF8.GetString(buffer, 0, received.Count);

                            // 触发接收文本消息事件
                            var onMessageReceived = OnMessageReceived;
                            onMessageReceived.TryInvoke(message);
                            break;
                        case WebSocketMessageType.Binary:
                            // 将接收到的数据从原始缓冲区复制到新创建的字节数组中
                            var bytes = new byte[received.Count];
                            Buffer.BlockCopy(buffer, 0, bytes, 0, received.Count);

                            // 触发接收二进制消息事件
                            var onBinaryMessageReceived = OnBinaryMessageReceived;
                            onBinaryMessageReceived.TryInvoke(bytes);
                            break;
                    }

                    // 如果这是消息的最后一部分，则清空缓冲区
                    if (received.EndOfMessage)
                    {
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception e) when (cancellationToken.IsCancellationRequested || e is OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    // 输出调试事件
                    Debugging.Error(e.Message);
                }
            }
        }
        finally
        {
            // 触发停止接收消息事件
            var onStoppedReceivingMessages = OnStoppedReceivingMessages;
            onStoppedReceivingMessages.TryInvoke();

            // 断开连接
            await DisconnectAsync(cancellationToken);
        }
    }

    /// <summary>
    ///     向服务器发送消息
    /// </summary>
    /// <param name="message">字符串消息</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public Task SendAsync(string message, CancellationToken cancellationToken = default) =>
        SendAsync(message, WebSocketMessageType.Text, cancellationToken);

    /// <summary>
    ///     向服务器发送消息
    /// </summary>
    /// <param name="message">字符串消息</param>
    /// <param name="webSocketMessageType">
    ///     <see cref="WebSocketMessageType" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task SendAsync(string message, WebSocketMessageType webSocketMessageType,
        CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(_clientWebSocket);

        // 检查连接是否处于打开状态
        if (_clientWebSocket.State != WebSocketState.Open)
        {
            return;
        }

        // 将字符串编码为字节数组
        var buffer = Encoding.UTF8.GetBytes(message);

        // 初始化 ArraySegment 实例
        var arraySegment = new ArraySegment<byte>(buffer);

        // 向服务器发送消息
        await _clientWebSocket.SendAsync(arraySegment, webSocketMessageType, true, cancellationToken);
    }

    /// <summary>
    ///     向服务器发送消息
    /// </summary>
    /// <param name="bytes">二进制消息</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(bytes);
        ArgumentNullException.ThrowIfNull(_clientWebSocket);

        // 检查连接是否处于打开状态
        if (_clientWebSocket.State != WebSocketState.Open)
        {
            return;
        }

        // 初始化 ArraySegment 实例
        var arraySegment = new ArraySegment<byte>(bytes);

        // 向服务器发送二进制消息
        await _clientWebSocket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, cancellationToken);
    }

    /// <summary>
    ///     断开连接
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    public async Task DisconnectAsync(CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(_clientWebSocket);

        // 检查连接是否处于打开状态
        if (_clientWebSocket.State != WebSocketState.Open)
        {
            return;
        }

        try
        {
            // 发送关闭帧并断开连接
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
        }
        finally
        {
            // 触发断开连接事件
            var onDisconnected = OnDisconnected;
            onDisconnected.TryInvoke();

            // 释放 WebSocketClient 实例
            Dispose();
        }
    }
}