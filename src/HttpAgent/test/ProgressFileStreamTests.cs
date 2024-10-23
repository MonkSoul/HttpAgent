// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ProgressFileStreamTests(ITestOutputHelper output)
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileInfo = new FileInfo(fileFullName);
        using var fileStream = File.OpenRead(fileFullName);

        Assert.Throws<ArgumentNullException>(() => new ProgressFileStream(null!, null!, fileInfo.Length, null!));
        Assert.Throws<ArgumentNullException>(() => new ProgressFileStream(fileStream, null!, fileInfo.Length, null!));
        Assert.Throws<ArgumentException>(() =>
            new ProgressFileStream(fileStream, string.Empty, fileInfo.Length, null!));
        Assert.Throws<ArgumentException>(() => new ProgressFileStream(fileStream, " ", fileInfo.Length, null!));
        Assert.Throws<ArgumentNullException>(
            () => new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        Assert.Equal(21, progressFileStream._fileLength);
        Assert.Equal(fileStream, progressFileStream._fileStream);
        Assert.NotNull(progressFileStream._fileTransferProgress);
        Assert.Equal(21, progressFileStream._fileTransferProgress.TotalFileSize);
        Assert.Equal("test.txt", progressFileStream._fileTransferProgress.FileName);
        Assert.Equal(fileFullName, progressFileStream._fileTransferProgress.FileFullName);
        Assert.NotNull(progressFileStream._stopwatch);
        Assert.Equal(0, progressFileStream._transferred);
        Assert.Equal(progressChannel, progressFileStream._progressChannel);

        Assert.Equal(fileStream.CanRead, progressFileStream.CanRead);
        Assert.Equal(fileStream.CanSeek, progressFileStream.CanSeek);
        Assert.Equal(fileStream.CanWrite, progressFileStream.CanWrite);
        Assert.Equal(fileStream.CanTimeout, progressFileStream.CanTimeout);
        Assert.Equal(21, progressFileStream.Length);
        Assert.Equal(fileStream.Position, progressFileStream.Position);
    }

    [Fact]
    public void Flush_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        progressFileStream.Flush();
    }

    [Fact]
    public async Task Read_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        await using var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        await using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        var bytes = new byte[1024];
        var bytesRead = progressFileStream.Read(bytes, 0, 10);
        Assert.True(bytesRead > 0);

        await foreach (var fileTransferProgress in progressChannel.Reader.ReadAllAsync())
        {
            Assert.NotNull(fileTransferProgress);
            output.WriteLine(fileTransferProgress.FileName);

            break;
        }
    }

    [Fact]
    public void Seek_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        progressFileStream.Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public async Task Write_ReturnOK()
    {
        var fileFullName = Path.GetTempFileName();
        await using var fileStream = new FileStream(fileFullName, FileMode.Create, FileAccess.Write,
            FileShare.Read,
            8092);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        await using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        var bytes = new byte[1024];
        progressFileStream.Write(bytes, 0, 21);

        await foreach (var fileTransferProgress in progressChannel.Reader.ReadAllAsync())
        {
            Assert.NotNull(fileTransferProgress);
            output.WriteLine(fileTransferProgress.FileName);

            break;
        }
    }

    [Fact]
    public void SetLength_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        using var fileStream = new FileStream(fileFullName, FileMode.Open, FileAccess.Write, FileShare.Read);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        progressFileStream.SetLength(21);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        var progressFileStream = new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        progressFileStream.Dispose();
        Assert.False(fileStream.CanRead);
        Assert.False(progressFileStream._stopwatch.IsRunning);
    }

    [Fact]
    public async Task ReportProgress_ReturnOK()
    {
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        await using var fileStream = File.OpenRead(fileFullName);
        var fileInfo = new FileInfo(fileFullName);
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        await using var progressFileStream =
            new ProgressFileStream(fileStream, fileFullName, fileInfo.Length, progressChannel);

        progressFileStream.ReportProgress(10);

        await foreach (var fileTransferProgress in progressChannel.Reader.ReadAllAsync())
        {
            Assert.NotNull(fileTransferProgress);
            output.WriteLine(fileTransferProgress.FileName);

            break;
        }
    }
}