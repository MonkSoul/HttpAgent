// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MultipartFormDataItemTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new MultipartFormDataItem(null!));
        Assert.Throws<ArgumentException>(() => new MultipartFormDataItem(string.Empty));
        Assert.Throws<ArgumentException>(() => new MultipartFormDataItem(" "));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var multipartFormDataItem = new MultipartFormDataItem("name");

        Assert.Equal("name", multipartFormDataItem.Name);
        Assert.Null(multipartFormDataItem.ContentType);
        Assert.Null(multipartFormDataItem.ContentEncoding);
        Assert.Null(multipartFormDataItem.RawContent);
        Assert.Null(multipartFormDataItem.FileName);
        Assert.Null(multipartFormDataItem.FileSize);
    }
}