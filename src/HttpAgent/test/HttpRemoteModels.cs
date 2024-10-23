// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public enum EnumModel
{
    None, Default
}

public class CustomStringContentProcessor : StringContentProcessor;

public class CustomStringContentProcessor2 : StringContentProcessor;

public class CustomStringContentConverter : StringContentConverter;

public class CustomByteArrayContentConverter : ByteArrayContentConverter;

public class ObjectModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class CustomObjectContentConverter<TResult> : ObjectContentConverter<TResult>;

public class CustomObjectContentConverter : ObjectContentConverter;

public class NotImplementObjectContentConverterFactory;

public class CustomObjectContentConverterFactory : IObjectContentConverterFactory
{
    /// <inheritdoc />
    public ObjectContentConverter<TResult> GetConverter<TResult>() => new CustomObjectContentConverter<TResult>();

    /// <inheritdoc />
    public ObjectContentConverter GetConverter(Type resultType) => new CustomObjectContentConverter();
}

public class MultipartModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class NotImplementRequestEventHandler;

public class CustomRequestEventHandler : IHttpRequestEventHandler
{
    public int counter;

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => counter++;

    /// <inheritdoc />
    public void OnPostSendRequest(HttpResponseMessage httpResponseMessage) => counter++;

    /// <inheritdoc />
    public void OnSendRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) => counter++;
}

public class NotImplementFileDownloadEventHandler;

public class CustomFileTransferEventHandler : IHttpFileTransferEventHandler
{
    public int counter;
    public int counter2;

    /// <inheritdoc />
    public void OnTransferStarted() => counter++;

    /// <inheritdoc />
    public void OnTransferCompleted(long duration) => counter++;

    /// <inheritdoc />
    public void OnTransferFailed(Exception exception) => counter++;

    /// <inheritdoc />
    public Task OnProgressChangedAsync(FileTransferProgress fileTransferProgress)
    {
        counter2++;
        return Task.CompletedTask;
    }
}

public class CustomFileTransferEventHandler2 : IHttpFileTransferEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnTransferStarted() => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnTransferCompleted(long duration) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnTransferFailed(Exception exception) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnProgressChangedAsync(FileTransferProgress fileTransferProgress) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostSendRequest(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnSendRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();
}

public class NotImplementServerSentEventsEventHandler;

public class CustomServerSentEventsEventHandler : IHttpServerSentEventsEventHandler
{
    public int counter;

    /// <inheritdoc />
    public void OnOpen() => counter++;

    /// <inheritdoc />
    public Task OnMessageAsync(ServerSentEventsData serverSentEventsData)
    {
        counter++;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void OnError(Exception exception) => counter++;
}

public class CustomServerSentEventsEventHandler2 : IHttpServerSentEventsEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostSendRequest(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnSendRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public void OnOpen() => throw new NotImplementedException();

    /// <inheritdoc />
    public Task OnMessageAsync(ServerSentEventsData serverSentEventsData) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnError(Exception exception) => throw new NotImplementedException();
}

public class NotLongPollingEventHandler;

public class CustomLongPollingEventHandler : IHttpLongPollingEventHandler
{
    public int counter;

    /// <inheritdoc />
    public Task OnDataReceivedAsync(HttpResponseMessage httpResponseMessage)
    {
        counter++;
        return Task.CompletedTask;
    }
}

public class CustomLongPollingEventHandler2 : IHttpLongPollingEventHandler, IHttpRequestEventHandler
{
    /// <inheritdoc />
    public Task OnDataReceivedAsync(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnPostSendRequest(HttpResponseMessage httpResponseMessage) => throw new NotImplementedException();

    /// <inheritdoc />
    public void OnSendRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) =>
        throw new NotImplementedException();
}

[MessagePackObject]
public class MessagePackModel1
{
    [Key(0)] public int Id { get; set; }

    [Key(1)] public string? Name { get; set; }
}