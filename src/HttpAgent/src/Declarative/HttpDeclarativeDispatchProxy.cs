// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求声明式代理类
/// </summary>
public class HttpDeclarativeDispatchProxy : DispatchProxyAsync
{
    /// <inheritdoc cref="IHttpRemoteService" />
    internal IHttpRemoteService RemoteService { get; set; } = null!;

    /// <inheritdoc cref="IServiceProvider" />
    internal IServiceProvider ServiceProvider { get; set; } = null!;

    /// <inheritdoc />
    public override object Invoke(MethodInfo method, object[] args)
    {
        // 检查方法是否贴有 [HttpMethod] 特性
        if (!method.IsDefined(typeof(HttpMethodAttribute), true))
        {
            return null!;
        }

        // 解析特殊类型参数实例
        var (completionOption, cancellationToken) = ExtractSpecialArguments(args);

        // 获取调用方法返回值
        var returnType = method.ReturnType == typeof(void) ? typeof(DoesNoReceiveContent) : method.ReturnType;

        // 发送 HTTP 远程请求
        return RemoteService.SendAs(returnType, HttpRequestBuilder.Declarative(method, args).Build(),
            completionOption, cancellationToken)!;
    }

    /// <inheritdoc />
    public override async Task InvokeAsync(MethodInfo method, object[] args)
    {
        // 检查方法是否贴有 [HttpMethod] 特性
        if (!method.IsDefined(typeof(HttpMethodAttribute), true))
        {
            return;
        }

        // 解析特殊类型参数实例
        var (completionOption, cancellationToken) = ExtractSpecialArguments(args);

        // 发送 HTTP 远程请求
        _ = await RemoteService.SendAsAsync<DoesNoReceiveContent>(HttpRequestBuilder.Declarative(method, args).Build(),
            completionOption, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
    {
        // 检查方法是否贴有 [HttpMethod] 特性
        if (!method.IsDefined(typeof(HttpMethodAttribute), true))
        {
            return default!;
        }

        // 解析特殊类型参数实例
        var (completionOption, cancellationToken) = ExtractSpecialArguments(args);

        // 发送 HTTP 远程请求
        var result = await RemoteService.SendAsAsync<T>(HttpRequestBuilder.Declarative(method, args).Build(),
            completionOption, cancellationToken);

        return result!;
    }

    /// <summary>
    ///     解析特殊类型参数实例
    /// </summary>
    /// <param name="args">调用方法的参数数组</param>
    /// <returns>
    ///     <see cref="Tuple{T1, T2}" />
    /// </returns>
    internal static (HttpCompletionOption CompletionOption, CancellationToken CancellationToken)
        ExtractSpecialArguments(object[] args)
    {
        // 尝试解析 HttpCompletionOption 参数
        var completionOption = args.FirstOrDefault(u => u is HttpCompletionOption) as HttpCompletionOption? ??
                               HttpCompletionOption.ResponseContentRead;

        // 尝试解析 CancellationToken
        var cancellationToken = args.FirstOrDefault(u => u is CancellationToken) as CancellationToken? ??
                                CancellationToken.None;

        return (completionOption, cancellationToken);
    }
}