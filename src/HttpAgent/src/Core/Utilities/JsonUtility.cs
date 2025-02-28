// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Utilities;

/// <summary>
///     提供 JSON 实用方法
/// </summary>
public static class JsonUtility
{
    /// <summary>
    ///     解析 JSON 字符串
    /// </summary>
    /// <param name="jsonString">JSON 字符串</param>
    /// <returns>
    ///     <see cref="JsonDocument" />
    /// </returns>
    /// <exception cref="JsonException"></exception>
    public static JsonDocument Parse(string jsonString)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonString);

        return JsonDocument.Parse(jsonString);
    }
}