// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式超时时间特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class TimeoutAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="TimeoutAttribute" />
    /// </summary>
    /// <param name="timeoutMilliseconds">超时时间（毫秒）</param>
    public TimeoutAttribute(double timeoutMilliseconds) => Timeout = timeoutMilliseconds;

    /// <summary>
    ///     超时时间（毫秒）
    /// </summary>
    public double Timeout { get; set; }
}