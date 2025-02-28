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

    /// <summary>
    ///     为所有 <see cref="HttpClient" /> 添加 HTTP 远程请求分析工具处理委托
    /// </summary>
    /// <param name="disableIn">自定义禁用配置委托</param>
    /// <returns>
    ///     <see cref="IHttpRemoteBuilder" />
    /// </returns>
    IHttpRemoteBuilder AddProfilerDelegatingHandler(Func<bool>? disableIn = null);

    /// <summary>
    ///     为所有 <see cref="HttpClient" /> 添加 HTTP 远程请求分析工具处理委托
    /// </summary>
    /// <param name="disableInProduction">是否在生产环境中禁用。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="IHttpClientBuilder" />
    /// </returns>
    IHttpRemoteBuilder AddProfilerDelegatingHandler(bool disableInProduction);
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

    /// <inheritdoc />
    public IHttpRemoteBuilder AddProfilerDelegatingHandler(Func<bool>? disableIn = null)
    {
        Services.ConfigureHttpClientDefaults(builder =>
        {
            builder.AddProfilerDelegatingHandler(disableIn);
        });

        return this;
    }

    /// <inheritdoc />
    public IHttpRemoteBuilder AddProfilerDelegatingHandler(bool disableInProduction)
    {
        Services.ConfigureHttpClientDefaults(builder =>
        {
            builder.AddProfilerDelegatingHandler(disableInProduction);
        });

        return this;
    }
}