// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RateLimitedStreamTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new RateLimitedStream(null!, 0));
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new RateLimitedStream(new MemoryStream(), -1));
        Assert.Equal("The bytes per second must be greater than zero. (Parameter 'bytesPerSecond')", exception.Message);

        var exception2 = Assert.Throws<ArgumentOutOfRangeException>(() => new RateLimitedStream(new MemoryStream(), 0));
        Assert.Equal("The bytes per second must be greater than zero. (Parameter 'bytesPerSecond')",
            exception2.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        Assert.Equal(fileStream, rateLimitedStream._innerStream);
        Assert.Equal(0, rateLimitedStream._totalBytesProcessed);
        Assert.Equal(fileStream.CanRead, rateLimitedStream.CanRead);
        Assert.Equal(fileStream.CanSeek, rateLimitedStream.CanSeek);
        Assert.Equal(fileStream.CanWrite, rateLimitedStream.CanWrite);
        Assert.Equal(fileStream.CanTimeout, rateLimitedStream.CanTimeout);
        Assert.Equal(21, rateLimitedStream.Length);
        Assert.Equal(fileStream.Position, rateLimitedStream.Position);
        Assert.NotNull(rateLimitedStream._stopwatch);
    }

    [Fact]
    public void Flush_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Flush();
    }

    [Fact]
    public void Read_ReturnOK()
    {
        var data = new byte[1024 * 1024];
        new Random().NextBytes(data);

        using var memoryStream = new MemoryStream();
        memoryStream.Write(data, 0, data.Length);
        memoryStream.Position = 0;

        const int bytesPerSecond = 1024 * 1024;
        using var rateLimitedStream = new RateLimitedStream(memoryStream, bytesPerSecond);

        var stopwatch = Stopwatch.StartNew();
        var buffer = new byte[1024 * 1024];
        var bytesRead = rateLimitedStream.Read(buffer, 0, buffer.Length);
        stopwatch.Stop();

        Assert.Equal(data.Length, bytesRead);

        Assert.InRange(stopwatch.ElapsedMilliseconds, 900, 1100);
        Assert.True(data.SequenceEqual(buffer));
    }


    [Fact]
    public void Seek_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var data = new byte[1024 * 1024];
        new Random().NextBytes(data);

        using var memoryStream = new MemoryStream();
        const int bytesPerSecond = 1024 * 1024;
        using var rateLimitedStream = new RateLimitedStream(memoryStream, bytesPerSecond);

        var stopwatch = Stopwatch.StartNew();
        rateLimitedStream.Write(data, 0, data.Length);
        stopwatch.Stop();

        Assert.InRange(stopwatch.ElapsedMilliseconds, 900, 1100);

        memoryStream.Position = 0;
        var verificationBuffer = new byte[data.Length];
        _ = memoryStream.Read(verificationBuffer, 0, verificationBuffer.Length);
        Assert.True(data.SequenceEqual(verificationBuffer));
    }

    [Fact]
    public void SetLength_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test6.txt");
        using var fileStream = File.OpenWrite(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.SetLength(21);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileStream = File.OpenRead(filePath);
        var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Dispose();
        Assert.False(fileStream.CanRead);
    }

    [Fact]
    public async Task ApplyRateLimitAsync_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        await using var fileStream = File.OpenRead(filePath);
        await using var rateLimitedStream = new RateLimitedStream(fileStream, 2);

        await rateLimitedStream.ApplyRateLimitAsync(21);
    }
}