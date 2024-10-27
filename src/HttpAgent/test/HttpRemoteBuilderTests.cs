// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Null(builder._httpContentConverterProviders);
        Assert.Null(builder._httpContentProcessorProviders);
        Assert.Null(builder._objectContentConverterFactoryType);
        Assert.Null(builder._httpDeclarativeTypes);
        Assert.Null(builder.DefaultContentType);
        Assert.Null(builder.DefaultFileDownloadDirectory);
    }

    [Fact]
    public void SetDefaultContentType_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.SetDefaultContentType(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetDefaultContentType(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetDefaultContentType(" ");
        });

        Assert.Throws<FormatException>(() =>
        {
            builder.SetDefaultContentType("unknown");
        });
    }

    [Fact]
    public void SetDefaultContentType_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.SetDefaultContentType("text/plain");
        Assert.Equal("text/plain", builder.DefaultContentType);

        builder.SetDefaultContentType("text/html;charset=utf-8");
        Assert.Equal("text/html", builder.DefaultContentType);

        builder.SetDefaultContentType("text/html; charset=unicode");
        Assert.Equal("text/html", builder.DefaultContentType);
    }

    [Fact]
    public void SetDefaultFileDownloadDirectory_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.SetDefaultFileDownloadDirectory(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetDefaultFileDownloadDirectory(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetDefaultFileDownloadDirectory(" ");
        });
    }

    [Fact]
    public void SetDefaultFileDownloadDirectory_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.SetDefaultFileDownloadDirectory(@"C:\Workspaces");

        Assert.Equal(@"C:\Workspaces", builder.DefaultFileDownloadDirectory);
    }

    [Fact]
    public void AddHttpContentProcessors_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.AddHttpContentProcessors(null!);
        });
    }

    [Fact]
    public void AddHttpContentProcessors_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpContentProcessors(() => []);
        Assert.NotNull(builder._httpContentProcessorProviders);

        builder.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(builder._httpContentProcessorProviders);

        // 运行时将抛异常
        builder.AddHttpContentProcessors(() => [null!, new StringContentProcessor()]);
        Assert.NotNull(builder._httpContentProcessorProviders);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        builder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(builder2._httpContentProcessorProviders);
        Assert.Equal(2, builder2._httpContentProcessorProviders.Count);
    }

    [Fact]
    public void AddHttpContentConverters_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.AddHttpContentConverters(null!);
        });
    }

    [Fact]
    public void AddHttpContentConverters_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();

        builder.AddHttpContentConverters(() => []);
        Assert.NotNull(builder._httpContentConverterProviders);

        builder.AddHttpContentConverters(() =>
            [new StringContentConverter(), new ObjectContentConverter<int>()]);
        Assert.NotNull(builder._httpContentConverterProviders);

        // 运行时将抛异常
        builder.AddHttpContentConverters(() => [null!, new StringContentConverter()]);
        Assert.NotNull(builder._httpContentConverterProviders);

        var builder2 = new HttpRemoteBuilder();
        builder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        builder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        Assert.NotNull(builder2._httpContentConverterProviders);
        Assert.Equal(2, builder2._httpContentConverterProviders.Count);
    }

    [Fact]
    public void UseObjectContentConverterFactory_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.UseObjectContentConverterFactory(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            builder.UseObjectContentConverterFactory(typeof(NotImplementObjectContentConverterFactory)));
        Assert.Equal(
            $"`{typeof(NotImplementObjectContentConverterFactory)}` type is not assignable from `{typeof(IObjectContentConverterFactory)}`. (Parameter 'factoryType')",
            exception.Message);
    }

    [Fact]
    public void UseObjectContentConverterFactory_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.UseObjectContentConverterFactory(typeof(CustomObjectContentConverterFactory));

        Assert.NotNull(builder._objectContentConverterFactoryType);
        Assert.Equal(typeof(CustomObjectContentConverterFactory), builder._objectContentConverterFactoryType);

        var builder2 = new HttpRemoteBuilder();
        builder2.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        Assert.NotNull(builder2._objectContentConverterFactoryType);
        Assert.Equal(typeof(CustomObjectContentConverterFactory), builder2._objectContentConverterFactoryType);
    }

    [Fact]
    public void AddDeclarative_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddDeclarative(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            builder.AddDeclarative(typeof(INonHttpTest)));
        Assert.Equal(
            $"`{typeof(INonHttpTest)}` type is not assignable from `{typeof(IHttpDeclarative)}` or interface. (Parameter 'declarativeType')",
            exception.Message);
        Assert.Throws<ArgumentException>(() =>
            builder.AddDeclarative(typeof(HttpTest)));
    }

    [Fact]
    public void AddDeclarative_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        builder.AddDeclarative(typeof(IHttpTest));

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Single(builder._httpDeclarativeTypes);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());

        builder.AddDeclarative<IHttpTest>();
        Assert.Single(builder._httpDeclarativeTypes);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());
    }

    [Fact]
    public void AddDeclaratives_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddDeclaratives(null!));
    }

    [Fact]
    public void AddDeclaratives_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        // ReSharper disable once RedundantExplicitParamsArrayCreation
        builder.AddDeclaratives([typeof(IHttpTest), typeof(IHttpTest), typeof(IHttpTest2)]);

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Equal(2, builder._httpDeclarativeTypes.Count);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());
        Assert.Equal(typeof(IHttpTest2), builder._httpDeclarativeTypes.Last());
    }

    [Fact]
    public void AddDeclarativeFromAssemblies_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddDeclarativeFromAssemblies(null!));
    }

    [Fact]
    public void AddDeclarativeFromAssemblies_ReturnOK()
    {
        var builder = new HttpRemoteBuilder();
        // ReSharper disable once RedundantExplicitParamsArrayCreation
        builder.AddDeclarativeFromAssemblies([typeof(HttpRemoteBuilderTests).Assembly]);

        Assert.NotNull(builder._httpDeclarativeTypes);
        Assert.Equal(2, builder._httpDeclarativeTypes.Count);
        Assert.Equal(typeof(IHttpTest), builder._httpDeclarativeTypes.First());
        Assert.Equal(typeof(IHttpTest2), builder._httpDeclarativeTypes.Last());
    }

    [Fact]
    public void EnsureLegalData_Invalid_Parameters()
    {
        var exception = Assert.Throws<ArgumentException>(() => HttpRemoteBuilder.EnsureLegalData("unknown"));
        Assert.Equal("The provided default content type is not valid. (Parameter 'defaultContentType')",
            exception.Message);
    }

    [Fact]
    public void EnsureLegalData_ReturnOK()
    {
        HttpRemoteBuilder.EnsureLegalData(null);
        HttpRemoteBuilder.EnsureLegalData("text/html");
        HttpRemoteBuilder.EnsureLegalData("text/html; charset=unicode");
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new HttpRemoteBuilder { DefaultContentType = "unknown" };

        var exception = Assert.Throws<ArgumentException>(() => builder.Build(new ServiceCollection()));
        Assert.Equal("The provided default content type is not valid. (Parameter 'defaultContentType')",
            exception.Message);
    }

    [Fact]
    public void Build_Default_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder();

        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Equal(29, services.Count);
    }

    [Fact]
    public void Build_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder();

        builder.Build(services);
        builder.Build(services);
        builder.Build(services);

        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Equal(29, services.Count);
    }

    [Fact]
    public void Build_UseObjectContentConverterFactory_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectContentConverterFactory));
        Assert.Equal(30, services.Count);
    }

    [Fact]
    public void Build_UseObjectContentConverterFactory_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();

        builder.Build(services);
        builder.Build(services);
        builder.Build(services);
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpClientFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentProcessorFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpContentConverterFactory));
        Assert.Contains(services, u => u.ServiceType == typeof(IHttpRemoteService));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectContentConverterFactory));
        Assert.Equal(30, services.Count);
    }

    [Fact]
    public void BuildHttpDeclarativeServices_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().AddDeclarative<IHttpTest>();

        builder.BuildHttpDeclarativeServices(services);
        builder.BuildHttpDeclarativeServices(services);

        Assert.Contains(services, u => u.ServiceType == typeof(IHttpTest));
        Assert.Single(services);
    }

    [Fact]
    public void Build_Resolve_ReturnOK()
    {
        var services = new ServiceCollection();
        var builder = new HttpRemoteBuilder().SetDefaultContentType("application/json")
            .SetDefaultFileDownloadDirectory(@"C:\Workspaces")
            .AddHttpContentProcessors(() => [new CustomStringContentProcessor()])
            .AddHttpContentConverters(() => [new CustomStringContentConverter()])
            .AddDeclarative<IHttpTest>();

        builder.Build(services);

        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteService = serviceProvider.GetRequiredService<IHttpRemoteService>();
        Assert.Equal("application/json", httpRemoteService.RemoteOptions.DefaultContentType);
        Assert.Equal(@"C:\Workspaces", httpRemoteService.RemoteOptions.DefaultFileDownloadDirectory);

        var httpContentProcessorFactory =
            (HttpContentProcessorFactory)serviceProvider.GetRequiredService(typeof(IHttpContentProcessorFactory));
        Assert.NotNull(httpContentProcessorFactory._processors);
        Assert.Equal(6, httpContentProcessorFactory._processors.Count);
        Assert.Equal(
            [
                typeof(StringContentProcessor), typeof(FormUrlEncodedContentProcessor),
                typeof(ByteArrayContentProcessor), typeof(StreamContentProcessor),
                typeof(MultipartFormDataContentProcessor), typeof(CustomStringContentProcessor)
            ],
            httpContentProcessorFactory._processors.Select(u => u.Key));

        var httpContentConverterFactory =
            (HttpContentConverterFactory)serviceProvider.GetRequiredService(typeof(IHttpContentConverterFactory));
        Assert.NotNull(httpContentConverterFactory._converters);
        Assert.Equal(5, httpContentConverterFactory._converters.Count);
        Assert.Equal(
            [
                typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(DoesNoReceiveContentConverter),
                typeof(CustomStringContentConverter)
            ],
            httpContentConverterFactory._converters.Select(u => u.Key));

        var httpTest = serviceProvider.GetRequiredService<IHttpTest>();
        Assert.NotNull(httpTest);

        dynamic httpTestProxy = httpTest;
        Assert.NotNull(httpTestProxy.RemoteService);
    }
}