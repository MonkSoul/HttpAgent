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
    ///     默认请求内容类型
    /// </summary>
    public string? DefaultContentType { get; internal init; } = Constants.DEFAULT_CONTENT_TYPE;

    /// <summary>
    ///     默认文件下载保存目录
    /// </summary>
    public string? DefaultFileDownloadDirectory { get; internal init; }

    /// <summary>
    ///     自定义 HTTP 声明式 <see cref="IHttpDeclarativeExtractor" /> 集合提供器
    /// </summary>
    /// <value>返回多个包含实现 <see cref="IHttpDeclarativeExtractor" /> 集合的集合。</value>
    public IList<Func<IEnumerable<IHttpDeclarativeExtractor>>>? HttpDeclarativeExtractors { get; init; }
}