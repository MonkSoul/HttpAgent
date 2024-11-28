﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StreamContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new StreamContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(StreamContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new StreamContentProcessor();
        using var stream = new MemoryStream();

        Assert.False(processor.CanProcess(null, "application/octet-stream"));
        Assert.True(processor.CanProcess(stream, "Application/Octet-stream"));
        Assert.True(processor.CanProcess(stream, "application/json"));
        Assert.True(processor.CanProcess(stream, "application/octet-stream"));
        Assert.True(processor.CanProcess(new StreamContent(stream), "application/octet-stream"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new StreamContentProcessor();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            processor.Process("furion", "application/octet-stream", null);
        });

        Assert.Equal("Expected a stream, but received an object of type `System.String`.", exception.Message);
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new StreamContentProcessor();

        var streamContent1 = processor.Process(null, "application/octet-stream", null);
        Assert.Null(streamContent1);

        using var stream = new MemoryStream();

        var streamContent2 = processor.Process(stream, "application/octet-stream", null);
        Assert.NotNull(streamContent2);
        Assert.NotNull(streamContent2.ReadAsStream());
        Assert.Equal("application/octet-stream", streamContent2.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", streamContent2.Headers.ContentType?.CharSet);

        var streamContent3 = processor.Process(stream, "application/octet-stream", Encoding.UTF32);
        Assert.NotNull(streamContent3);
        Assert.NotNull(streamContent3.ReadAsStream());
        Assert.Equal("application/octet-stream", streamContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", streamContent3.Headers.ContentType?.CharSet);

        var streamContent4 =
            processor.Process(new StreamContent(stream), "application/octet-stream", null);
        Assert.NotNull(streamContent4);
        Assert.NotNull(streamContent4.ReadAsStream());
        Assert.Equal("application/octet-stream", streamContent4.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", streamContent4.Headers.ContentType?.CharSet);
    }
}