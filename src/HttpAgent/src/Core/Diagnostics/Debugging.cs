// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System;

/// <summary>
///     向事件管理器中输出事件信息
/// </summary>
internal static class Debugging
{
    /// <summary>
    ///     输出一行事件信息
    /// </summary>
    /// <param name="level">
    ///     <para>信息级别</para>
    ///     <list type="number">
    ///         <item>
    ///             <description>跟踪</description>
    ///         </item>
    ///         <item>
    ///             <description>信息</description>
    ///         </item>
    ///         <item>
    ///             <description>警告</description>
    ///         </item>
    ///         <item>
    ///             <description>错误</description>
    ///         </item>
    ///         <item>
    ///             <description>文件</description>
    ///         </item>
    ///         <item>
    ///             <description>提示</description>
    ///         </item>
    ///         <item>
    ///             <description>搜索</description>
    ///         </item>
    ///         <item>
    ///             <description>时钟</description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <param name="message">事件信息</param>
    internal static void WriteLine(int level, string message)
    {
        // 获取信息级别对应的 emoji
        var category = GetLevelEmoji(level);

        Debug.WriteLine(message, category);
    }

    /// <summary>
    ///     输出错误级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    internal static void Error(string message) => WriteLine(4, message);

    /// <summary>
    ///     输出文件级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    internal static void File(string message) => WriteLine(5, message);

    /// <summary>
    ///     获取信息级别对应的 emoji
    /// </summary>
    /// <param name="level">
    ///     <para>信息级别</para>
    ///     <list type="number">
    ///         <item>
    ///             <description>跟踪</description>
    ///         </item>
    ///         <item>
    ///             <description>信息</description>
    ///         </item>
    ///         <item>
    ///             <description>警告</description>
    ///         </item>
    ///         <item>
    ///             <description>错误</description>
    ///         </item>
    ///         <item>
    ///             <description>文件</description>
    ///         </item>
    ///         <item>
    ///             <description>提示</description>
    ///         </item>
    ///         <item>
    ///             <description>搜索</description>
    ///         </item>
    ///         <item>
    ///             <description>时钟</description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string GetLevelEmoji(int level) =>
        level switch
        {
            1 => "🛠️",
            2 => "ℹ️",
            3 => "⚠️",
            4 => "❌",
            5 => "📄",
            6 => "💡",
            7 => "🔍",
            8 => "⏱️",
            _ => string.Empty
        };
}