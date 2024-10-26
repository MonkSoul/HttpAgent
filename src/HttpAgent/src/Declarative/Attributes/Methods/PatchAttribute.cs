﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP PATCH 远程请求声明式特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PatchAttribute : HttpMethodAttribute
{
    /// <summary>
    ///     <inheritdoc cref="PatchAttribute" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    public PatchAttribute(string? requestUri = null)
        : base(HttpMethod.Patch, requestUri)
    {
    }
}