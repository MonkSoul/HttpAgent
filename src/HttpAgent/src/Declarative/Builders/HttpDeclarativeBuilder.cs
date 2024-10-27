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
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    internal HttpDeclarativeBuilder(MethodInfo method, object?[] args)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);

        Method = method;
        Args = args;

        // 初始化被调用方法的参数键值字典
        Parameters = method.GetParameters().Select((p, i) => new { p, v = args[i] }).ToDictionary(u => u.p, u => u.v);
    }

    /// <summary>
    ///     被调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     被调用方法的参数值数组
    /// </summary>
    public object?[] Args { get; }

    /// <summary>
    ///     被调用方法的参数键值字典
    /// </summary>
    internal Dictionary<ParameterInfo, object?> Parameters { get; }

    /// <summary>
    ///     构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal HttpRequestBuilder Build()
    {
        // 检查被调用方法是否贴有 [HttpMethod] 特性
        if (!Method.IsDefined(typeof(HttpMethodAttribute), true))
        {
            throw new InvalidOperationException($"Method {Method.Name} does not have a HttpMethodAttribute.");
        }

        // 获取 HttpMethodAttribute 实例
        var httpMethodAttribute = Method.GetCustomAttribute<HttpMethodAttribute>(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethodAttribute);

        // 尝试解析 Action<HttpRequestBuilder> 参数
        var configure = Args.FirstOrDefault(u => u is Action<HttpRequestBuilder>) as Action<HttpRequestBuilder>;

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder =
            HttpRequestBuilder.Create(httpMethodAttribute.Method, httpMethodAttribute.RequestUri, configure);

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

        // 解析查询参数
        ExtractQueryParameters(httpRequestBuilder);

        // TODO: 路径参数，Body 参数

        // TODO：处理请求头（移除请求头）

        // TODO：处理超时

        return httpRequestBuilder;
    }

    /// <summary>
    ///     解析查询参数
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractQueryParameters(HttpRequestBuilder httpRequestBuilder)
    {
        // 查找所有贴有 [Query] 特性的参数
        var queryParameters = Parameters.Where(u =>
            !_specialArgumentTypes.Contains(u.Key.ParameterType) && u.Key.IsDefined(typeof(QueryAttribute))).ToArray();

        // 空检查
        if (queryParameters.Length == 0)
        {
            return;
        }

        foreach (var (parameter, value) in queryParameters)
        {
            // 获取 QueryAttribute 实例
            var queryAttribute = parameter.GetCustomAttribute<QueryAttribute>()!;

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);
            if (!aliasAsDefined)
            {
                parameterName = string.IsNullOrWhiteSpace(queryAttribute.AliasAs)
                    ? parameterName
                    : queryAttribute.AliasAs.Trim();
            }

            // 检查参数类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
            if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
            {
                httpRequestBuilder.WithQueryParameter(parameterName, value, queryAttribute.Escape);

                continue;
            }

            // 空检查
            if (value is not null)
            {
                httpRequestBuilder.WithQueryParameters(value, queryAttribute.Prefix, queryAttribute.Escape);
            }
        }
    }
}