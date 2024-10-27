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
        // 查找所有符合的参数
        var pathParameters = context.Parameters
            .Where(u => !HttpDeclarativeExtractorContext.SpecialArgumentTypes.Contains(u.Key.ParameterType)).ToArray();

        // 空检查
        if (pathParameters.Length == 0)
        {
            return;
        }

        // 遍历符合的参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in pathParameters)
        {
            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 检查参数类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
            if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
            {
                httpRequestBuilder.WithPathParameter(parameterName, value);

                continue;
            }

            // 空检查
            if (value is not null)
            {
                httpRequestBuilder.WithPathParameters(value, parameterName);
            }
        }
    }
}