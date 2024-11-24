// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式多部分表单内容特性
/// </summary>
/// <remarks>需配合 <see cref="MultipartAttribute" /> 使用。</remarks>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MultipartFormAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="MultipartFormAttribute" />
    /// </summary>
    public MultipartFormAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="MultipartFormAttribute" />
    /// </summary>
    /// <param name="boundary">多部分表单内容的边界</param>
    public MultipartFormAttribute(string boundary) => Boundary = boundary;

    /// <summary>
    ///     多部分表单内容的边界
    /// </summary>
    public string? Boundary { get; set; }
}