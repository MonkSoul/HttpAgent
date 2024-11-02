﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class BodyAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="BodyAttribute" />
    /// </summary>
    public BodyAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="BodyAttribute" />
    /// </summary>
    /// <param name="contentType">内容类型</param>
    public BodyAttribute(string contentType) => ContentType = contentType;

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    public BodyAttribute(string contentType, string contentEncoding)
        : this(contentType) =>
        ContentEncoding = contentEncoding;

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public string? ContentEncoding { get; set; }
}