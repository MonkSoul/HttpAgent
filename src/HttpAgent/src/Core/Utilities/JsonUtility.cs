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

    /// <summary>
    ///     尝试解析 JSON 字符串
    /// </summary>
    /// <param name="jsonString">JSON 字符串</param>
    /// <param name="jsonDocument">
    ///     <see cref="JsonDocument" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool TryParse(string jsonString, [NotNullWhen(true)] out JsonDocument? jsonDocument)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonString);

        try
        {
            jsonDocument = Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            jsonDocument = null;
            return false;
        }
    }

    /// <summary>
    ///     检查 <see cref="JsonDocument" /> 的 <c>ValueKind</c> 属性值是否是 <c>Object</c>、<c>Array</c> 或 <c>Null</c>
    /// </summary>
    /// <param name="jsonDocument">
    ///     <see cref="JsonDocument" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsObjectOrArrayOrNull(JsonDocument jsonDocument)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonDocument);

        var root = jsonDocument.RootElement;
        return root.ValueKind is JsonValueKind.Object or JsonValueKind.Array or JsonValueKind.Null;
    }
}