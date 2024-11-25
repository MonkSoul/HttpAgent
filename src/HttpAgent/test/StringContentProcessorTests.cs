// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StringContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new StringContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(StringContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new StringContentProcessor();

        Assert.True(processor.CanProcess(new StringContent(""), "text/plain"));
        Assert.True(processor.CanProcess(JsonContent.Create(new { id = 1, name = "Furion" }), "application/json"));
        Assert.True(processor.CanProcess(null, "application/json"));
        Assert.True(processor.CanProcess(null, "application/json-patch+json"));
        Assert.True(processor.CanProcess(null, "application/xml"));
        Assert.True(processor.CanProcess(null, "application/xml-patch+xml"));
        Assert.True(processor.CanProcess(null, "text/xml"));
        Assert.True(processor.CanProcess(null, "text/html"));
        Assert.True(processor.CanProcess(null, "text/plain"));
        Assert.True(processor.CanProcess(null, "Text/Plain"));

        Assert.False(processor.CanProcess(null, "application/x-www-form-urlencoded"));
        Assert.False(processor.CanProcess(null, "application/octet-stream"));
        Assert.False(processor.CanProcess(null, "multipart/form-data"));
        Assert.False(processor.CanProcess(null, "image/jpeg"));
        Assert.False(processor.CanProcess(null, "image/png"));

        Assert.False(processor.CanProcess(new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()),
            "application/x-www-form-urlencoded"));
    }

    [Fact]
    public async Task Process_ReturnOK()
    {
        var processor = new StringContentProcessor();

        Assert.Null(processor.Process(null, "application/json", null));

        var stringContent = new StringContent("Hello World");
        var httpContent1 = processor.Process(stringContent, "application/json", null);
        Assert.Same(stringContent, httpContent1);

        var httpContent2 = processor.Process("furion", "text/plain", null);
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(StringContent), httpContent2.GetType());
        Assert.Equal("furion", await httpContent2.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent2.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent2.Headers.ContentType?.CharSet);

        var httpContent3 = processor.Process(new { id = 10, name = "furion" }, "application/json", Encoding.UTF32);
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(StringContent), httpContent3.GetType());
        Assert.Equal("{\"id\":10,\"name\":\"furion\"}", await httpContent3.ReadAsStringAsync());
        Assert.Equal("application/json", httpContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent3.Headers.ContentType?.CharSet);

        var httpContent4 = processor.Process(10, "text/plain", null);
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(StringContent), httpContent4.GetType());
        Assert.Equal("10", await httpContent4.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent4.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent4.Headers.ContentType?.CharSet);

        var httpContent5 = processor.Process(EnumModel.Default, "text/plain", null);
        Assert.NotNull(httpContent5);
        Assert.Equal(typeof(StringContent), httpContent5.GetType());
        Assert.Equal("1", await httpContent5.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent5.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent5.Headers.ContentType?.CharSet);

        using var jsonDocument = JsonDocument.Parse("\"furion\"");
        var httpContent6 = processor.Process(jsonDocument.RootElement, "application/json",
            Encoding.UTF32);
        Assert.NotNull(httpContent6);
        Assert.Equal(typeof(StringContent), httpContent6.GetType());
        Assert.Equal("furion", await httpContent6.ReadAsStringAsync());
        Assert.Equal("application/json", httpContent6.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent6.Headers.ContentType?.CharSet);

        var httpContent7 = processor.Process(new StringContent("furion"), "text/plain", null);
        Assert.NotNull(httpContent7);
        Assert.Equal(typeof(StringContent), httpContent7.GetType());
        Assert.Equal("furion", await httpContent7.ReadAsStringAsync());
        Assert.Equal("text/plain", httpContent7.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", httpContent7.Headers.ContentType?.CharSet);
    }
}