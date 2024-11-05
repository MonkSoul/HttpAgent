// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ReadOnlyMemoryContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var readOnlyMemoryContentProcessor = new ReadOnlyMemoryContentProcessor();
        Assert.NotNull(readOnlyMemoryContentProcessor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(ReadOnlyMemoryContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var readOnlyMemoryContentProcessor = new ReadOnlyMemoryContentProcessor();

        Assert.False(readOnlyMemoryContentProcessor.CanProcess(null, "application/octet-stream"));
        Assert.True(
            readOnlyMemoryContentProcessor.CanProcess(new ReadOnlyMemory<byte>([]), "application/octet-stream"));
        Assert.True(readOnlyMemoryContentProcessor.CanProcess(new ReadOnlyMemory<byte>(Array.Empty<byte>()),
            "application/octet-stream"));
        Assert.False(readOnlyMemoryContentProcessor.CanProcess(new FormUrlEncodedContent([]),
            "application/octet-stream"));
        Assert.False(readOnlyMemoryContentProcessor.CanProcess(new StringContent(""),
            "application/octet-stream"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var readOnlyMemoryContentProcessor = new ReadOnlyMemoryContentProcessor();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            readOnlyMemoryContentProcessor.Process(Array.Empty<byte>(), "application/octet-stream", null);
        });

        Assert.Equal("Expected a ReadOnlyMemory<byte>, but received an object of type `System.Byte[]`.",
            exception.Message);
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var readOnlyMemoryContentProcessor = new ReadOnlyMemoryContentProcessor();

        var readOnlyMemoryContent1 = readOnlyMemoryContentProcessor.Process(null, "application/octet-stream", null);
        Assert.Null(readOnlyMemoryContent1);

        var readOnlyMemoryContent2 =
            readOnlyMemoryContentProcessor.Process(new ReadOnlyMemory<byte>([]), "application/octet-stream", null);
        Assert.NotNull(readOnlyMemoryContent2);
        Assert.NotNull(readOnlyMemoryContent2.ReadAsStream());
        Assert.Equal("application/octet-stream", readOnlyMemoryContent2.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", readOnlyMemoryContent2.Headers.ContentType?.CharSet);

        var readOnlyMemoryContent3 =
            readOnlyMemoryContentProcessor.Process(new ReadOnlyMemory<byte>([]), "application/octet-stream",
                Encoding.UTF32);
        Assert.NotNull(readOnlyMemoryContent3);
        Assert.NotNull(readOnlyMemoryContent3.ReadAsStream());
        Assert.Equal("application/octet-stream", readOnlyMemoryContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", readOnlyMemoryContent3.Headers.ContentType?.CharSet);
    }
}