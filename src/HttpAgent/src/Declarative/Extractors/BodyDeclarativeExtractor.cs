// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="BodyAttribute" /> 特性提取器
/// </summary>
internal sealed class BodyDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 查找所有贴有 [Body] 特性的参数
        var bodyParameters = context.Parameters.Where(u =>
            !HttpDeclarativeExtractorContext.SpecialArgumentTypes.Contains(u.Key.ParameterType) &&
            u.Key.IsDefined(typeof(BodyAttribute))).ToArray();

        // 空检查
        if (bodyParameters.Length == 0)
        {
            return;
        }

        // 获取首个贴有 [Body] 特性的参数
        var firstBodyParameter = bodyParameters.First();

        // 获取 BodyAttribute 实例
        var bodyAttribute = firstBodyParameter.Key.GetCustomAttribute<BodyAttribute>()!;

        // 设置原始请求内容
        httpRequestBuilder.SetRawContent(firstBodyParameter.Value, bodyAttribute.ContentType);

        // 设置内容编码
        if (!string.IsNullOrWhiteSpace(bodyAttribute.ContentEncoding))
        {
            httpRequestBuilder.SetContentEncoding(bodyAttribute.ContentEncoding);
        }
    }
}