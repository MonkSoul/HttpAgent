﻿// 版权归百小僧及百签科技（广东）有限公司所有。
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
    /// <remarks>
    ///     <para>当特性作用于方法或接口时，则表示移除指定查询参数操作。</para>
    ///     <para>当特性作用于参数时，则表示添加查询参数，同时设置查询参数键为 <c>name</c> 的值。</para>
    /// </remarks>
    /// <param name="name">查询参数键</param>
    public QueryAttribute(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="name">查询参数键</param>
    /// <param name="value">查询参数的值</param>
    public QueryAttribute(string name, object? value)
        : this(name) =>
        Value = value;

    /// <summary>
    ///     查询参数键
    /// </summary>
    /// <remarks>该属性优先级低于 <see cref="AliasAs" /> 属性设置的值。</remarks>
    public string? Name { get; set; }

    /// <summary>
    ///     查询参数的值
    /// </summary>
    /// <remarks>当特性作用于参数时，表示默认值。</remarks>
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
    /// <remarks>
    ///     <para>特性用于参数时有效。</para>
    ///     <para>该属性优先级高于 <see cref="Name" /> 属性设置的值。</para>
    /// </remarks>
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
    ///     是否替换已存在的查询参数。默认值为 <c>false</c>
    /// </summary>
    public bool Replace { get; set; }

    /// <summary>
    ///     是否设置了值
    /// </summary>
    internal bool HasSetValue { get; private set; }
}