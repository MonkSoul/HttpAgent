// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="ObjectContentConverter{TResult}" /> 默认基类
/// </summary>
public class ObjectContentConverter : IHttpContentConverter
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content.ReadFromJsonAsync(resultType,
            ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault, cancellationToken).GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync(resultType,
            ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault, cancellationToken);
}

/// <summary>
///     对象转换器
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class ObjectContentConverter<TResult> : ObjectContentConverter, IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public virtual TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content.ReadFromJsonAsync<TResult>(
            ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault, cancellationToken).GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync<TResult>(
            ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault, cancellationToken);
}