// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     查询参数特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class QueryAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    public QueryAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="aliasAs">别名</param>
    public QueryAttribute(string aliasAs) => AliasAs = aliasAs;

    /// <summary>
    ///     别名
    /// </summary>
    public string? AliasAs { get; set; }

    /// <summary>
    ///     是否转义
    /// </summary>
    public bool Escape { get; set; }

    /// <summary>
    ///     参数前缀
    /// </summary>
    /// <remarks>作用于对象类型时有效。</remarks>
    public string? Prefix { get; set; }
}