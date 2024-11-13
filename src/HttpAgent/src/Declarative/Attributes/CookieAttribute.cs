// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 Cookie 特性
/// </summary>
/// <remarks>支持多次指定。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class CookieAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="CookieAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public CookieAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="CookieAttribute" />
    /// </summary>
    /// <remarks>当特性作用于方法或接口时，则表示移除指定 Cookie 操作。</remarks>
    /// <param name="name">Cookie</param>
    public CookieAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="CookieAttribute" />
    /// </summary>
    /// <param name="name">Cookie</param>
    /// <param name="value">Cookie 的值</param>
    public CookieAttribute(string name, object? value)
        : this(name) =>
        Value = value;

    /// <summary>
    ///     Cookie
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Cookie 的值
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
    ///     是否设置了值
    /// </summary>
    internal bool HasSetValue { get; private set; }
}