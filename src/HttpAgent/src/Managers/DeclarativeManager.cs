// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式管理器
/// </summary>
internal sealed class DeclarativeManager
{
    /// <inheritdoc cref="HttpDeclarativeBuilder" />
    internal readonly HttpDeclarativeBuilder _httpDeclarativeBuilder;

    /// <inheritdoc cref="IHttpRemoteService" />
    internal readonly IHttpRemoteService _httpRemoteService;

    /// <summary>
    ///     <inheritdoc cref="DeclarativeManager" />
    /// </summary>
    /// <param name="httpRemoteService">
    ///     <see cref="IHttpRemoteService" />
    /// </param>
    /// <param name="httpDeclarativeBuilder">
    ///     <see cref="HttpDeclarativeBuilder" />
    /// </param>
    internal DeclarativeManager(IHttpRemoteService httpRemoteService, HttpDeclarativeBuilder httpDeclarativeBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteService);
        ArgumentNullException.ThrowIfNull(httpDeclarativeBuilder);

        _httpRemoteService = httpRemoteService;
        _httpDeclarativeBuilder = httpDeclarativeBuilder;

        // 构建 HttpRequestBuilder 实例
        RequestBuilder = httpDeclarativeBuilder.Build(httpRemoteService.RemoteOptions);
    }

    /// <summary>
    ///     <inheritdoc cref="HttpRequestBuilder" />
    /// </summary>
    internal HttpRequestBuilder RequestBuilder { get; }

    /// <summary>
    ///     开始请求
    /// </summary>
    internal object? Start()
    {
        // 解析特殊类型参数实例
        var (completionOption, cancellationToken) = ExtractSpecialArguments(_httpDeclarativeBuilder.Args);

        // 获取被调用方法返回值类型
        var method = _httpDeclarativeBuilder.Method;
        var returnType = method.ReturnType == typeof(void) ? typeof(DoesNoReceiveContent) : method.ReturnType;

        // 发送 HTTP 远程请求
        return _httpRemoteService.SendAs(returnType, RequestBuilder, completionOption, cancellationToken);
    }

    /// <summary>
    ///     开始请求
    /// </summary>
    internal async Task<T?> StartAsync<T>()
    {
        // 解析特殊类型参数实例
        var (completionOption, cancellationToken) = ExtractSpecialArguments(_httpDeclarativeBuilder.Args);

        // 发送 HTTP 远程请求
        return await _httpRemoteService.SendAsAsync<T>(RequestBuilder, completionOption, cancellationToken);
    }

    /// <summary>
    ///     解析特殊类型参数实例
    /// </summary>
    /// <param name="args">被调用方法的参数值数组</param>
    /// <returns>
    ///     <see cref="Tuple{T1, T2}" />
    /// </returns>
    internal static (HttpCompletionOption CompletionOption, CancellationToken CancellationToken)
        ExtractSpecialArguments(object?[] args)
    {
        // 尝试解析单个 HttpCompletionOption 参数
        var completionOption = args.SingleOrDefault(u => u is HttpCompletionOption) as HttpCompletionOption?;

        // 尝试解析单个 CancellationToken
        var cancellationToken = args.SingleOrDefault(u => u is CancellationToken) as CancellationToken?;

        return (completionOption ?? HttpCompletionOption.ResponseContentRead,
            cancellationToken ?? CancellationToken.None);
    }
}