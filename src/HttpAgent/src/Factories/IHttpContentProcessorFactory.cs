// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentProcessor" /> 工厂
/// </summary>
public interface IHttpContentProcessorFactory
{
    /// <summary>
    ///     构建 <see cref="HttpContent" /> 实例
    /// </summary>
    /// <param name="rawContent">原始请求内容</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="encoding">内容编码</param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 数组</param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    HttpContent? Build(object? rawContent, string contentType, Encoding? encoding = null,
        params IHttpContentProcessor[]? processors);
}