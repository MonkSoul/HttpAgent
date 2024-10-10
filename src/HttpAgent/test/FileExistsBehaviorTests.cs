// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileExistsBehaviorTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames(typeof(FileExistsBehavior));
        Assert.Equal(3, names.Length);

        string[] strings =
        [
            nameof(FileExistsBehavior.CreateNew), nameof(FileExistsBehavior.Overwrite), nameof(FileExistsBehavior.Skip)
        ];
        Assert.True(strings.SequenceEqual(names));
    }
}