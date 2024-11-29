// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Extensions;

/// <summary>
///     HTTP 远程服务拓展类
/// </summary>
public static class HttpRemoteExtensions
{
    /// <summary>
    ///     添加 HTTP 远程请求分析工具处理委托
    /// </summary>
    /// <param name="builder">
    ///     <see cref="IHttpClientBuilder" />
    /// </param>
    /// <param name="disableInProduction">是否在生产环境中禁用。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    public static IHttpClientBuilder AddProfilerDelegatingHandler(this IHttpClientBuilder builder,
        bool disableInProduction = false)
    {
        // 获取 IServiceCollection 实例
        var services = builder.Services;

        // 注册请求分析工具服务
        services.TryAddTransient<ProfilerDelegatingHandler>();

        // 检查是否在生产环境中禁用
        if (disableInProduction && GetHostEnvironmentName(services)?.ToLower() == "production")
        {
            return builder;
        }

        return builder.AddHttpMessageHandler<ProfilerDelegatingHandler>();
    }

    /// <summary>
    ///     分析 <see cref="HttpRequestMessage" /> 标头
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="requestSummary">请求标头摘要</param>
    /// <param name="contentSummary">请求内容标头摘要</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ProfilerHeaders(this HttpRequestMessage httpRequestMessage,
        string? requestSummary = "Request Headers", string? contentSummary = "Content Headers")
    {
        // 格式化请求条目
        var requestEntry = StringUtility.FormatKeyValuesSummary(httpRequestMessage.Headers, requestSummary);

        // 获取 HttpContent 实例
        var httpContent = httpRequestMessage.Content;

        // 空检查
        if (httpContent is null)
        {
            return requestEntry;
        }

        // 获取请求内容集合
        var httpContentCollection = httpContent as MultipartContent ?? [httpContent];

        // 初始化 StringBuilder 实例用于构建请求内容标头条目
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"{contentSummary} ({httpContent?.GetType().Name}): ");

        var count = httpContentCollection.Count();
        for (var i = 0; i < count; i++)
        {
            // 获取当前请求内容
            var content = httpContentCollection.ElementAt(i);

            // 获取内容（表单名）摘要
            var nameSummary = string.IsNullOrWhiteSpace(content.Headers.ContentDisposition?.Name)
                ? null
                : $"\t[{content.Headers.ContentDisposition?.Name}]";

            // 格式化请求内容标头条目
            var contentHeaderEntry =
                StringUtility.FormatKeyValuesSummary(content.Headers.ToDictionary(u => $"  {u.Key}", u => u.Value),
                    nameSummary);

            // 处理最后一行换行问题
            if (i == count - 1)
            {
                stringBuilder.Append(contentHeaderEntry);
            }
            else
            {
                stringBuilder.AppendLine(contentHeaderEntry);
            }
        }

        return $"{requestEntry}\r\n{stringBuilder}";
    }

    /// <summary>
    ///     分析 <see cref="HttpResponseMessage" /> 标头
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="summary">摘要</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? ProfilerHeaders(this HttpResponseMessage httpResponseMessage,
        string? summary = "Response Headers") =>
        StringUtility.FormatKeyValuesSummary(
            httpResponseMessage.Headers.ConcatIgnoreNull(httpResponseMessage.Content.Headers),
            summary);

    /// <summary>
    ///     分析常规和 <see cref="HttpResponseMessage" /> 标头
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="responseSummary">响应标头摘要</param>
    /// <param name="generalSummary">常规摘要</param>
    /// <param name="generalCustomKeyValues">自定义常规摘要键值集合</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string ProfilerGeneralAndHeaders(this HttpResponseMessage httpResponseMessage,
        string? responseSummary = "Response Headers", string? generalSummary = "General",
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? generalCustomKeyValues = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 获取 HttpRequestMessage 实例
        var httpRequestMessage = httpResponseMessage.RequestMessage;

        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestMessage);

        // 获取 HttpContent 实例
        var httpContent = httpRequestMessage.Content;

        // 格式化 HTTP 声明式条目
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? declarativeKeyValues =
            httpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>(Constants.DECLARATIVE_METHOD_KEY),
                out var methodSignature)
                ? [new KeyValuePair<string, IEnumerable<string>>("Declarative", [methodSignature])]
                : null;

        // 格式化常规条目
        var generalEntry = StringUtility.FormatKeyValuesSummary(new[]
        {
            new KeyValuePair<string, IEnumerable<string>>("Request URL",
                [httpRequestMessage.RequestUri?.OriginalString!]),
            new KeyValuePair<string, IEnumerable<string>>("HTTP Method", [httpRequestMessage.Method.ToString()]),
            new KeyValuePair<string, IEnumerable<string>>("Status Code",
                [$"{(int)httpResponseMessage.StatusCode} {httpResponseMessage.StatusCode}"]),
            new KeyValuePair<string, IEnumerable<string>>("HTTP Content",
                [$"{httpContent?.GetType().Name}"])
        }.ConcatIgnoreNull(declarativeKeyValues).ConcatIgnoreNull(generalCustomKeyValues), generalSummary);

        // 格式化响应条目
        var responseEntry = httpResponseMessage.ProfilerHeaders(responseSummary);

        return $"{generalEntry}\r\n{responseEntry}";
    }

    /// <summary>
    ///     获取主机环境名
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? GetHostEnvironmentName(IServiceCollection services)
    {
        // 获取主机环境对象
        var hostEnvironment = services
            .FirstOrDefault(u => u.ServiceType.FullName == "Microsoft.Extensions.Hosting.IHostEnvironment")
            ?.ImplementationInstance;

        // 空检查
        return hostEnvironment is null
            ? null
            : Convert.ToString(hostEnvironment.GetType().GetProperty("EnvironmentName")?.GetValue(hostEnvironment));
    }
}