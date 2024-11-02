// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MessagePackContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var messagePackContentProcessor = new MessagePackContentProcessor();
        Assert.NotNull(messagePackContentProcessor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(MessagePackContentProcessor)));

        Assert.NotNull(MessagePackContentProcessor._messagePackSerializerLazy);
        Assert.NotNull(MessagePackContentProcessor._messagePackSerializerLazy.Value);
        Assert.NotNull(messagePackContentProcessor._messagePackSerializer);
        Assert.Equal(MessagePackContentProcessor._messagePackSerializerLazy.Value,
            messagePackContentProcessor._messagePackSerializer);
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var messagePackContentProcessor = new MessagePackContentProcessor();

        Assert.True(messagePackContentProcessor.CanProcess(null, "application/msgpack"));
        Assert.True(messagePackContentProcessor.CanProcess(Array.Empty<byte>(), "application/msgpack"));
        Assert.True(messagePackContentProcessor.CanProcess(Array.Empty<byte>(), "Application/Msgpack"));
        Assert.True(messagePackContentProcessor.CanProcess(new MessagePackModel1(), "Application/Msgpack"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var messagePackContentProcessor = new MessagePackContentProcessor();

        Assert.Throws<TargetInvocationException>(() =>
            messagePackContentProcessor.Process(new MessagePackModel0(), "application/msgpack", null));
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var messagePackContentProcessor = new MessagePackContentProcessor();

        var messagePackContent1 = messagePackContentProcessor.Process(null, "application/msgpack", null);
        Assert.Null(messagePackContent1);

        var messagePackContent2 =
            messagePackContentProcessor.Process(Array.Empty<byte>(), "application/msgpack", null);
        Assert.NotNull(messagePackContent2);
        Assert.NotNull(messagePackContent2.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent2.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", messagePackContent2.Headers.ContentType?.CharSet);

        var messagePackContent3 =
            messagePackContentProcessor.Process(Array.Empty<byte>(), "application/msgpack", Encoding.UTF32);
        Assert.NotNull(messagePackContent3);
        Assert.NotNull(messagePackContent3.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", messagePackContent3.Headers.ContentType?.CharSet);

        var messagePackContent4 =
            messagePackContentProcessor.Process(new MessagePackModel1(), "application/msgpack",
                null);
        Assert.NotNull(messagePackContent4);
        Assert.NotNull(messagePackContent4.ReadAsStream());
        Assert.Equal("application/msgpack", messagePackContent4.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", messagePackContent4.Headers.ContentType?.CharSet);
    }
}