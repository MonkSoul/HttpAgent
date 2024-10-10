// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ServerSentEventsDataTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var serverSentEventsData = new ServerSentEventsData();

        Assert.NotNull(serverSentEventsData._dataBuffer);
        Assert.Equal(0, serverSentEventsData._dataBuffer.Length);
        Assert.Null(serverSentEventsData._cachedData);
        Assert.Null(serverSentEventsData.Event);
        Assert.NotNull(serverSentEventsData.Data);
        Assert.Empty(serverSentEventsData.Data);
        Assert.Null(serverSentEventsData.Id);
    }

    [Fact]
    public void AppendData_ReturnOK()
    {
        var serverSentEventsData = new ServerSentEventsData();
        serverSentEventsData.AppendData(null);
        Assert.Null(serverSentEventsData._cachedData);
        Assert.Equal(string.Empty, serverSentEventsData.Data);
        Assert.NotNull(serverSentEventsData._cachedData);

        serverSentEventsData.AppendData("furion");
        Assert.Null(serverSentEventsData._cachedData);
        Assert.Equal("furion", serverSentEventsData.Data);
        Assert.NotNull(serverSentEventsData._cachedData);
    }
}