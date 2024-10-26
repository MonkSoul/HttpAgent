// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpContentConverterFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory1 = new HttpContentConverterFactory(serviceProvider, null);
        Assert.NotNull(httpContentConverterFactory1._serviceProvider);
        Assert.NotNull(httpContentConverterFactory1._converters);
        Assert.Equal(4, httpContentConverterFactory1._converters.Count);
        Assert.Equal(
            [
                typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(DoesNoReceiveContentConverter)
            ],
            httpContentConverterFactory1._converters.Select(u => u.Key));

        var httpContentConverterFactory2 =
            new HttpContentConverterFactory(serviceProvider, [new CustomStringContentConverter()]);
        Assert.NotNull(httpContentConverterFactory2._converters);
        Assert.Equal(5, httpContentConverterFactory2._converters.Count);
        Assert.Equal(
            [
                typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(DoesNoReceiveContentConverter),
                typeof(CustomStringContentConverter)
            ],
            httpContentConverterFactory2._converters.Select(u => u.Key));

        var httpContentConverterFactory3 =
            new HttpContentConverterFactory(serviceProvider,
                [new StringContentConverter(), new ByteArrayContentConverter()]);
        Assert.NotNull(httpContentConverterFactory3._converters);
        Assert.Equal(4, httpContentConverterFactory3._converters.Count);
        Assert.Equal(
            [
                typeof(StringContentConverter), typeof(ByteArrayContentConverter),
                typeof(StreamContentConverter), typeof(DoesNoReceiveContentConverter)
            ],
            httpContentConverterFactory3._converters.Select(u => u.Key));
    }

    [Fact]
    public void GetConverter_Invalid_Parameters()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = httpContentConverterFactory.GetConverter<HttpResponseMessage>();
        });

        Assert.Equal("`HttpResponseMessage` type cannot be directly processed as `TResult`.", exception.Message);
    }

    [Fact]
    public void GetConverter_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, null);

        Assert.Equal(typeof(StringContentConverter), httpContentConverterFactory.GetConverter<string>().GetType());
        Assert.Equal(typeof(ByteArrayContentConverter), httpContentConverterFactory.GetConverter<byte[]>().GetType());
        Assert.Equal(typeof(StreamContentConverter), httpContentConverterFactory.GetConverter<Stream>().GetType());
        Assert.Equal(typeof(DoesNoReceiveContentConverter),
            httpContentConverterFactory.GetConverter<DoesNoReceiveContent>().GetType());
        Assert.Equal(typeof(ObjectContentConverter<int>), httpContentConverterFactory.GetConverter<int>().GetType());
        Assert.Equal(typeof(ObjectContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter<ObjectModel>().GetType());
    }

    [Fact]
    public void GetConverter_Of_Customize_ObjectContentConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote(builder =>
        {
            builder.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();
        });

        using var serviceProvider = services.BuildServiceProvider();
        var httpContentConverterFactory = serviceProvider.GetRequiredService<IHttpContentConverterFactory>();

        Assert.Equal(typeof(CustomObjectContentConverter<ObjectModel>),
            httpContentConverterFactory.GetConverter<ObjectModel>().GetType());
    }

    [Fact]
    public void GetConverter_WithCustomize_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, [new CustomByteArrayContentConverter()]);

        Assert.Equal(typeof(CustomStringContentConverter),
            httpContentConverterFactory.GetConverter<string>(new CustomStringContentConverter()).GetType());
        Assert.Equal(typeof(CustomByteArrayContentConverter),
            httpContentConverterFactory.GetConverter<byte[]>().GetType());
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        var result = httpContentConverterFactory.Read<string>(httpResponseMessage);
        Assert.Equal("furion", result);

        var result2 = httpContentConverterFactory.Read<HttpResponseMessage>(httpResponseMessage);
        Assert.Equal(result2, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        await using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        var result = await httpContentConverterFactory.ReadAsync<string>(httpResponseMessage);
        Assert.Equal("furion", result);

        var result2 = await httpContentConverterFactory.ReadAsync<HttpResponseMessage>(httpResponseMessage);
        Assert.Equal(result2, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        await using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await httpContentConverterFactory.ReadAsync<string>(httpResponseMessage,
            cancellationToken: cancellationTokenSource.Token);
        Assert.Equal("furion", result);
    }

    [Fact]
    public void GetConverter_WithType_Invalid_Parameters()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, null);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = httpContentConverterFactory.GetConverter(typeof(HttpResponseMessage));
        });

        Assert.Equal("`HttpResponseMessage` type cannot be directly processed as result type.", exception.Message);
    }

    [Fact]
    public void GetConverter_WithType_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory = new HttpContentConverterFactory(serviceProvider, null);

        Assert.Equal(typeof(StringContentConverter),
            httpContentConverterFactory.GetConverter(typeof(string)).GetType());
        Assert.Equal(typeof(ByteArrayContentConverter),
            httpContentConverterFactory.GetConverter(typeof(byte[])).GetType());
        Assert.Equal(typeof(StreamContentConverter),
            httpContentConverterFactory.GetConverter(typeof(Stream)).GetType());
        Assert.Equal(typeof(ObjectContentConverter), httpContentConverterFactory.GetConverter(typeof(int)).GetType());
        Assert.Equal(typeof(ObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(ObjectModel)).GetType());
    }

    [Fact]
    public void GetConverter_WithType_Of_Customize_ObjectContentConverter_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote(builder =>
        {
            builder.UseObjectContentConverterFactory<CustomObjectContentConverterFactory>();
        });

        using var serviceProvider = services.BuildServiceProvider();
        var httpContentConverterFactory = serviceProvider.GetRequiredService<IHttpContentConverterFactory>();

        Assert.Equal(typeof(CustomObjectContentConverter),
            httpContentConverterFactory.GetConverter(typeof(ObjectModel)).GetType());
    }

    [Fact]
    public void GetConverter_WithType_WithCustomize_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, [new CustomByteArrayContentConverter()]);

        Assert.Equal(typeof(CustomStringContentConverter),
            httpContentConverterFactory.GetConverter(typeof(string), new CustomStringContentConverter()).GetType());
        Assert.Equal(typeof(CustomByteArrayContentConverter),
            httpContentConverterFactory.GetConverter(typeof(byte[])).GetType());
    }

    [Fact]
    public void Read_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        var result = httpContentConverterFactory.Read(typeof(string), httpResponseMessage);
        Assert.Equal("furion", result);

        var result2 = httpContentConverterFactory.Read(typeof(HttpResponseMessage), httpResponseMessage);
        Assert.Equal(result2, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        await using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        var result = await httpContentConverterFactory.ReadAsync(typeof(string), httpResponseMessage);
        Assert.Equal("furion", result);

        var result2 = await httpContentConverterFactory.ReadAsync(typeof(HttpResponseMessage), httpResponseMessage);
        Assert.Equal(result2, httpResponseMessage);
    }

    [Fact]
    public async Task ReadAsync_WithType_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        await using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var httpContentConverterFactory =
            new HttpContentConverterFactory(serviceProvider, null);

        using var cancellationTokenSource = new CancellationTokenSource();

        var result = await httpContentConverterFactory.ReadAsync(typeof(string), httpResponseMessage,
            cancellationToken: cancellationTokenSource.Token);
        Assert.Equal("furion", result);
    }
}