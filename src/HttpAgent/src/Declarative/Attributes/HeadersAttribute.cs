// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     请求标头特性
/// </summary>
/// <remarks>支持指定多次。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class HeadersAttribute : Attribute
{
    /// <summary>
    ///     <see cref="Values" /> 私有字段
    /// </summary>
    private object? _values;

    /// <summary>
    ///     <inheritdoc cref="HeadersAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public HeadersAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HeadersAttribute" />
    /// </summary>
    /// <remarks>当特性作用于方法或接口时，则表示移除指定标头操作。</remarks>
    /// <param name="name">标头</param>
    public HeadersAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="HeadersAttribute" />
    /// </summary>
    /// <param name="name">标头</param>
    /// <param name="value">标头的值</param>
    public HeadersAttribute(string name, object? value)
        : this(name) =>
        Values = value;

    /// <summary>
    ///     标头
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     标头的值
    /// </summary>
    public object? Values
    {
        get => _values;
        set
        {
            _values = value;
            HasSetValues = true;
        }
    }

    /// <summary>
    ///     是否设置了标头的值
    /// </summary>
    internal bool HasSetValues { get; private set; }

    /// <summary>
    ///     别名
    /// </summary>
    /// <remarks>特性用于参数时有效。</remarks>
    public string? AliasAs { get; set; }
    
    /// <summary>
    ///     是否转义
    /// </summary>
    public bool Escape { get; set; }
}