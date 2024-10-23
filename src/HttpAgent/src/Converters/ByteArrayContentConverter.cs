// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     字节数组内容转换器
/// </summary>
public class ByteArrayContentConverter : IHttpContentConverter<byte[]>
{
    /// <inheritdoc />
    public virtual byte[]? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken).GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<byte[]?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        Read(httpResponseMessage, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await ReadAsync(httpResponseMessage, cancellationToken);
}