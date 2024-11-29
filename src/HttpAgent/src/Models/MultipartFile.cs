// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     多部分表单文件
/// </summary>
/// <remarks>使用 <c>MultipartFile.CreateFrom[Source]</c> 静态方法创建。</remarks>
public sealed class MultipartFile
{
    /// <summary>
    ///     <inheritdoc cref="MultipartFile" />
    /// </summary>
    internal MultipartFile()
    {
    }

    /// <summary>
    ///     表单名称
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    ///     文件的名称
    /// </summary>
    public string? FileName { get; private set; }

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; private set; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public Encoding? ContentEncoding { get; private set; }

    /// <summary>
    ///     文件来源
    /// </summary>
    public object? Source { get; private set; }

    /// <summary>
    ///     <see cref="FileSourceType" />
    /// </summary>
    internal FileSourceType FileSourceType { get; private set; }

    /// <summary>
    ///     从字节数组中添加文件
    /// </summary>
    /// <param name="byteArray">字节数组</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="MultipartFile" />
    /// </returns>
    public static MultipartFile CreateFromByteArray(byte[] byteArray, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null) =>
        new()
        {
            Name = name,
            FileName = fileName,
            ContentType = contentType,
            ContentEncoding = contentEncoding,
            Source = byteArray,
            FileSourceType = FileSourceType.ByteArray
        };

    /// <summary>
    ///     从 <see cref="Stream" /> 中添加文件
    /// </summary>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="MultipartFile" />
    /// </returns>
    public static MultipartFile CreateFromStream(Stream stream, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null) =>
        new()
        {
            Name = name,
            FileName = fileName,
            ContentType = contentType,
            ContentEncoding = contentEncoding,
            Source = stream,
            FileSourceType = FileSourceType.Stream
        };

    /// <summary>
    ///     从本地路径中添加文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="MultipartFile" />
    /// </returns>
    public static MultipartFile CreateFromPath(string filePath, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null) =>
        new()
        {
            Name = name,
            FileName = fileName,
            ContentType = contentType,
            ContentEncoding = contentEncoding,
            Source = filePath,
            FileSourceType = FileSourceType.Path
        };

    /// <summary>
    ///     从 Base64 字符串中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>100MB</c> 以内。</remarks>
    /// <param name="base64String">Base64 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="MultipartFile" />
    /// </returns>
    public static MultipartFile CreateFromBase64String(string base64String, string name = "file",
        string? fileName = null, string? contentType = null, Encoding? contentEncoding = null) =>
        new()
        {
            Name = name,
            FileName = fileName,
            ContentType = contentType,
            ContentEncoding = contentEncoding,
            Source = base64String,
            FileSourceType = FileSourceType.Base64String
        };

    /// <summary>
    ///     从互联网 URL 中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>100MB</c> 以内。</remarks>
    /// <param name="url">互联网 URL 地址</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="MultipartFile" />
    /// </returns>
    public static MultipartFile CreateFromRemote(string url, string name = "file", string? fileName = null,
        string? contentType = null, Encoding? contentEncoding = null) =>
        new()
        {
            Name = name,
            FileName = fileName,
            ContentType = contentType,
            ContentEncoding = contentEncoding,
            Source = url,
            FileSourceType = FileSourceType.Remote
        };
}