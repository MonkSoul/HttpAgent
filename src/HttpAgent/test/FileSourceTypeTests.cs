// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileSourceTypeTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames<FileSourceType>();
        Assert.Equal(6, names.Length);

        string[] strings =
        [
            nameof(FileSourceType.None), nameof(FileSourceType.Path),
            nameof(FileSourceType.Base64String), nameof(FileSourceType.Remote), nameof(FileSourceType.Stream),
            nameof(FileSourceType.ByteArray)
        ];
        Assert.True(strings.SequenceEqual(names));
    }
}