// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HeadersAttribute" /> 特性提取器
/// </summary>
internal sealed class HeadersDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 HeadersAttribute 特性集合
        var headersAttributes = context.Method.GetDefinedCustomAttributes<HeadersAttribute>(true)?.ToArray();

        // 空检查
        if (headersAttributes is { Length: > 0 })
        {
            // 遍历所有 [Headers] 特性并添加到 HttpRequestBuilder 中
            foreach (var headersAttribute in headersAttributes)
            {
                // 获取标头
                var headerName = headersAttribute.Name;

                // 空检查
                ArgumentException.ThrowIfNullOrEmpty(headerName);

                // 添加请求标头
                if (headersAttribute.HasSetValues)
                {
                    httpRequestBuilder.WithHeader(headerName, headersAttribute.Values, headersAttribute.Escape);
                }
                // 移除请求标头
                else
                {
                    httpRequestBuilder.RemoveHeaders(headerName);
                }
            }
        }

        /* 情况二：当特性作用于参数时 */

        // 查找所有贴有 [Headers] 特性的参数
        var headersParameters = context.Parameters.Where(u =>
                HttpDeclarativeExtractorContext.FilterSpecialParameter(u.Key) &&
                u.Key.IsDefined(typeof(HeadersAttribute), true))
            .ToArray();

        // 空检查
        if (headersParameters.Length == 0)
        {
            return;
        }

        // 遍历所有贴有 [Headers] 特性的参数
        foreach (var (parameter, value) in headersParameters)
        {
            // 获取 HeadersAttribute 特性集合
            var parameterHeadersAttributes = parameter.GetCustomAttributes<HeadersAttribute>(true);

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历所有 [Headers] 特性并添加到 HttpRequestBuilder 中
            foreach (var headersAttribute in parameterHeadersAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(headersAttribute.AliasAs)
                        ? parameterName
                        : headersAttribute.AliasAs.Trim();
                }

                // 添加请求标头
                httpRequestBuilder.WithHeader(parameterName, value ?? headersAttribute.Values, headersAttribute.Escape);
            }
        }
    }
}