// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HelpersTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("http://localhost", "")]
    [InlineData("http://localhost/test", "")]
    [InlineData("http://localhost/test.pdf", "test.pdf")]
    [InlineData("http://localhost/test.pdf?id=10&name=furion", "test.pdf")]
    [InlineData(
        "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe",
        "Installer_迅捷屏幕录像工具_1.7.9_123.exe")]
    public void GetFileNameFromUri(string? url, string? fileName) =>
        Assert.Equal(fileName,
            HttpAgent.Helpers.GetFileNameFromUri(string.IsNullOrWhiteSpace(url)
                ? null
                : new Uri(url, UriKind.RelativeOrAbsolute)));

    [Fact]
    public void ParseHttpMethod_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.ParseHttpMethod(null));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.ParseHttpMethod(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.ParseHttpMethod(" "));
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("Connect")]
    [InlineData("Options")]
    [InlineData("delete")]
    [InlineData("trace")]
    [InlineData("Unknown")]
    [InlineData("HEAD")]
    [InlineData("PUT")]
    public void ParseHttpMethod_ReturnOK(string httpMethod) => HttpAgent.Helpers.ParseHttpMethod(httpMethod);

    [Fact]
    public void IsFormUrlEncodedString_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(null!));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpAgent.Helpers.IsFormUrlEncodedFormat(" "));
    }

    [Theory]
    [InlineData("furion", false)]
    [InlineData("id=1&name=furion", true)]
    [InlineData("id=1", true)]
    [InlineData("name=furion", true)]
    [InlineData("key=value%20with%20space", true)]
    [InlineData("invalid&key", false)]
    [InlineData("key value", false)]
    [InlineData("=value", false)]
    [InlineData("key=", true)]
    [InlineData("key%3Dvalue", false)]
    public void IsFormUrlEncodedFormat_ReturnOK(string output, bool result) =>
        Assert.Equal(result, HttpAgent.Helpers.IsFormUrlEncodedFormat(output));
}