﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     长轮询管理器
/// </summary>
internal sealed class LongPollingManager
{
    /// <inheritdoc cref="HttpLongPollingBuilder" />
    internal readonly HttpLongPollingBuilder _httpLongPollingBuilder;

    /// <inheritdoc cref="IHttpRemoteService" />
    internal readonly IHttpRemoteService _httpRemoteService;

    /// <summary>
    ///     <inheritdoc cref="LongPollingManager" />
    /// </summary>
    /// <param name="httpRemoteService">
    ///     <see cref="IHttpRemoteService" />
    /// </param>
    /// <param name="httpLongPollingBuilder">
    ///     <see cref="HttpLongPollingBuilder" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    internal LongPollingManager(IHttpRemoteService httpRemoteService, HttpLongPollingBuilder httpLongPollingBuilder,
        Action<HttpRequestBuilder>? configure = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteService);
        ArgumentNullException.ThrowIfNull(httpLongPollingBuilder);

        _httpRemoteService = httpRemoteService;
        _httpLongPollingBuilder = httpLongPollingBuilder;

        // 解析 IHttpLongPollingEventHandler 事件处理程序
        LongPollingEventHandler = (httpLongPollingBuilder.LongPollingEventHandlerType is not null
            ? httpRemoteService.ServiceProvider.GetService(httpLongPollingBuilder.LongPollingEventHandlerType)
            : null) as IHttpLongPollingEventHandler;

        // 构建 HttpRequestBuilder 实例
        RequestBuilder = httpLongPollingBuilder.Build(httpRemoteService.ServiceProvider
            .GetRequiredService<IOptions<HttpRemoteOptions>>().Value, configure);
    }

    /// <summary>
    ///     当前重试次数
    /// </summary>
    internal int CurrentRetries { get; private set; }

    /// <summary>
    ///     <inheritdoc cref="HttpRequestBuilder" />
    /// </summary>
    internal HttpRequestBuilder RequestBuilder { get; }

    /// <summary>
    ///     <inheritdoc cref="IHttpLongPollingEventHandler" />
    /// </summary>
    internal IHttpLongPollingEventHandler? LongPollingEventHandler { get; }

    /// <summary>
    ///     开始请求
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void Start(CancellationToken cancellationToken = default)
    {
        // 初始化数据接收传输的通道
        var dataChannel = Channel.CreateUnbounded<HttpResponseMessage>();

        // 初始化接收服务器响应数据任务
        var fetchResponseTask = FetchResponseAsync(dataChannel, cancellationToken);

        try
        {
            // 循环读取数据直到取消请求或读取完毕
            while (!cancellationToken.IsCancellationRequested)
            {
                // 发送 HTTP 远程请求
                var httpResponseMessage = _httpRemoteService.Send(RequestBuilder, cancellationToken);

                // 发送响应数据对象到通道
                dataChannel.Writer.TryWrite(httpResponseMessage);

                // 检查是否应该终止长轮询
                if (ShouldTerminatePolling(httpResponseMessage))
                {
                    break;
                }

                // 检查是否请求成功
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    // 重置当前重试次数
                    CurrentRetries = 0;
                }
            }
        }
        // 任务被取消
        catch (Exception e) when (cancellationToken.IsCancellationRequested || e is OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            // 检查是否达到了最大当前重试次数
            if (CurrentRetries < _httpLongPollingBuilder.MaxRetries)
            {
                // 重新开始接收
                Retry(cancellationToken);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Failed to establish server connection after `{_httpLongPollingBuilder.MaxRetries}` attempts.",
                    e);
            }
        }
        finally
        {
            // 关闭通道
            dataChannel.Writer.Complete();

            // 等待接收服务器响应数据任务完成
            fetchResponseTask.Wait(cancellationToken);
        }
    }

    /// <summary>
    ///     开始请求
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <exception cref="InvalidOperationException"></exception>
    internal async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // 初始化数据接收传输的通道
        var dataChannel = Channel.CreateUnbounded<HttpResponseMessage>();

        // 初始化接收服务器响应数据任务
        var fetchResponseTask = FetchResponseAsync(dataChannel, cancellationToken);

        try
        {
            // 循环读取数据直到取消请求或读取完毕
            while (!cancellationToken.IsCancellationRequested)
            {
                // 发送 HTTP 远程请求
                var httpResponseMessage = await _httpRemoteService.SendAsync(RequestBuilder, cancellationToken);

                // 发送响应数据对象到通道
                await dataChannel.Writer.WriteAsync(httpResponseMessage, cancellationToken);

                // 检查是否应该终止长轮询
                if (ShouldTerminatePolling(httpResponseMessage))
                {
                    break;
                }

                // 检查是否请求成功
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    // 重置当前重试次数
                    CurrentRetries = 0;
                }
            }
        }
        // 任务被取消
        catch (Exception e) when (cancellationToken.IsCancellationRequested || e is OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            // 检查是否达到了最大当前重试次数
            if (CurrentRetries < _httpLongPollingBuilder.MaxRetries)
            {
                // 重新开始接收
                await RetryAsync(cancellationToken);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Failed to establish server connection after `{_httpLongPollingBuilder.MaxRetries}` attempts.",
                    e);
            }
        }
        finally
        {
            // 关闭通道
            dataChannel.Writer.Complete();

            // 等待接收服务器响应数据任务完成
            await fetchResponseTask;
        }
    }

    /// <summary>
    ///     重新开始请求
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal void Retry(CancellationToken cancellationToken = default)
    {
        // 递增当前重试次数
        CurrentRetries++;

        // 根据配置的重新连接的间隔时间延迟重新开始请求
        Task.Delay(_httpLongPollingBuilder.RetryInterval, cancellationToken).Wait(cancellationToken);

        // 重新开始接收
        Start(cancellationToken);
    }

    /// <summary>
    ///     重新开始请求
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task RetryAsync(CancellationToken cancellationToken = default)
    {
        // 递增当前重试次数
        CurrentRetries++;

        // 根据配置的重新连接的间隔时间延迟重新开始请求
        await Task.Delay(_httpLongPollingBuilder.RetryInterval, cancellationToken);

        // 重新开始接收
        await StartAsync(cancellationToken);
    }

    /// <summary>
    ///     检查是否应该终止长轮询
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldTerminatePolling(HttpResponseMessage httpResponseMessage)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 检查响应标头中是否存在长轮询结束符
        if (httpResponseMessage.Headers.TryGetValues(Constants.X_END_OF_STREAM_HEADER, out _))
        {
            return true;
        }

        // 如果响应状态码不是成功的，则递增当前重试次数
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            CurrentRetries++;
        }

        return CurrentRetries >= _httpLongPollingBuilder.MaxRetries;
    }

    /// <summary>
    ///     接收服务器响应数据任务
    /// </summary>
    /// <param name="dataChannel">数据接收传输的通道</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task FetchResponseAsync(Channel<HttpResponseMessage> dataChannel,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(dataChannel);

        // 空检查
        if (_httpLongPollingBuilder.OnDataReceived is null && _httpLongPollingBuilder.OnError is null &&
            LongPollingEventHandler is null)
        {
            return;
        }

        try
        {
            // 从数据接收传输的通道中读取所有的数据
            await foreach (var httpResponseMessage in dataChannel.Reader.ReadAllAsync(cancellationToken))
            {
                // 如果请求了取消，则抛出 OperationCanceledException
                cancellationToken.ThrowIfCancellationRequested();

                // 处理服务器响应数据
                await HandleResponseAsync(httpResponseMessage, cancellationToken);
            }
        }
        catch (Exception e) when (cancellationToken.IsCancellationRequested || e is OperationCanceledException)
        {
            // 任务被取消
        }
        catch (Exception e)
        {
            // 输出调试事件
            Debugging.Error(e.Message);
        }
    }

    /// <summary>
    ///     处理服务器响应数据
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task HandleResponseAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 检查响应标头中是否存在长轮询结束符
        if (httpResponseMessage.Headers.TryGetValues(Constants.X_END_OF_STREAM_HEADER, out _))
        {
            await HandleEndOfStreamAsync(httpResponseMessage, cancellationToken);

            return;
        }

        // 处理服务器返回 200~299 状态码的数据
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            await HandleDataReceivedAsync(httpResponseMessage, cancellationToken);
        }
        // 处理服务器返回非 200~299 状态码的数据
        else
        {
            await HandleErrorAsync(httpResponseMessage, cancellationToken);
        }
    }

    /// <summary>
    ///     处理服务器返回 <c>200~299</c> 状态码的数据
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task HandleDataReceivedAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 空检查
        if (LongPollingEventHandler is not null)
        {
            await DelegateExtensions.TryInvokeAsync(LongPollingEventHandler.OnDataReceivedAsync, httpResponseMessage,
                cancellationToken);
        }

        await _httpLongPollingBuilder.OnDataReceived.TryInvokeAsync(httpResponseMessage, cancellationToken);
    }

    /// <summary>
    ///     处理服务器返回非 <c>200~299</c> 状态码的数据
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task HandleErrorAsync(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 空检查
        if (LongPollingEventHandler is not null)
        {
            await DelegateExtensions.TryInvokeAsync(LongPollingEventHandler.OnErrorAsync, httpResponseMessage,
                cancellationToken);
        }

        await _httpLongPollingBuilder.OnError.TryInvokeAsync(httpResponseMessage, cancellationToken);
    }

    /// <summary>
    ///     处理服务器响应标头包含 <c>X-End-Of-Stream</c> 时触发的操作
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal async Task HandleEndOfStreamAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 空检查
        if (LongPollingEventHandler is not null)
        {
            await DelegateExtensions.TryInvokeAsync(LongPollingEventHandler.OnEndOfStreamAsync, httpResponseMessage,
                cancellationToken);
        }

        await _httpLongPollingBuilder.OnEndOfStream.TryInvokeAsync(httpResponseMessage, cancellationToken);
    }
}