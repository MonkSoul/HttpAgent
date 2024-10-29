// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     路径参数提取器
/// </summary>
internal sealed class PathDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        /* 情况一：当特性作用于方法或接口时 */

        // 获取 PathAttribute 特性集合
        var pathAttributes = context.Method.GetDefinedCustomAttributes<PathAttribute>(true, false)?.ToArray();

        // 空检查
        if (pathAttributes is { Length: > 0 })
        {
            // 遍历所有 [Path] 特性并添加到 HttpRequestBuilder 中
            foreach (var pathAttribute in pathAttributes)
            {
                // 设置路径参数
                httpRequestBuilder.WithPathParameter(pathAttribute.Name, pathAttribute.Value);
            }
        }

        /* 情况二：将所有非特殊参数添加到路径参数中 */

        // 查找所有路径参数
        var pathParameters = context.Parameters
            .Where(u => HttpDeclarativeExtractorContext.FilterSpecialParameter(u.Key)).ToArray();

        // 空检查
        if (pathParameters.Length == 0)
        {
            return;
        }

        // 遍历所有路径参数
        foreach (var (parameter, value) in pathParameters)
        {
            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out _);

            // 检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
            if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
            {
                // 设置路径参数
                httpRequestBuilder.WithPathParameter(parameterName, value);

                continue;
            }

            // 设置路径参数
            httpRequestBuilder.WithPathParameters(value, parameterName);
        }
    }
}