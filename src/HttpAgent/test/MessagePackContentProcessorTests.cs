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

        Assert.NotNull(MessagePackContentProcessor._serializerCache);
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

    [Fact]
    public void CreateSerializerDelegate_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => MessagePackContentProcessor.CreateSerializerDelegate(null!));

    [Fact]
    public void CreateSerializerDelegate_ReturnOK()
    {
        Assert.Throws<ArgumentNullException>(() => MessagePackContentProcessor.CreateSerializerDelegate(null!));

        var messagePackSerializerType = Type.GetType("MessagePack.MessagePackSerializer, MessagePack")!;
        var serializeMethod = messagePackSerializerType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .SingleOrDefault(u =>
                u is { Name: "Serialize", IsGenericMethod: true } && u.ReturnType == typeof(byte[]) &&
                u.GetParameters().Length == 3 &&
                u.GetGenericArguments().Length == 1)!;

        var func = MessagePackContentProcessor.CreateSerializerDelegate(serializeMethod);
        Assert.NotNull(func);
        var bytes = func(new MessagePackModel1());
        Assert.NotNull(bytes);
        Assert.Single(MessagePackContentProcessor._serializerCache);

        var func1 = MessagePackContentProcessor.CreateSerializerDelegate(serializeMethod);
        Assert.NotNull(func1);
        var bytes1 = func1(new MessagePackModel1());
        Assert.NotNull(bytes1);
        Assert.Single(MessagePackContentProcessor._serializerCache);
    }
}