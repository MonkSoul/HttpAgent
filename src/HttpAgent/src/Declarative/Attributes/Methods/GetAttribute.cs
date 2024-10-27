﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP GET 远程请求声明式特性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class GetAttribute : HttpMethodAttribute
{
    /// <summary>
    ///     <inheritdoc cref="GetAttribute" />
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    public GetAttribute(string? requestUri = null)
        : base(HttpMethod.Get, requestUri)
    {
    }
}