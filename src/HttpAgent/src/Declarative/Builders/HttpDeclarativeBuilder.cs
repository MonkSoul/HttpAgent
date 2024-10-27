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
            throw new InvalidOperationException($"No `[HttpMethod]` annotation was found in method `{Method.Name}`.");
        }

        // 获取 HttpMethodAttribute 实例
        var httpMethodAttribute = Method.GetCustomAttribute<HttpMethodAttribute>(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethodAttribute);

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder =
            HttpRequestBuilder.Create(httpMethodAttribute.Method, httpMethodAttribute.RequestUri);

        // 解析被调用方法信息
        ExtractMethod(httpRequestBuilder, ExtractHttpClientName, ExtractTraceIdentifier, ExtractProfiler,
            ExtractSimulateBrowser, ExtractDisableCache, ExtractEnsureSuccessStatusCode, ExtractTimeout,
            ExtractQueryParameters, ExtractPathParameters, ExtractHeaders, ExtractBodyContent, ExtractMultipartContent,
            ExtractHttpRequestBuilderConfigure);

        return httpRequestBuilder;
    }

    /// <summary>
    ///     解析被调用方法信息
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="actions">解析动作集合</param>
    internal static void ExtractMethod(HttpRequestBuilder httpRequestBuilder,
        params Action<HttpRequestBuilder>[] actions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestBuilder);
        ArgumentNullException.ThrowIfNull(actions);

        // 逐条调用
        foreach (var action in actions)
        {
            action(httpRequestBuilder);
        }
    }

    /// <summary>
    ///     解析 <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractHttpClientName(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [HttpClientName] 特性
        if (!Method.IsDefined(typeof(HttpClientNameAttribute), true))
        {
            return;
        }

        httpRequestBuilder.SetHttpClientName(Method.GetCustomAttribute<HttpClientNameAttribute>(true)!.Name);
    }

    /// <summary>
    ///     解析跟踪标识
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractTraceIdentifier(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [TraceIdentifier] 特性
        if (!Method.IsDefined(typeof(TraceIdentifierAttribute), true))
        {
            return;
        }

        httpRequestBuilder.SetTraceIdentifier(Method.GetCustomAttribute<TraceIdentifierAttribute>(true)!.Identifier);
    }

    /// <summary>
    ///     解析请求分析工具
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractProfiler(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [Profiler] 特性
        if (!Method.IsDefined(typeof(ProfilerAttribute), true))
        {
            return;
        }

        httpRequestBuilder.Profiler(Method.GetCustomAttribute<ProfilerAttribute>(true)!.Enabled);
    }

    /// <summary>
    ///     解析模拟浏览器环境
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractSimulateBrowser(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [SimulateBrowser] 特性
        if (!Method.IsDefined(typeof(SimulateBrowserAttribute), true))
        {
            return;
        }

        httpRequestBuilder.SimulateBrowser();
    }

    /// <summary>
    ///     解析禁用 HTTP 缓存
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractDisableCache(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [DisableCache] 特性
        if (!Method.IsDefined(typeof(DisableCacheAttribute), true))
        {
            return;
        }

        httpRequestBuilder.DisableCache(Method.GetCustomAttribute<DisableCacheAttribute>(true)!.Disabled);
    }

    /// <summary>
    ///     解析如果 HTTP 响应的 IsSuccessStatusCode 属性是 <c>false</c>，则引发异常特性
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractEnsureSuccessStatusCode(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [EnsureSuccessStatusCode] 特性
        if (!Method.IsDefined(typeof(EnsureSuccessStatusCodeAttribute), true))
        {
            return;
        }

        httpRequestBuilder.EnsureSuccessStatusCode(Method.GetCustomAttribute<EnsureSuccessStatusCodeAttribute>(true)!
            .Enabled);
    }

    /// <summary>
    ///     解析超时时间
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractTimeout(HttpRequestBuilder httpRequestBuilder)
    {
        // 检查方法或接口是否定义了 [Timeout] 特性
        if (!Method.IsDefined(typeof(TimeoutAttribute), true))
        {
            return;
        }

        httpRequestBuilder.SetTimeout(Method.GetCustomAttribute<TimeoutAttribute>(true)!.Timeout);
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

        // 遍历查询参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in queryParameters)
        {
            // 获取 QueryAttribute 实例
            var queryAttribute = parameter.GetCustomAttribute<QueryAttribute>()!;

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 检查参数是否贴了 [AliasAs] 特性
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

    /// <summary>
    ///     解析路径参数
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractPathParameters(HttpRequestBuilder httpRequestBuilder)
    {
        // 查找所有符合的参数
        var pathParameters = Parameters.Where(u => !_specialArgumentTypes.Contains(u.Key.ParameterType)).ToArray();

        // 空检查
        if (pathParameters.Length == 0)
        {
            return;
        }

        // 遍历符合的参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in pathParameters)
        {
            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 检查参数类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
            if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
            {
                httpRequestBuilder.WithPathParameter(parameterName, value);

                continue;
            }

            // 空检查
            if (value is not null)
            {
                httpRequestBuilder.WithPathParameters(value, parameterName);
            }
        }
    }

    /// <summary>
    ///     解析请求标头
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractHeaders(HttpRequestBuilder httpRequestBuilder)
    {
        // ++++ 情况一：当为方法或接口添加 [Headers] 特性时 ++++

        // 获取 HeadersAttribute 特性集合
        var headersAttributes = Method.GetCustomAttributes<HeadersAttribute>(true).ToArray();

        // 空检查
        if (headersAttributes.Length > 0)
        {
            foreach (var headersAttribute in headersAttributes)
            {
                // 判断是否是添加请求标头的操作
                if (headersAttribute.HasSetValues)
                {
                    httpRequestBuilder.WithHeader(headersAttribute.Name, headersAttribute.Values);
                }
                // 移除请求标头
                else
                {
                    httpRequestBuilder.RemoveHeaders(headersAttribute.Name);
                }
            }
        }

        // ++++ 情况二：当为参数添加 [Headers] 特性时 ++++

        // 查找所有贴有 [Headers] 特性的参数
        var headersParameters = Parameters.Where(u =>
                !_specialArgumentTypes.Contains(u.Key.ParameterType) && u.Key.IsDefined(typeof(HeadersAttribute)))
            .ToArray();

        // 空检查
        if (headersParameters.Length == 0)
        {
            return;
        }

        // 遍历请求标头参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in headersParameters)
        {
            // 获取 HeadersAttribute 特性集合
            var parameterHeadersAttributes = parameter.GetCustomAttributes<HeadersAttribute>()!;

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历 HeadersAttribute 特性集合
            foreach (var headersAttribute in parameterHeadersAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(headersAttribute.AliasAs)
                        ? parameterName
                        : headersAttribute.AliasAs.Trim();
                }

                httpRequestBuilder.WithHeader(parameterName, value ?? headersAttribute.Values);
            }
        }
    }

    /// <summary>
    ///     解析请求内容参数
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractBodyContent(HttpRequestBuilder httpRequestBuilder)
    {
        // 查找所有贴有 [Body] 特性的参数
        var bodyParameters = Parameters.Where(u =>
            !_specialArgumentTypes.Contains(u.Key.ParameterType) && u.Key.IsDefined(typeof(BodyAttribute))).ToArray();

        // 空检查
        if (bodyParameters.Length == 0)
        {
            return;
        }

        // 获取首个贴有 [Body] 特性的参数
        var firstBodyParameter = bodyParameters.First();

        // 获取 BodyAttribute 实例
        var bodyAttribute = firstBodyParameter.Key.GetCustomAttribute<BodyAttribute>()!;

        // 设置原始请求内容
        httpRequestBuilder.SetRawContent(firstBodyParameter.Value, bodyAttribute.ContentType);

        // 设置内容编码
        if (!string.IsNullOrWhiteSpace(bodyAttribute.ContentEncoding))
        {
            httpRequestBuilder.SetContentEncoding(bodyAttribute.ContentEncoding);
        }
    }

    /// <summary>
    ///     解析多部分表单内容
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractMultipartContent(HttpRequestBuilder httpRequestBuilder)
    {
        // 尝试解析 Action<HttpMultipartFormDataBuilder> 参数
        if (Args.FirstOrDefault(u => u is Action<HttpMultipartFormDataBuilder>) is Action<HttpMultipartFormDataBuilder>
            multipartContentBuilderAction)
        {
            httpRequestBuilder.SetMultipartContent(multipartContentBuilderAction);
        }
    }

    /// <summary>
    ///     解析 <inheritdoc cref="HttpRequestBuilder" /> 自定义配置
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal void ExtractHttpRequestBuilderConfigure(HttpRequestBuilder httpRequestBuilder)
    {
        // 尝试解析 Action<HttpRequestBuilder> 参数
        var configure = Args.FirstOrDefault(u => u is Action<HttpRequestBuilder>) as Action<HttpRequestBuilder>;

        configure?.Invoke(httpRequestBuilder);
    }
}