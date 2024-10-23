// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="IEnumerable" /> 拓展类
/// </summary>
internal static class IEnumerableExtensions
{
    /// <summary>
    ///     根据指定类型筛选 <see cref="IEnumerable" /> 的元素
    /// </summary>
    /// <param name="source">
    ///     <see cref="IEnumerable" />
    /// </param>
    /// <param name="resultType">筛选的结果类型</param>
    /// <returns>
    ///     <see cref="IEnumerable" />
    /// </returns>
    internal static IEnumerable OfType(this IEnumerable source, Type resultType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(resultType);

        foreach (var obj in source)
        {
            if (resultType.IsInstanceOfType(obj))
            {
                yield return obj;
            }
        }
    }
}