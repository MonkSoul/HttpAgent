// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Extensions;

/// <summary>
///     <see cref="HttpContext" /> 拓展类
/// </summary>
public static partial class HttpContextExtensions
{
    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static TResult? ForwardAs<TResult>(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<TResult>(httpContext,
        Helpers.ParseHttpMethod(httpContext?.Request.Method),
        string.IsNullOrWhiteSpace(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute), configure,
        completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static TResult? ForwardAs<TResult>(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<TResult>(httpContext, httpMethod,
        string.IsNullOrWhiteSpace(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute), configure,
        completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static TResult? ForwardAs<TResult>(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<TResult>(httpContext,
        Helpers.ParseHttpMethod(httpContext?.Request.Method), requestUri,
        configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static TResult? ForwardAs<TResult>(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(httpMethod);

        // 创建 HttpContextForwardBuilder 实例
        var httpContextForwardBuilder = CreateForwardBuilder(httpContext, httpMethod, requestUri, forwardOptions);

        // 构建 HttpRequestBuilder 实例
        var httpRequestBuilder = httpContextForwardBuilder.Build(configure);

        // 获取 IHttpRemoteService 实例
        var httpRemoteService = httpContext.RequestServices.GetRequiredService<IHttpRemoteService>();

        // 获取 IHttpContentConverterFactory 实例
        var httpContentConverterFactory =
            httpContext.RequestServices.GetRequiredService<IHttpContentConverterFactory>();

        // 发送 HTTP 远程请求
        var httpResponseMessage =
            httpRemoteService.Send(httpRequestBuilder, completionOption, httpContext.RequestAborted);

        // 根据配置选项将 HttpResponseMessage 信息转发到 HttpContext 中
        ForwardResponseMessage(httpContext, httpResponseMessage, httpContextForwardBuilder.ForwardOptions);

        // 将 HttpResponseMessage 转换为 TResult 实例
        return httpContentConverterFactory.Read<TResult>(httpResponseMessage,
            httpRequestBuilder.HttpContentConverterProviders?.SelectMany(u => u.Invoke()).ToArray(),
            httpContext.RequestAborted);
    }

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static Task<TResult?> ForwardAsAsync<TResult>(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<TResult>(httpContext, Helpers.ParseHttpMethod(httpContext?.Request.Method),
            string.IsNullOrWhiteSpace(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute), configure,
            completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static Task<TResult?> ForwardAsAsync<TResult>(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<TResult>(httpContext, httpMethod,
            string.IsNullOrWhiteSpace(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute), configure,
            completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static Task<TResult?> ForwardAsAsync<TResult>(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<TResult>(httpContext, Helpers.ParseHttpMethod(httpContext?.Request.Method), requestUri,
            configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    public static async Task<TResult?> ForwardAsAsync<TResult>(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(httpMethod);

        // 创建 HttpContextForwardBuilder 实例
        var httpContextForwardBuilder = CreateForwardBuilder(httpContext, httpMethod, requestUri, forwardOptions);

        // 构建 HttpRequestBuilder 实例
        var httpRequestBuilder = await httpContextForwardBuilder.BuildAsync(configure);

        // 获取 IHttpRemoteService 实例
        var httpRemoteService = httpContext.RequestServices.GetRequiredService<IHttpRemoteService>();

        // 获取 IHttpContentConverterFactory 实例
        var httpContentConverterFactory =
            httpContext.RequestServices.GetRequiredService<IHttpContentConverterFactory>();

        // 发送 HTTP 远程请求
        var httpResponseMessage =
            await httpRemoteService.SendAsync(httpRequestBuilder, completionOption, httpContext.RequestAborted);

        // 根据配置选项将 HttpResponseMessage 信息转发到 HttpContext 中
        ForwardResponseMessage(httpContext, httpResponseMessage, httpContextForwardBuilder.ForwardOptions);

        // 将 HttpResponseMessage 转换为 TResult 实例
        return await httpContentConverterFactory.ReadAsync<TResult>(httpResponseMessage,
            httpRequestBuilder.HttpContentConverterProviders?.SelectMany(u => u.Invoke()).ToArray(),
            httpContext.RequestAborted);
    }

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ForwardAsString(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<string>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ForwardAsString(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<string>(httpContext, httpMethod, requestUri,
        configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ForwardAsString(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<string>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ForwardAsString(this HttpContext? httpContext, HttpMethod httpMethod, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<string>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static Task<string?> ForwardAsStringAsync(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<string>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static Task<string?> ForwardAsStringAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<string>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static Task<string?> ForwardAsStringAsync(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<string>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static Task<string?> ForwardAsStringAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<string>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static byte[]? ForwardAsByteArray(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<byte[]>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static byte[]? ForwardAsByteArray(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<byte[]>(httpContext, httpMethod, requestUri,
        configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static byte[]? ForwardAsByteArray(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<byte[]>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static byte[]? ForwardAsByteArray(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<byte[]>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static Task<byte[]?> ForwardAsByteArrayAsync(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<byte[]>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static Task<byte[]?> ForwardAsByteArrayAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<byte[]>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static Task<byte[]?> ForwardAsByteArrayAsync(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<byte[]>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <c>byte[]</c>
    /// </returns>
    public static Task<byte[]?> ForwardAsByteArrayAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<byte[]>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Stream? ForwardAsStream(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<Stream>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Stream? ForwardAsStream(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<Stream>(httpContext, httpMethod, requestUri,
        configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Stream? ForwardAsStream(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<Stream>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Stream? ForwardAsStream(this HttpContext? httpContext, HttpMethod httpMethod, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<Stream>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Task<Stream?> ForwardAsStreamAsync(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<Stream>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Task<Stream?> ForwardAsStreamAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<Stream>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Task<Stream?> ForwardAsStreamAsync(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<Stream>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static Task<Stream?> ForwardAsStreamAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<Stream>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static IActionResult? ForwardAsActionResult(this HttpContext? httpContext, string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<IActionResult>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static IActionResult? ForwardAsActionResult(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) => ForwardAs<IActionResult>(httpContext, httpMethod,
        requestUri,
        configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static IActionResult? ForwardAsActionResult(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<IActionResult>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static IActionResult? ForwardAsActionResult(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAs<IActionResult>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static Task<IActionResult?> ForwardAsActionResultAsync(this HttpContext? httpContext,
        string? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<IActionResult>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static Task<IActionResult?> ForwardAsActionResultAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        string? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<IActionResult>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static Task<IActionResult?> ForwardAsActionResultAsync(this HttpContext? httpContext, Uri? requestUri = null,
        Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<IActionResult>(httpContext, requestUri, configure, completionOption, forwardOptions);

    /// <summary>
    ///     转发 <see cref="HttpContext" /> 到新的 HTTP 远程地址
    /// </summary>
    /// <param name="httpContext">
    ///     <see cref="HttpContext" />
    /// </param>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="configure">自定义配置委托</param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="forwardOptions">
    ///     <see cref="HttpContextForwardOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="IActionResult" />
    /// </returns>
    public static Task<IActionResult?> ForwardAsActionResultAsync(this HttpContext? httpContext, HttpMethod httpMethod,
        Uri? requestUri = null, Action<HttpRequestBuilder>? configure = null,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        HttpContextForwardOptions? forwardOptions = null) =>
        ForwardAsAsync<IActionResult>(httpContext, httpMethod, requestUri, configure, completionOption, forwardOptions);
}