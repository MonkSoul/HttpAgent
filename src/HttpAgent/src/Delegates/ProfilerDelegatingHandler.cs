// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求分析工具处理委托
/// </summary>
/// <remarks>参考文献：https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#outgoing-request-middleware</remarks>
/// <param name="logger">
///     <see cref="Logger{T}" />
/// </param>
public sealed class ProfilerDelegatingHandler(ILogger<Logging> logger) : DelegatingHandler
{
    /// <summary>
    ///     是否启用请求分析工具
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsEnabled(HttpRequestMessage httpRequestMessage) =>
        !(httpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY),
            out var value) && value == "TRUE");

    /// <inheritdoc />
    protected override HttpResponseMessage Send(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        // 检查是否启用请求分析工具
        if (!IsEnabled(httpRequestMessage))
        {
            return base.Send(httpRequestMessage, cancellationToken);
        }

        // 记录请求标头
        LogRequestHeaders(logger, httpRequestMessage);

        // 打印 CookieContainer 内容
        LogCookieContainer(logger, httpRequestMessage, ExtractSocketsHttpHandler());

        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        // 发送 HTTP 远程请求
        var httpResponseMessage = base.Send(httpRequestMessage, cancellationToken);

        // 获取请求耗时
        var requestDuration = stopwatch.ElapsedMilliseconds;

        // 停止计时
        stopwatch.Stop();

        // 记录常规和响应标头
        LogResponseHeadersAndSummary(logger, httpResponseMessage, requestDuration);

        return httpResponseMessage;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        // 检查是否启用请求分析工具
        if (!IsEnabled(httpRequestMessage))
        {
            return await base.SendAsync(httpRequestMessage, cancellationToken);
        }

        // 记录请求标头
        LogRequestHeaders(logger, httpRequestMessage);

        // 打印 CookieContainer 内容
        LogCookieContainer(logger, httpRequestMessage, ExtractSocketsHttpHandler());

        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        // 发送 HTTP 远程请求
        var httpResponseMessage = await base.SendAsync(httpRequestMessage, cancellationToken);

        // 获取请求耗时
        var requestDuration = stopwatch.ElapsedMilliseconds;

        // 停止计时
        stopwatch.Stop();

        // 记录常规和响应标头
        LogResponseHeadersAndSummary(logger, httpResponseMessage, requestDuration);

        return httpResponseMessage;
    }

    /// <summary>
    ///     记录请求标头
    /// </summary>
    /// <param name="logger">
    ///     <see cref="ILogger" />
    /// </param>
    /// <param name="request">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal static void LogRequestHeaders(ILogger logger, HttpRequestMessage request) =>
        Log(logger, request.ProfilerHeaders());

    /// <summary>
    ///     记录常规和响应标头
    /// </summary>
    /// <param name="logger">
    ///     <see cref="ILogger" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    internal static void LogResponseHeadersAndSummary(ILogger logger, HttpResponseMessage httpResponseMessage,
        long requestDuration) =>
        Log(logger, httpResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
            [new KeyValuePair<string, IEnumerable<string>>("Request Duration (ms)", [$"{requestDuration:N2}"])]));

    /// <summary>
    ///     打印 <see cref="CookieContainer" /> 内容
    /// </summary>
    /// <param name="logger">
    ///     <see cref="ILogger" />
    /// </param>
    /// <param name="request">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="socketsHttpHandler">
    ///     <see cref="SocketsHttpHandler" />
    /// </param>
    internal static void LogCookieContainer(ILogger logger, HttpRequestMessage request,
        SocketsHttpHandler? socketsHttpHandler)
    {
        // 空检查
        if (socketsHttpHandler is null || request.RequestUri is null)
        {
            return;
        }

        // 获取 Cookie 集合
        var cookies = socketsHttpHandler.CookieContainer.GetCookies(request.RequestUri);

        // 空检查
        if (cookies is { Count: 0 })
        {
            return;
        }

        // 打印日志
        Log(logger, StringUtility.FormatKeyValuesSummary(
            cookies.ToDictionary(u => u.Name, u => Enumerable.Empty<string>().Concat([u.Value])),
            "Cookie Container"));
    }

    /// <summary>
    ///     打印日志
    /// </summary>
    /// <param name="logger">
    ///     <see cref="ILogger" />
    /// </param>
    /// <param name="message">日志消息</param>
    internal static void Log(ILogger logger, string? message)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(logger);

        // 空检查
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        // 打印日志
        logger.LogInformation("{message}", message);
    }

    /// <summary>
    ///     提取 <see cref="SocketsHttpHandler" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="SocketsHttpHandler" />
    /// </returns>
    internal SocketsHttpHandler? ExtractSocketsHttpHandler() =>
        InnerHandler switch
        {
            LoggingHttpMessageHandler loggingHttpMessageHandler =>
                loggingHttpMessageHandler.InnerHandler as SocketsHttpHandler,
            LoggingScopeHttpMessageHandler loggingScopeHttpMessageHandler =>
                loggingScopeHttpMessageHandler.InnerHandler as SocketsHttpHandler,
            SocketsHttpHandler innerSocketsHttpHandler => innerSocketsHttpHandler,
            _ => null
        };
}