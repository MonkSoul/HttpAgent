// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="MultipartContent" /> 请求内容提取器
/// </summary>
internal sealed class MultipartBodyDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 尝试解析 Action<HttpMultipartFormDataBuilder> 参数
        if (context.Args.FirstOrDefault(u => u is Action<HttpMultipartFormDataBuilder>) is
            Action<HttpMultipartFormDataBuilder>
            multipartContentBuilderAction)
        {
            httpRequestBuilder.SetMultipartContent(multipartContentBuilderAction);
        }
    }
}