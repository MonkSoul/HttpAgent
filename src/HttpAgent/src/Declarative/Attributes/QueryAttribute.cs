// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式查询参数特性
/// </summary>
/// <remarks>支持多次指定。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class QueryAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public QueryAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <remarks>当特性作用于方法或接口时，则表示移除指定查询参数操作。</remarks>
    /// <param name="name">查询参数</param>
    public QueryAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="name">查询参数</param>
    /// <param name="value">查询参数的值</param>
    public QueryAttribute(string name, object? value)
        : this(name) =>
        Value = value;

    /// <summary>
    ///     查询参数
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     查询参数的值
    /// </summary>
    public object? Value
    {
        get;
        set
        {
            field = value;
            HasSetValue = true;
        }
    }

    /// <summary>
    ///     别名
    /// </summary>
    /// <remarks>特性用于参数时有效。</remarks>
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

    /// <summary>
    ///     是否值已存在时则采用替换的方式，否则采用追加方式。默认值为 <c>false</c>
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    ///     是否设置了值
    /// </summary>
    internal bool HasSetValue { get; private set; }
}