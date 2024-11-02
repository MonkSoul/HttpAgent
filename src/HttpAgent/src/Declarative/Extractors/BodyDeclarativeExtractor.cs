// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="BodyAttribute" /> 特性提取器
/// </summary>
internal sealed class BodyDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 查找单个贴有 [Body] 特性的参数
        var bodyParameter = context.Parameters.SingleOrDefault(u =>
            HttpDeclarativeExtractorContext.FilterSpecialParameter(u.Key) &&
            u.Key.IsDefined(typeof(BodyAttribute), true));

        // 解析参数信息
        var (parameter, value) = bodyParameter;

        // 空检查
        if (parameter is null)
        {
            return;
        }

        // 获取 BodyAttribute 实例
        var bodyAttribute = parameter.GetCustomAttribute<BodyAttribute>(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(bodyAttribute);

        // 设置原始请求内容
        httpRequestBuilder.SetRawContent(value, bodyAttribute.ContentType);

        // 设置内容编码
        if (!string.IsNullOrWhiteSpace(bodyAttribute.ContentEncoding))
        {
            httpRequestBuilder.SetContentEncoding(bodyAttribute.ContentEncoding);
        }
    }
}