// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRequestMessage" /> 构建器
/// </summary>
public sealed partial class HttpRequestBuilder
{
    /// <summary>
    ///     <see cref="StringContentForFormUrlEncodedContentProcessor" /> 实例
    /// </summary>
    internal static readonly Lazy<StringContentForFormUrlEncodedContentProcessor>
        _stringContentForFormUrlEncodedContentProcessorInstance =
            new(() => new StringContentForFormUrlEncodedContentProcessor());

    /// <summary>
    ///     <inheritdoc cref="HttpRequestBuilder" />
    /// </summary>
    /// <param name="httpMethod">请求方式</param>
    /// <param name="requestUri">请求地址</param>
    internal HttpRequestBuilder(HttpMethod httpMethod, Uri? requestUri)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpMethod);

        Method = httpMethod;
        RequestUri = requestUri;
    }

    /// <summary>
    ///     构建 <see cref="HttpRequestMessage" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="baseUri">基地址</param>
    /// <returns>
    ///     <see cref="HttpRequestMessage" />
    /// </returns>
    internal HttpRequestMessage Build(HttpRemoteOptions httpRemoteOptions,
        IHttpContentProcessorFactory httpContentProcessorFactory, Uri? baseUri)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);
        ArgumentNullException.ThrowIfNull(httpContentProcessorFactory);
        ArgumentNullException.ThrowIfNull(Method);

        // 构建最终的请求地址
        var finalRequestUri = BuildFinalRequestUri(baseUri);

        // 初始化 HttpRequestMessage 实例
        var httpRequestMessage = new HttpRequestMessage(Method, finalRequestUri);

        // 追加请求标头
        AppendHeaders(httpRequestMessage);

        // 追加 Cookies
        AppendCookies(httpRequestMessage);

        // 移除 Cookies
        RemoveCookies(httpRequestMessage);

        // 移除请求标头
        RemoveHeaders(httpRequestMessage);

        // 构建并设置指定的 HttpRequestMessage 请求消息的内容
        BuildAndSetContent(httpRequestMessage, httpContentProcessorFactory, httpRemoteOptions);

        // 追加 HTTP 请求的属性集
        AppendProperties(httpRequestMessage);

        return httpRequestMessage;
    }

    /// <summary>
    ///     构建最终的请求地址
    /// </summary>
    /// <param name="baseUri">基地址</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string BuildFinalRequestUri(Uri? baseUri)
    {
        // 替换路径参数，处理非标准 HTTP URI 的应用场景（如 {url}），此时需优先解决路径参数问题
        var newRequestUri = RequestUri is null or { OriginalString: null }
            ? RequestUri
            : new Uri(ReplacePathPlaceholders(RequestUri.OriginalString), UriKind.RelativeOrAbsolute);

        // 初始化 UriBuilder 实例
        var uriBuilder = new UriBuilder(baseUri is null ? newRequestUri! : new Uri(baseUri, newRequestUri!));

        // 追加片段标识符
        AppendFragment(uriBuilder);

        // 追加查询参数
        AppendQueryParameters(uriBuilder);

        // 替换路径参数
        var finalRequestUri = ReplacePathPlaceholders(uriBuilder.Uri.ToString());

        return finalRequestUri;
    }

    /// <summary>
    ///     追加片段标识符
    /// </summary>
    /// <param name="uriBuilder">
    ///     <see cref="UriBuilder" />
    /// </param>
    internal void AppendFragment(UriBuilder uriBuilder)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(Fragment))
        {
            return;
        }

        uriBuilder.Fragment = Fragment;
    }

    /// <summary>
    ///     追加查询参数
    /// </summary>
    /// <param name="uriBuilder">
    ///     <see cref="UriBuilder" />
    /// </param>
    internal void AppendQueryParameters(UriBuilder uriBuilder)
    {
        // 空检查
        if (QueryParameters.IsNullOrEmpty())
        {
            return;
        }

        // 解析 URL 中的查询字符串为键值对列表
        var queryParameters = uriBuilder.Query.ParseUrlQueryParameters();

        // 追加查询参数
        foreach (var (key, values) in QueryParameters)
        {
            queryParameters.AddRange(values.Select(value =>
                new KeyValuePair<string, string?>(key, value)));
        }

        // 构建查询字符串赋值给 UriBuilder 的 Query 属性
        uriBuilder.Query =
            "?" + string.Join('&',
                queryParameters
                    // 过滤已标记为移除的查询参数
                    .WhereIf(QueryParametersToRemove is { Count: > 0 },
                        u => QueryParametersToRemove?.TryGetValue(u.Key, out _) == false)
                    .Select(u => $"{u.Key}={u.Value}"));
    }

    /// <summary>
    ///     替换路径参数
    /// </summary>
    /// <param name="originalUri">源请求地址</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string ReplacePathPlaceholders(string originalUri)
    {
        var newUri = originalUri;

        // 空检查
        if (!PathParameters.IsNullOrEmpty())
        {
            newUri = newUri.ReplacePlaceholders(PathParameters);
        }

        // 空检查
        if (!ObjectPathParameters.IsNullOrEmpty())
        {
            newUri = ObjectPathParameters.Aggregate(newUri,
                (current, objectPathParameter) =>
                    current.ReplacePlaceholders(objectPathParameter.Value, objectPathParameter.Key));
        }

        return newUri!;
    }

    /// <summary>
    ///     追加请求标头
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal void AppendHeaders(HttpRequestMessage httpRequestMessage)
    {
        // 添加跟踪标识
        if (!string.IsNullOrWhiteSpace(TraceIdentifier))
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(Constants.X_TRACE_ID_HEADER, TraceIdentifier);
        }

        // 设置身份验证凭据请求授权标头
        if (AuthenticationHeader is not null)
        {
            httpRequestMessage.Headers.Authorization = AuthenticationHeader;
        }

        // 设置禁用 HTTP 缓存
        if (DisableCacheEnabled)
        {
            httpRequestMessage.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true, NoStore = true, MustRevalidate = true
            };
        }

        // 空检查
        if (Headers.IsNullOrEmpty())
        {
            return;
        }

        // 遍历请求标头集合并追加到 HttpRequestMessage.Headers 中
        foreach (var (key, values) in Headers)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(key, values);
        }
    }

    /// <summary>
    ///     移除请求标头
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal void RemoveHeaders(HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (HeadersToRemove.IsNullOrEmpty())
        {
            return;
        }

        // 遍历请求标头集合并从 HttpRequestMessage.Headers 中移除
        foreach (var headerName in HeadersToRemove)
        {
            httpRequestMessage.Headers.Remove(headerName);
        }
    }

    /// <summary>
    ///     追加 Cookies
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal void AppendCookies(HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (Cookies.IsNullOrEmpty())
        {
            return;
        }

        httpRequestMessage.Headers.TryAddWithoutValidation(HeaderNames.Cookie,
            string.Join("; ", Cookies.Select(u => $"{u.Key}={u.Value.EscapeDataString(true)}")));
    }

    /// <summary>
    ///     移除 Cookies
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal void RemoveCookies(HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (CookiesToRemove.IsNullOrEmpty())
        {
            return;
        }

        // 获取已经设置的 Cookies
        if (!httpRequestMessage.Headers.TryGetValues(HeaderNames.Cookie, out var cookies))
        {
            return;
        }

        // 解析 Cookies 标头值
        var cookieList = CookieHeaderValue.ParseList(cookies.ToList());

        // 空检查
        if (cookieList.Count == 0)
        {
            return;
        }

        // 重新设置 Cookies
        httpRequestMessage.Headers.Remove(HeaderNames.Cookie);
        httpRequestMessage.Headers.TryAddWithoutValidation(HeaderNames.Cookie,
            // 过滤已标记为移除的 Cookie 键
            string.Join("; ", cookieList.WhereIf(CookiesToRemove is { Count: > 0 },
                    u => CookiesToRemove?.TryGetValue(u.Name.ToString(), out _) == false)
                .Select(u => $"{u.Name}={u.Value}")));
    }

    /// <summary>
    ///     构建并设置指定的 <see cref="HttpRequestMessage" /> 请求消息的内容
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    internal void BuildAndSetContent(HttpRequestMessage httpRequestMessage,
        IHttpContentProcessorFactory httpContentProcessorFactory, HttpRemoteOptions httpRemoteOptions)
    {
        // 获取自定义的 IHttpContentProcessor 集合
        var processors = HttpContentProcessorProviders?.SelectMany(u => u.Invoke()).ToArray();

        // 构建 MultipartFormDataContent 请求内容
        if (MultipartFormDataBuilder is not null)
        {
            ContentType = MediaTypeNames.Multipart.FormData;
            RawContent = MultipartFormDataBuilder.Build(httpRemoteOptions, httpContentProcessorFactory, processors);
        }

        // 检查是否设置了内容
        if (RawContent is null)
        {
            return;
        }

        // 设置默认的内容类型
        SetDefaultContentType(httpRemoteOptions.DefaultContentType);

        // 构建 HttpContent 实例
        var httpContent = httpContentProcessorFactory.Build(RawContent, ContentType!, ContentEncoding, processors);

        // 调用用于处理在设置请求消息的内容时的操作
        OnPreSetContent?.Invoke(httpContent);

        // 设置 HttpRequestMessage 请求消息的内容
        httpRequestMessage.Content = httpContent;
    }

    /// <summary>
    ///     追加 HTTP 请求的属性集
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal void AppendProperties(HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (Properties.Count > 0)
        {
            // 注意：httpRequestMessage.Properties 已过时，使用 Options 替代
            httpRequestMessage.Options.TryAdd(Properties);
        }

        // 检查是否禁用全局请求分析工具
        if (__Disabled_Profiler__)
        {
            httpRequestMessage.Options.AddOrUpdate(Constants.DISABLED_PROFILER_KEY, "TRUE");
        }
    }

    /// <summary>
    ///     设置默认的内容类型
    /// </summary>
    /// <param name="defaultContentType">默认请求内容类型</param>
    internal void SetDefaultContentType(string? defaultContentType)
    {
        // 空检查
        if (!string.IsNullOrWhiteSpace(ContentType))
        {
            return;
        }

        ContentType = RawContent switch
        {
            JsonContent => MediaTypeNames.Application.Json,
            FormUrlEncodedContent => MediaTypeNames.Application.FormUrlEncoded,
            (byte[] or Stream or ByteArrayContent or StreamContent or ReadOnlyMemoryContent or ReadOnlyMemory<byte>)
                and not StringContent => MediaTypeNames.Application
                    .Octet,
            MultipartContent => MediaTypeNames.Multipart.FormData,
            _ => defaultContentType ?? Constants.DEFAULT_CONTENT_TYPE
        };
    }
}