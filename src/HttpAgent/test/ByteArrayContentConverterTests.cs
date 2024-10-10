﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ByteArrayContentConverterTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var converter = new ByteArrayContentConverter();
        Assert.NotNull(converter);
        Assert.True(typeof(IHttpContentConverter<byte[]>).IsAssignableFrom(typeof(ByteArrayContentConverter)));
    }

    [Fact]
    public void Read_ReturnOK()
    {
        using var byteArrayContent = new ByteArrayContent("furion"u8.ToArray());
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = byteArrayContent;

        var converter = new ByteArrayContentConverter();
        var bytes = converter.Read(httpResponseMessage);
        Assert.NotNull(bytes);
        Assert.Equal("furion", Encoding.UTF8.GetString(bytes));
    }

    [Fact]
    public async Task ReadAsync_ReturnOK()
    {
        using var byteArrayContent = new ByteArrayContent("furion"u8.ToArray());
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = byteArrayContent;

        var converter = new ByteArrayContentConverter();
        var bytes = await converter.ReadAsync(httpResponseMessage);
        Assert.NotNull(bytes);
        Assert.Equal("furion", Encoding.UTF8.GetString(bytes));
    }

    [Fact]
    public async Task ReadAsync_WithCancellationToken_ReturnOK()
    {
        using var byteArrayContent = new ByteArrayContent("furion"u8.ToArray());
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = byteArrayContent;

        using var cancellationTokenSource = new CancellationTokenSource();

        var converter = new ByteArrayContentConverter();
        var bytes = await converter.ReadAsync(httpResponseMessage, cancellationTokenSource.Token);
        Assert.NotNull(bytes);
        Assert.Equal("furion", Encoding.UTF8.GetString(bytes));
    }
}