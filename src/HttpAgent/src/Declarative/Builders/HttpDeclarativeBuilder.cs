// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求声明式构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.Declarative(method, args)</c> 静态方法创建。</remarks>
public sealed class HttpDeclarativeBuilder
{
    /// <summary>
    ///     特殊参数类型
    /// </summary>
    internal static Type[] _specialArgumentTypes =
    [
        typeof(Action<HttpRequestBuilder>), typeof(Action<HttpMultipartFormDataBuilder>), typeof(HttpCompletionOption),
        typeof(CancellationToken)
    ];

    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeBuilder" />
    /// </summary>
    /// <param name="method">调用方法</param>
    /// <param name="args">调用方法的参数数组</param>
    internal HttpDeclarativeBuilder(MethodInfo method, object?[] args)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);

        Method = method;
        Args = args;
    }

    /// <summary>
    ///     调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     调用方法的参数数组
    /// </summary>
    public object?[] Args { get; }

    /// <summary>
    ///     构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    internal HttpRequestBuilder Build()
    {
        // 获取 HttpMethodAttribute 实例
        var httpMethodAttribute = Method.GetCustomAttribute<HttpMethodAttribute>(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethodAttribute);

        // 尝试解析 Action<HttpRequestBuilder> 参数
        var configure = Args.FirstOrDefault(u => u is Action<HttpRequestBuilder>) as Action<HttpRequestBuilder>;

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder =
            HttpRequestBuilder.Create(httpMethodAttribute.Method, httpMethodAttribute.RequestUri, configure);

        // TODO: 查询参数，路径参数

        // 尝试解析 Action<HttpMultipartFormDataBuilder> 参数
        if (Args.FirstOrDefault(u => u is Action<HttpMultipartFormDataBuilder>) is Action<HttpMultipartFormDataBuilder>
            multipartContentBuilderAction)
        {
            httpRequestBuilder.SetMultipartContent(multipartContentBuilderAction);
        }

        // 检查方法或接口是否定义了 [Profiler] 特性
        if (Method.IsDefined(typeof(ProfilerAttribute), true))
        {
            httpRequestBuilder.Profiler();
        }

        // 检查方法或接口是否定义了 [DisableCache] 特性
        if (Method.IsDefined(typeof(DisableCacheAttribute), true))
        {
            httpRequestBuilder.DisableCache(Method.GetCustomAttribute<DisableCacheAttribute>()!.Disabled);
        }

        return httpRequestBuilder;
    }
}