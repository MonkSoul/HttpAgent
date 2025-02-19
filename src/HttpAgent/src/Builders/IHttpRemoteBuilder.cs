// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求服务构建器
/// </summary>
public interface IHttpRemoteBuilder
{
    /// <summary>
    ///     <see cref="IServiceCollection" />
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    ///     配置 <see cref="HttpRemoteOptions" /> 实例
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    IHttpRemoteBuilder ConfigureOptions(Action<HttpRemoteOptions> configure);

    /// <summary>
    ///     配置 <see cref="HttpRemoteOptions" /> 实例
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    IHttpRemoteBuilder ConfigureOptions(Action<HttpRemoteOptions, IServiceProvider> configure);
}

/// <summary>
///     <see cref="IHttpRemoteBuilder" /> 默认实现
/// </summary>
internal sealed class DefaultHttpRemoteBuilder : IHttpRemoteBuilder
{
    /// <summary>
    ///     <inheritdoc cref="DefaultHttpRemoteBuilder" />
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    public DefaultHttpRemoteBuilder(IServiceCollection services)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(services);

        Services = services;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public IHttpRemoteBuilder ConfigureOptions(Action<HttpRemoteOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        Services.Configure(configure);

        return this;
    }

    /// <inheritdoc />
    public IHttpRemoteBuilder ConfigureOptions(Action<HttpRemoteOptions, IServiceProvider> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        Services.AddOptions<HttpRemoteOptions>().Configure(configure);

        return this;
    }
}