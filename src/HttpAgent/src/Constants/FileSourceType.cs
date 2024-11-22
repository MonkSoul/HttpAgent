// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     指定多部分表单内容文件的来源类型
/// </summary>
public enum FileSourceType
{
    /// <summary>
    ///     缺省值
    /// </summary>
    /// <remarks>不用作为文件的来源。</remarks>
    None = 0,

    /// <summary>
    ///     文件路径
    /// </summary>
    Path,

    /// <summary>
    ///     Base64 字符串
    /// </summary>
    Base64String,

    /// <summary>
    ///     互联网地址
    /// </summary>
    Remote
}