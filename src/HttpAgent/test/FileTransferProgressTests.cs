﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileTransferProgressTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new FileTransferProgress(null!, 0));
        Assert.Throws<ArgumentException>(() =>
            new FileTransferProgress(string.Empty, 0));
        Assert.Throws<ArgumentException>(() => new FileTransferProgress(" ", 0));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);

        Assert.NotNull(fileTransferProgress.FilePath);
        Assert.Equal(@"C:\Workspaces\furion.index.html", fileTransferProgress.FilePath);
        Assert.Equal(1000L, fileTransferProgress.TotalFileSize);

        Assert.NotNull(fileTransferProgress.FileName);
        Assert.Equal("furion.index.html", fileTransferProgress.FileName);

        Assert.Equal(double.Epsilon, FileTransferProgress._epsilon);
        Assert.Equal(0, fileTransferProgress.Transferred);
        Assert.Equal(0, fileTransferProgress.PercentageComplete);
        Assert.Equal(0, fileTransferProgress.TransferRate);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress.TimeElapsed);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress.EstimatedTimeRemaining);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        Assert.Equal(
            "Transfer Progress: \r\n\tFile Name:                        furion.index.html\r\n\tFile Path:                        C:\\Workspaces\\furion.index.html\r\n\tTotal File Size:                  0.95 MB\r\n\tTransferred:                      0.48 MB\r\n\tPercentage Complete:              50.00%\r\n\tTransfer Rate:                    2.38 MB/s\r\n\tTime Elapsed (s):                 0.20\r\n\tEstimated Time Remaining (s):     0.20",
            fileTransferProgress.ToString());
    }

    [Fact]
    public void ToSummaryString_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000000L);
        fileTransferProgress.UpdateProgress(500000, TimeSpan.FromMilliseconds(200));

        Assert.Equal(
            @"Transferred 0.48 MB of 0.95 MB (50.00% complete, Speed: 2.38 MB/s, Time: 0.20s, ETA: 0.20s), File: furion.index.html, Path: C:\Workspaces\furion.index.html.",
            fileTransferProgress.ToSummaryString());
    }

    [Fact]
    public void UpdateProgress_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(500, fileTransferProgress.Transferred);
        Assert.Equal(TimeSpan.FromMilliseconds(200), fileTransferProgress.TimeElapsed);
        Assert.Equal(50, fileTransferProgress.PercentageComplete);
        Assert.Equal(2500, fileTransferProgress.TransferRate);
        Assert.Equal(0.2, fileTransferProgress.EstimatedTimeRemaining.TotalSeconds);

        var fileTransferProgress2 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress2.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(500, fileTransferProgress2.Transferred);
        Assert.Equal(TimeSpan.FromMilliseconds(200), fileTransferProgress2.TimeElapsed);
        Assert.Equal(-1, fileTransferProgress2.PercentageComplete);
        Assert.Equal(2500, fileTransferProgress2.TransferRate);
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress2.EstimatedTimeRemaining);

        var fileTransferProgress3 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress3.UpdateProgress(500, TimeSpan.Zero);

        Assert.Equal(500, fileTransferProgress3.Transferred);
        Assert.Equal(TimeSpan.Zero, fileTransferProgress3.TimeElapsed);
        Assert.Equal(-1, fileTransferProgress3.PercentageComplete);
        Assert.Equal(0, fileTransferProgress3.TransferRate);
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress3.EstimatedTimeRemaining);
    }

    [Fact]
    public void CalculateEstimatedTimeRemaining_ReturnOK()
    {
        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", -1);
        fileTransferProgress.UpdateProgress(500, TimeSpan.FromMilliseconds(200));
        Assert.Equal(TimeSpan.MaxValue, fileTransferProgress.CalculateEstimatedTimeRemaining());

        var fileTransferProgress2 =
            new FileTransferProgress(@"C:\Workspaces\furion.index.html", 1000L);
        fileTransferProgress2.UpdateProgress(500, TimeSpan.FromMilliseconds(200));

        Assert.Equal(0.2, fileTransferProgress2.CalculateEstimatedTimeRemaining().TotalSeconds);
    }
}