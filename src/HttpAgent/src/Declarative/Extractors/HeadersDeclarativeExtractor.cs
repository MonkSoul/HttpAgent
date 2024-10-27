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
        // ++++ 情况一：当为方法或接口添加 [Headers] 特性时 ++++

        // 获取 HeadersAttribute 特性集合
        var headersAttributes = context.Method.GetCustomAttributes<HeadersAttribute>(true).ToArray();

        // 空检查
        if (headersAttributes.Length > 0)
        {
            foreach (var headersAttribute in headersAttributes)
            {
                // 判断是否是添加请求标头的操作
                if (headersAttribute.HasSetValues)
                {
                    httpRequestBuilder.WithHeader(headersAttribute.Name, headersAttribute.Values);
                }
                // 移除请求标头
                else
                {
                    httpRequestBuilder.RemoveHeaders(headersAttribute.Name);
                }
            }
        }

        // ++++ 情况二：当为参数添加 [Headers] 特性时 ++++

        // 查找所有贴有 [Headers] 特性的参数
        var headersParameters = context.Parameters.Where(u =>
                !HttpDeclarativeExtractorContext.SpecialArgumentTypes.Contains(u.Key.ParameterType) &&
                u.Key.IsDefined(typeof(HeadersAttribute)))
            .ToArray();

        // 空检查
        if (headersParameters.Length == 0)
        {
            return;
        }

        // 遍历请求标头参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in headersParameters)
        {
            // 获取 HeadersAttribute 特性集合
            var parameterHeadersAttributes = parameter.GetCustomAttributes<HeadersAttribute>()!;

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 遍历 HeadersAttribute 特性集合
            foreach (var headersAttribute in parameterHeadersAttributes)
            {
                // 检查参数是否贴了 [AliasAs] 特性
                if (!aliasAsDefined)
                {
                    parameterName = string.IsNullOrWhiteSpace(headersAttribute.AliasAs)
                        ? parameterName
                        : headersAttribute.AliasAs.Trim();
                }

                httpRequestBuilder.WithHeader(parameterName, value ?? headersAttribute.Values);
            }
        }
    }
}