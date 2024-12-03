// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     带应用速率限制的流
/// </summary>
public sealed class RateLimitedStream : Stream
{
    /// <summary>
    ///     每秒允许的最大字节数
    /// </summary>
    private readonly int _bytesPerSecond;

    /// <inheritdoc cref="Stream" />
    internal readonly Stream _innerStream;

    /// <summary>
    ///     用于精确计时的 <see cref="Stopwatch" /> 实例
    /// </summary>
    internal readonly Stopwatch _stopwatch = new();

    /// <summary>
    ///     到目前为止已读取或写入的总字节数
    /// </summary>
    internal long _totalBytesProcessed;

    /// <summary>
    ///     <inheritdoc cref="RateLimitedStream" />
    /// </summary>
    /// <param name="innerStream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="bytesPerSecond">每秒允许的最大字节数</param>
    public RateLimitedStream(Stream innerStream, int bytesPerSecond)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(innerStream);

        // 小于或等于 0 检查
        if (bytesPerSecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytesPerSecond),
                "The bytes per second must be greater than zero.");
        }

        _innerStream = innerStream;
        _bytesPerSecond = bytesPerSecond;

        // 启动 Stopwatch 来开始计时
        _stopwatch.Start();
    }

    /// <inheritdoc />
    public override bool CanRead => _innerStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => _innerStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => _innerStream.CanWrite;

    /// <inheritdoc />
    public override bool CanTimeout => _innerStream.CanTimeout;

    /// <inheritdoc />
    public override long Length => _innerStream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    /// <inheritdoc />
    public override void Flush() => _innerStream.Flush();

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        // 根据设定的速率限制调整读写操作的速度
        ApplyRateLimitAsync(count).GetAwaiter().GetResult();

        // 从内部流读取数据到缓冲区
        return _innerStream.Read(buffer, offset, count);
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

    /// <inheritdoc />
    public override void SetLength(long value) => _innerStream.SetLength(value);

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        // 向内部流写入数据
        _innerStream.Write(buffer, offset, count);

        // 根据设定的速率限制调整读写操作的速度
        ApplyRateLimitAsync(count).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // 释放托管资源
        if (disposing)
        {
            _innerStream.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///     根据设定的速率限制调整读写操作的速度
    /// </summary>
    /// <param name="bytesToProcess">本次操作将处理的字节数</param>
    internal async Task ApplyRateLimitAsync(int bytesToProcess)
    {
        // 自开始以来经过的时间（秒）
        var elapsedSeconds = _stopwatch.ElapsedMilliseconds / 1000.0;

        // 根据速率预期应读取的字节数
        var totalBytesExpected = elapsedSeconds * _bytesPerSecond;

        // 计算实际与预期之差
        var bytesOverLimit = _totalBytesProcessed + bytesToProcess - totalBytesExpected;

        if (bytesOverLimit > 0)
        {
            // 如果实际操作超过预期，则计算需要等待的时间，并进行延迟
            var delayMilliseconds = (int)(bytesOverLimit / _bytesPerSecond * 1000.0);

            await Task.Delay(delayMilliseconds).ConfigureAwait(false);
        }

        // 更新已处理的总字节数
        _totalBytesProcessed += bytesToProcess;
    }
}