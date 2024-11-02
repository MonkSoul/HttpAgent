// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式远程请求代理类
/// </summary>
public class HttpDeclarativeDispatchProxy : DispatchProxyAsync
{
    /// <inheritdoc cref="IHttpRemoteService" />
    public IHttpRemoteService RemoteService { get; internal set; } = null!;

    /// <inheritdoc />
    public override object Invoke(MethodInfo method, object[] args) => RemoteService.Declarative(method, args)!;

    /// <inheritdoc />
    public override async Task InvokeAsync(MethodInfo method, object[] args) =>
        _ = await InvokeAsyncT<DoesNoReceiveContent>(method, args);

    /// <inheritdoc />
    public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args) =>
        (await RemoteService.DeclarativeAsync<T>(method, args))!;
}