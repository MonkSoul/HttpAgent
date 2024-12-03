﻿// 版权归百小僧及百签科技（广东）有限公司所有。
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

        Assert.Equal(4096, RateLimitedStream.CHUNK_SIZE);
        Assert.Equal(5, rateLimitedStream._bytesPerSecond);
        Assert.Equal(fileStream, rateLimitedStream._innerStream);
        Assert.NotNull(rateLimitedStream._lockObject);
        Assert.NotNull(rateLimitedStream._stopwatch);
        Assert.Equal(5, rateLimitedStream._availableTokens);
        Assert.Equal(0, rateLimitedStream._lastTokenRefillTime);

        Assert.Equal(fileStream.CanRead, rateLimitedStream.CanRead);
        Assert.Equal(fileStream.CanSeek, rateLimitedStream.CanSeek);
        Assert.Equal(fileStream.CanWrite, rateLimitedStream.CanWrite);
        Assert.Equal(fileStream.CanTimeout, rateLimitedStream.CanTimeout);
        Assert.Equal(21, rateLimitedStream.Length);
        Assert.Equal(fileStream.Position, rateLimitedStream.Position);
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
    public void Seek_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(filePath);
        using var rateLimitedStream = new RateLimitedStream(fileStream, 5);

        rateLimitedStream.Seek(0, SeekOrigin.Begin);
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
    public void Read_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream(testData);
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);
        var buffer = new byte[4096];
        var totalBytesRead = 0;

        while (totalBytesRead < testData.Length)
        {
            var bytesRead =
                rateLimitedStream.Read(buffer, 0, Math.Min(buffer.Length, testData.Length - totalBytesRead));
            totalBytesRead += bytesRead;
            Assert.InRange(bytesRead, 0, 4096);

            if (totalBytesRead < testData.Length)
            {
                Thread.Sleep(1000);
            }
        }

        Assert.Equal(testData.Length, totalBytesRead);
    }

    [Fact]
    public void Write_ReturnOK()
    {
        var testData = new byte[8192];
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 4096);

        rateLimitedStream.Write(testData, 0, testData.Length / 2);

        Thread.Sleep(1000);

        rateLimitedStream.Write(testData, testData.Length / 2, testData.Length / 2);

        Assert.Equal(testData.Length, memoryStream.Length);
    }

    [Fact]
    public void RefillTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);
        rateLimitedStream.RefillTokens();

        // 无
    }

    [Fact]
    public void WaitForTokens_ReturnOK()
    {
        using var memoryStream = new MemoryStream();
        using var rateLimitedStream = new RateLimitedStream(memoryStream, 1024);

        rateLimitedStream._availableTokens = 256.0;
        rateLimitedStream.WaitForTokens(512);

        // 无
    }
}