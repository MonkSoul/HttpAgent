// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpContext" /> 转发配置选项
/// </summary>
public sealed class HttpContextForwardOptions
{
    /// <summary>
    ///     是否转发请求标头
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool WithRequestHeaders { get; set; } = true;

    /// <summary>
    ///     是否转发响应状态码
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool WithResponseStatusCode { get; set; } = true;

    /// <summary>
    ///     是否转发响应标头
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool WithResponseHeaders { get; set; } = true;

    /// <summary>
    ///     是否转发响应内容标头
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool WithResponseContentHeaders { get; set; } = true;

    /// <summary>
    ///     用于在转发响应之前执行自定义操作
    /// </summary>
    public Action<HttpContext, HttpResponseMessage>? OnForward { get; set; }
}