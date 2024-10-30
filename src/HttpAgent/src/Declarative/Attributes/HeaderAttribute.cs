// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     请求标头特性
/// </summary>
/// <remarks>支持多次指定。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class HeaderAttribute : Attribute
{
    /// <summary>
    ///     <see cref="Value" /> 私有字段
    /// </summary>
    private object? _value;

    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public HeaderAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <remarks>当特性作用于方法或接口时，则表示移除指定标头操作。</remarks>
    /// <param name="name">标头</param>
    public HeaderAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="HeaderAttribute" />
    /// </summary>
    /// <param name="name">标头</param>
    /// <param name="value">标头的值</param>
    public HeaderAttribute(string name, object? value)
        : this(name) =>
        Value = value;

    /// <summary>
    ///     标头
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     标头的值
    /// </summary>
    public object? Value
    {
        get => _value;
        set
        {
            _value = value;
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