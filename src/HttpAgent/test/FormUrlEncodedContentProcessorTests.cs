﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FormUrlEncodedContentProcessorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var processor = new FormUrlEncodedContentProcessor();
        Assert.NotNull(processor);
        Assert.True(typeof(IHttpContentProcessor).IsAssignableFrom(typeof(FormUrlEncodedContentProcessor)));
    }

    [Fact]
    public void CanProcess_ReturnOK()
    {
        var processor = new FormUrlEncodedContentProcessor();

        Assert.False(processor.CanProcess(null, "application/octet-stream"));
        Assert.True(processor.CanProcess(null, "application/x-www-form-urlencoded"));
        Assert.True(processor.CanProcess(null, "Application/X-www-form-urlencoded"));
        Assert.False(processor.CanProcess(null, "application/json"));

        Assert.True(processor.CanProcess(new { }, "application/x-www-form-urlencoded"));
        Assert.True(processor.CanProcess(new FormUrlEncodedContent([]),
            "application/x-www-form-urlencoded"));
    }

    [Fact]
    public void Process_Invalid_Parameters()
    {
        var processor = new FormUrlEncodedContentProcessor();

        Assert.Throws<NotSupportedException>(() =>
        {
            processor.Process("furion", "application/x-www-form-urlencoded", null);
        });
    }

    [Fact]
    public void Process_ReturnOK()
    {
        var processor = new FormUrlEncodedContentProcessor();

        Assert.Null(processor.Process(null, "application/x-www-form-urlencoded", null));

        var formUrlEncodedContent =
            new FormUrlEncodedContent(new List<KeyValuePair<string, string>> { new("key", "value") });
        var httpContent1 = processor.Process(formUrlEncodedContent, "application/x-www-form-urlencoded", null);
        Assert.Same(formUrlEncodedContent, httpContent1);

        var httpContent2 = processor.Process(new { }, "application/x-www-form-urlencoded", null);
        Assert.NotNull(httpContent2);
        Assert.Equal(typeof(FormUrlEncodedContent), httpContent2.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent2.Headers.ContentType?.MediaType);
        Assert.Null(httpContent2.Headers.ContentType?.CharSet);

        var httpContent3 = processor.Process(new { }, "application/x-www-form-urlencoded", Encoding.UTF32);
        Assert.NotNull(httpContent3);
        Assert.Equal(typeof(FormUrlEncodedContent), httpContent3.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent3.Headers.ContentType?.MediaType);
        Assert.Equal("utf-32", httpContent3.Headers.ContentType?.CharSet);

        var httpContent4 = processor.Process(new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()),
            "application/x-www-form-urlencoded", null);
        Assert.NotNull(httpContent4);
        Assert.Equal(typeof(FormUrlEncodedContent), httpContent4.GetType());
        Assert.Equal("application/x-www-form-urlencoded", httpContent4.Headers.ContentType?.MediaType);
        Assert.Null(httpContent4.Headers.ContentType?.CharSet);
    }
}