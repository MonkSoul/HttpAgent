// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式远程请求构建器
/// </summary>
/// <remarks>使用 <c>HttpRequestBuilder.Declarative(method, args)</c> 静态方法创建。</remarks>
public sealed class HttpDeclarativeBuilder
{
    /// <summary>
    ///     HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 提取器集合
    /// </summary>
    internal static readonly Dictionary<Type, IHttpDeclarativeExtractor> _extractors = new()
    {
        { typeof(ValidationDeclarativeExtractor), new ValidationDeclarativeExtractor() },
        { typeof(HttpClientNameDeclarativeExtractor), new HttpClientNameDeclarativeExtractor() },
        { typeof(TraceIdentifierDeclarativeExtractor), new TraceIdentifierDeclarativeExtractor() },
        { typeof(ProfilerDeclarativeExtractor), new ProfilerDeclarativeExtractor() },
        { typeof(SimulateBrowserDeclarativeExtractor), new SimulateBrowserDeclarativeExtractor() },
        { typeof(AcceptLanguageDeclarativeExtractor), new AcceptLanguageDeclarativeExtractor() },
        { typeof(DisableCacheDeclarativeExtractor), new DisableCacheDeclarativeExtractor() },
        { typeof(EnsureSuccessStatusCodeDeclarativeExtractor), new EnsureSuccessStatusCodeDeclarativeExtractor() },
        { typeof(TimeoutDeclarativeExtractor), new TimeoutDeclarativeExtractor() },
        { typeof(QueryDeclarativeExtractor), new QueryDeclarativeExtractor() },
        { typeof(PathDeclarativeExtractor), new PathDeclarativeExtractor() },
        { typeof(CookieDeclarativeExtractor), new CookieDeclarativeExtractor() },
        { typeof(HeaderDeclarativeExtractor), new HeaderDeclarativeExtractor() },
        { typeof(BodyDeclarativeExtractor), new BodyDeclarativeExtractor() },
        { typeof(MultipartBodyDeclarativeExtractor), new MultipartBodyDeclarativeExtractor() },
        { typeof(HttpRequestBuilderDeclarativeExtractor), new HttpRequestBuilderDeclarativeExtractor() }
    };

    /// <summary>
    ///     标识是否已加载自定义 HTTP 声明式提取器
    /// </summary>
    internal bool _hasLoadedExtractors;

    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeBuilder" />
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    internal HttpDeclarativeBuilder(MethodInfo method, object?[] args)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(args);

        Method = method;
        Args = args;
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
    ///     构建 <see cref="HttpRequestBuilder" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpRequestBuilder" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal HttpRequestBuilder Build(HttpRemoteOptions httpRemoteOptions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);

        // 检查被调用方法是否贴有 [HttpMethod] 特性
        if (!Method.IsDefined(typeof(HttpMethodAttribute), true))
        {
            throw new InvalidOperationException($"No `[HttpMethod]` annotation was found in method `{Method.Name}`.");
        }

        // 获取 HttpMethodAttribute 实例
        var httpMethodAttribute = Method.GetCustomAttribute<HttpMethodAttribute>(true)!;

        // 初始化 HttpRequestBuilder 实例
        var httpRequestBuilder =
            HttpRequestBuilder.Create(httpMethodAttribute.Method, httpMethodAttribute.RequestUri);

        // 初始化 HttpDeclarativeExtractorContext 实例
        var httpDeclarativeExtractorContext = new HttpDeclarativeExtractorContext(Method, Args);

        // 检查是否已加载自定义 HTTP 声明式提取器
        if (!_hasLoadedExtractors)
        {
            _hasLoadedExtractors = true;

            // 添加自定义 IHttpDeclarativeExtractor 数组
            _extractors.TryAdd(httpRemoteOptions.HttpDeclarativeExtractors?.SelectMany(u => u.Invoke()).ToArray(),
                value => value.GetType());
        }

        // 遍历 HTTP 声明式提取器集合
        foreach (var extractor in _extractors.Values)
        {
            // 提取方法信息构建 HttpRequestBuilder 实例
            extractor.Extract(httpRequestBuilder, httpDeclarativeExtractorContext);
        }

        return httpRequestBuilder;
    }
}