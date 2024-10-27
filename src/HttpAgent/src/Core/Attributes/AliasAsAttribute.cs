// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System;

/// <summary>
///     设置别名特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class AliasAsAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="AliasAsAttribute" />
    /// </summary>
    /// <param name="aliasAs">别名</param>
    public AliasAsAttribute(string aliasAs) => AliasAs = aliasAs;

    /// <summary>
    ///     别名
    /// </summary>
    public string? AliasAs { get; set; }
}