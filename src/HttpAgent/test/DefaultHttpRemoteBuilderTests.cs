// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DefaultHttpRemoteBuilderTests
{
    [Fact]
    public void DefaultHttpRemoteBuilder_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new DefaultHttpRemoteBuilder(null!));

    [Fact]
    public void DefaultHttpRemoteBuilder_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);
        Assert.NotNull(builder);
        Assert.Same(services, builder.Services);
    }

    [Fact]
    public void ConfigureOptions_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        Assert.Throws<ArgumentNullException>(() => builder.ConfigureOptions((Action<HttpRemoteOptions>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureOptions((Action<HttpRemoteOptions, IServiceProvider>)null!));
    }

    [Fact]
    public void ConfigureOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        builder.ConfigureOptions(options => options.DefaultContentType = "application/json");
        builder.ConfigureOptions((options, serviceProvider) =>
        {
            Assert.NotNull(serviceProvider);
            options.DefaultContentType = "application/json";
        });
    }

    [Fact]
    public void ConfigureHttpClientDefaults_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        Assert.Throws<ArgumentNullException>(() => builder.ConfigureHttpClientDefaults(null!));
    }

    [Fact]
    public void ConfigureHttpClientDefaults_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new DefaultHttpRemoteBuilder(services);

        services.AddHttpClient();
        services.AddHttpClient("github", client => { });

        builder.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddProfilerDelegatingHandler();
            clientBuilder.AddProfilerDelegatingHandler(true);
        });

        using var serviceProvider = services.BuildServiceProvider();
        var httpClientFactoryOptions = serviceProvider.GetService<IOptions<HttpClientFactoryOptions>>()?.Value;
        Assert.NotNull(httpClientFactoryOptions);
        Assert.NotNull(httpClientFactoryOptions.HttpMessageHandlerBuilderActions);
        Assert.Equal(2, httpClientFactoryOptions.HttpMessageHandlerBuilderActions.Count);
    }
}