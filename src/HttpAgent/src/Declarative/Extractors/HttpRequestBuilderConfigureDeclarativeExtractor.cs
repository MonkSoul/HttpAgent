// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpRequestBuilder" /> 自定义配置提取器
/// </summary>
internal sealed class HttpRequestBuilderConfigureDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 尝试解析单个 Action<HttpRequestBuilder> 参数
        if (context.Args.SingleOrDefault(u => u is Action<HttpRequestBuilder>) is Action<HttpRequestBuilder> configure)
        {
            configure.Invoke(httpRequestBuilder);
        }
    }
}