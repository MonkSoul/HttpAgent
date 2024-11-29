// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求选项
/// </summary>
public sealed class HttpRemoteOptions
{
    /// <summary>
    ///     默认 JSON 序列化配置
    /// </summary>
    /// <remarks>参考文献：https://learn.microsoft.com/zh-cn/dotnet/standard/serialization/system-text-json/configure-options。</remarks>
    public static readonly JsonSerializerOptions JsonSerializerOptionsDefault = new(JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    ///     默认请求内容类型
    /// </summary>
    public string? DefaultContentType { get; set; } = Constants.TEXT_PLAIN_MIME_TYPE;

    /// <summary>
    ///     默认文件下载保存目录
    /// </summary>
    public string? DefaultFileDownloadDirectory { get; set; }

    /// <summary>
    ///     请求分析工具日志级别
    /// </summary>
    /// <remarks>默认值为 <see cref="LogLevel.Warning" /></remarks>
    public LogLevel ProfilerLogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    ///     JSON 序列化配置
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonSerializerOptionsDefault;

    /// <summary>
    ///     自定义 HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 集合提供器
    /// </summary>
    /// <value>返回多个包含实现 <see cref="IHttpDeclarativeExtractor" /> 集合的集合。</value>
    internal IReadOnlyList<Func<IEnumerable<IHttpDeclarativeExtractor>>>? HttpDeclarativeExtractors { get; set; }
}