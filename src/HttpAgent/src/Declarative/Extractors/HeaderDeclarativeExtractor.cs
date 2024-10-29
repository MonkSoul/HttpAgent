// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HeaderAttribute" /> 特性提取器
/// </summary>
internal sealed class HeaderDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 HeaderAttribute 特性集合
        var headerAttributes = context.Method.GetDefinedCustomAttributes<HeaderAttribute>(true, false)?.ToArray();

        // 空检查
        if (headerAttributes is { Length: > 0 })
        {
            // 遍历所有 [Header] 特性并添加到 HttpRequestBuilder 中
            foreach (var headerAttribute in headerAttributes)
            {
                // 获取标头
                var headerName = headerAttribute.Name;

                // 空检查
                ArgumentException.ThrowIfNullOrEmpty(headerName);

                // 添加请求标头
                if (headerAttribute.HasSetValue)
                {
                    httpRequestBuilder.WithHeader(headerName, headerAttribute.Value, headerAttribute.Escape);
                }
                // 移除请求标头
                else
                {
                    httpRequestBuilder.RemoveHeaders(headerName);
                }
            }
        }

        /* 情况二：当特性作用于参数时 */

        // 查找所有贴有 [Header] 特性的参数
        var headersParameters = context.Parameters.Where(u =>
                HttpDeclarativeExtractorContext.FilterSpecialParameter(u.Key) &&
                u.Key.IsDefined(typeof(HeaderAttribute), true))
            .ToArray();

        // 空检查
        if (headersParameters.Length == 0)
        {
            return;
        }

        // 遍历所有贴有 [Header] 特性的参数
        foreach (var (parameter, value) in headersParameters)
        {
            // 获取 HeaderAttribute 特性集合
            var parameterHeaderAttributes = parameter.GetCustomAttributes<HeaderAttribute>(true);

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历所有 [Header] 特性并添加到 HttpRequestBuilder 中
            foreach (var headerAttribute in parameterHeaderAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(headerAttribute.AliasAs)
                        ? parameterName
                        : headerAttribute.AliasAs.Trim();
                }

                // 添加请求标头
                httpRequestBuilder.WithHeader(parameterName, value ?? headerAttribute.Value, headerAttribute.Escape);
            }
        }
    }
}