// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="QueryAttribute" /> 特性提取器
/// </summary>
internal sealed class QueryDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 查找所有贴有 [Query] 特性的参数
        var queryParameters = context.Parameters.Where(u =>
            !HttpDeclarativeExtractorContext.SpecialArgumentTypes.Contains(u.Key.ParameterType) &&
            u.Key.IsDefined(typeof(QueryAttribute))).ToArray();

        // 空检查
        if (queryParameters.Length == 0)
        {
            return;
        }

        // 遍历查询参数集合并添加到 HttpRequestBuilder 构建器中
        foreach (var (parameter, value) in queryParameters)
        {
            // 获取 QueryAttribute 实例
            var queryAttribute = parameter.GetCustomAttribute<QueryAttribute>()!;

            // 获取参数名
            var parameterName = AliasAsUtility.GetParameterName(parameter, out var aliasAsDefined);

            // 检查参数是否贴了 [AliasAs] 特性
            if (!aliasAsDefined)
            {
                parameterName = string.IsNullOrWhiteSpace(queryAttribute.AliasAs)
                    ? parameterName
                    : queryAttribute.AliasAs.Trim();
            }

            // 检查参数类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
            if (parameter.ParameterType.IsBaseTypeOrEnumOrCollection())
            {
                httpRequestBuilder.WithQueryParameter(parameterName, value, queryAttribute.Escape);

                continue;
            }

            // 空检查
            if (value is not null)
            {
                httpRequestBuilder.WithQueryParameters(value, queryAttribute.Prefix, queryAttribute.Escape);
            }
        }
    }
}