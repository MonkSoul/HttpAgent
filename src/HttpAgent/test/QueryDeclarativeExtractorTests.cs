// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class QueryDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(QueryDeclarativeExtractor)));

        var extractor = new QueryDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        
    }
}

// [Query("header1", "value1")]
// [Query("header2", "value2")]
// [Query("header3")]
// public interface IQueryDeclarativeTest : IHttpDeclarativeExtractor
// {
//     [Post("http://localhost:5000")]
//     [Query("header4")]
//     Task Test1();
//
//     [Post("http://localhost:5000")]
//     [Query("header3", "value3")]
//     Task Test2();
//
//     [Post("http://localhost:5000")]
//     [Query("header2", "value21")]
//     [Query("header4", "value4")]
//     Task Test3();
//
//     [Post("http://localhost:5000")]
//     [Query("header3", "value3")]
//     Task Test4([Query] int id, [Query] [Query(AliasAs = "myName")] string name, [Query(Values = 30)] int? age);
// }