// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="DoesNoReceiveContent" /> 内容转换器
/// </summary>
public class DoesNoReceiveContentConverter : IHttpContentConverter<DoesNoReceiveContent>
{
    /// <inheritdoc />
    public virtual DoesNoReceiveContent?
        Read(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken = default) =>
        default;

    /// <inheritdoc />
    public virtual Task<DoesNoReceiveContent?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<DoesNoReceiveContent?>(default);

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        Read(httpResponseMessage, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await ReadAsync(httpResponseMessage, cancellationToken);
}