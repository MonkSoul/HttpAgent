// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ObjectContentConverterFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var factory = new ObjectContentConverterFactory();
        Assert.NotNull(factory);
    }

    [Fact]
    public void GetConverter_ReturnOK()
    {
        var factory = new ObjectContentConverterFactory();
        Assert.True(factory.GetConverter(typeof(object)).GetType() == typeof(ObjectContentConverter));
        Assert.True(factory.GetConverter<object>().GetType() == typeof(ObjectContentConverter<object>));
    }
}