﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StringContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new StringContentConverter();
        Assert.NotNull(converter);
        Assert.True(typeof(IHttpContentConverter<string>).IsAssignableFrom(typeof(StringContentConverter)));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new StringContentConverter();
        var str = converter.Read(httpResponseMessage);
        Assert.Equal("furion", str);
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var converter = new StringContentConverter();
        var str = await converter.ReadAsync(httpResponseMessage);
        Assert.Equal("furion", str);
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var stringContent = new StringContent("furion");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new StringContentConverter();
        var str = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.Equal("furion", str);
    }
}