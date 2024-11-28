﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpMultipartFormDataBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpMultipartFormDataBuilder(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.NotNull(builder._httpRequestBuilder);
        Assert.NotNull(builder._partContents);
        Assert.Empty(builder._partContents);
        Assert.Null(builder.Boundary);
    }

    [Fact]
    public void SetBoundary_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.SetBoundary(null!));
        Assert.Throws<ArgumentException>(() => builder.SetBoundary(string.Empty));
        Assert.Throws<ArgumentException>(() => builder.SetBoundary(" "));
    }

    [Fact]
    public void SetBoundary_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.SetBoundary("--------------------");
        Assert.NotNull(builder.Boundary);
        Assert.Equal("--------------------", builder.Boundary);

        builder.Boundary = "x-----------x";
    }

    [Fact]
    public void AddJson_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddJson(null!));
        try
        {
            builder.AddJson("{\"id\":1,\"name\":\"furion\"");
        }
        catch (Exception e)
        {
            Assert.Equal("JsonReaderException", e.GetType().Name);
        }
    }

    [Fact]
    public void AddJson_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        builder.AddJson(new { id = 1, name = "furion" });
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("id", builder._partContents[0].Name);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Equal(1, builder._partContents[0].RawContent);
        Assert.Equal("name", builder._partContents[1].Name);
        Assert.Equal("text/plain", builder._partContents[1].ContentType);
        Assert.Null(builder._partContents[1].ContentEncoding);
        Assert.Equal("furion", builder._partContents[1].RawContent);

        var builder2 = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder2.AddJson("{\"id\":1,\"name\":\"furion\"}");
        Assert.Equal(2, builder2._partContents.Count);
        Assert.Equal("id", builder2._partContents[0].Name);
        Assert.Equal("text/plain", builder2._partContents[0].ContentType);
        Assert.Null(builder2._partContents[0].ContentEncoding);
        Assert.Equal("1", builder2._partContents[0].RawContent?.ToString());
        Assert.Equal("name", builder2._partContents[1].Name);
        Assert.Equal("text/plain", builder2._partContents[1].ContentType);
        Assert.Null(builder2._partContents[1].ContentEncoding);
        Assert.Equal("furion", builder2._partContents[1].RawContent?.ToString());
        Assert.NotNull(builder2._httpRequestBuilder.Disposables);
        Assert.Single(builder2._httpRequestBuilder.Disposables);
        Assert.True(builder2._httpRequestBuilder.Disposables[0] is JsonDocument);

        builder.AddJson(new { id = 1, name = "furion" }, "child");
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("child", builder._partContents[2].Name);
        Assert.Equal("application/json", builder._partContents[2].ContentType);
        Assert.Null(builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);

        var builder3 = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder3.AddJson("{\"id\":1,\"name\":\"furion\"}", "obj");
        Assert.Single(builder3._partContents);
        Assert.Equal("obj", builder3._partContents[0].Name);
        Assert.Equal("application/json", builder3._partContents[0].ContentType);
        Assert.Null(builder3._partContents[0].ContentEncoding);
        Assert.Equal("{\"id\":1,\"name\":\"furion\"}", builder3._partContents[0].RawContent);
    }

    [Fact]
    public void AddFormItem_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddFormItem(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFormItem(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFormItem(null, " "));
    }

    [Fact]
    public void AddFormItem_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddFormItem("furion", "name", Encoding.UTF8);
        Assert.Single(builder._partContents);
        Assert.Equal("name", builder._partContents[0].Name);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[0].ContentEncoding);
        Assert.Equal("furion", builder._partContents[0].RawContent);
    }

    [Fact]
    public void AddHtml_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddHtml(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddHtml(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddHtml(null, " "));
    }

    [Fact]
    public void AddHtml_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddHtml("<html><head></head><body></body></html>", "test", Encoding.UTF8);
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("text/html", builder._partContents[0].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[0].ContentEncoding);
        Assert.Equal("<html><head></head><body></body></html>", builder._partContents[0].RawContent);
    }

    [Fact]
    public void AddXml_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddXml(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddXml(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddXml(null, " "));
    }

    [Fact]
    public void AddXml_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "test", Encoding.UTF8);
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("application/xml", builder._partContents[0].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[0].ContentEncoding);
        Assert.Equal("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", builder._partContents[0].RawContent);
    }

    [Fact]
    public void AddText_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddText(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddText(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddText(null, " "));
    }

    [Fact]
    public void AddText_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddText("furion", "test", Encoding.UTF8);
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[0].ContentEncoding);
        Assert.Equal("furion", builder._partContents[0].RawContent);
    }

    [Fact]
    public void AddObject_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        // content-type 为空情况
        Assert.Throws<ArgumentNullException>(() => builder.AddObject(null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddObject(null!, null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddObject(null!, null, " "));

        // rawObject  为空情况
        Assert.Throws<ArgumentNullException>(() => builder.AddObject(null, null, "application/json"));

        // 不能转换为字典情况
        Assert.Throws<NotSupportedException>(() => builder.AddObject("", null, "application/json"));
    }

    [Fact]
    public void AddObject_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        builder.AddObject(null, "name");
        Assert.Single(builder._partContents);
        Assert.Equal("name", builder._partContents[0].Name);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Null(builder._partContents[0].RawContent);

        builder.AddObject(new { }, "name");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[1].Name);
        Assert.Equal("text/plain", builder._partContents[1].ContentType);
        Assert.Null(builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);

        builder.AddObject(new { }, "name", "application/json");
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[2].Name);
        Assert.Equal("application/json", builder._partContents[2].ContentType);
        Assert.Null(builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);

        builder.AddObject(new { }, "name", "application/json;charset=utf-8");
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[3].Name);
        Assert.Equal("application/json", builder._partContents[3].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[3].ContentEncoding);
        Assert.NotNull(builder._partContents[3].RawContent);

        builder.AddObject(new MultipartModel(), "name", "application/json;charset=utf-8", Encoding.UTF32);
        Assert.Equal(5, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[4].Name);
        Assert.Equal("application/json", builder._partContents[4].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[4].ContentEncoding);
        Assert.NotNull(builder._partContents[4].RawContent);

        builder.AddObject(new MultipartModel { Id = 1, Name = "furion" }, null, "application/json", Encoding.UTF8);
        Assert.Equal(7, builder._partContents.Count);

        Assert.Equal("Id", builder._partContents[5].Name);
        Assert.Equal(Encoding.UTF8, builder._partContents[5].ContentEncoding);
        Assert.Equal("text/plain", builder._partContents[5].ContentType);
        Assert.NotNull(builder._partContents[5].RawContent);
        Assert.Equal(1, builder._partContents[5].RawContent);

        Assert.Equal("Name", builder._partContents[6].Name);
        Assert.Equal("text/plain", builder._partContents[6].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[6].ContentEncoding);
        Assert.NotNull(builder._partContents[6].RawContent);
        Assert.Equal("furion", builder._partContents[6].RawContent);
    }

    [Fact]
    public void AddFileFromRemote_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        const string url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";

        // url 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(null!, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(" ", null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(url, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, " ", null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(url, "test", null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, "test", null, " "));
    }

    [Fact]
    public void AddFileFromRemote_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        const string url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";

        builder.AddFileFromRemote(url, "test");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("application/octet-stream", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.Equal("ContentLengthReadStream", builder._partContents[0].RawContent?.GetType().Name);
        Assert.Equal("Installer_迅捷屏幕录像工具_1.7.9_123.exe",
            builder._partContents[0].FileName);
        Assert.Equal(2785992, builder._partContents[0].FileSize);
        Assert.NotNull(builder._httpRequestBuilder.Disposables);
        Assert.Single(builder._httpRequestBuilder.Disposables);
        Assert.Equal("ContentLengthReadStream", builder._httpRequestBuilder.Disposables[0].GetType().Name);

        builder.AddFileFromRemote(url, "test", "test.exe");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test.exe", builder._partContents[1].FileName);
    }

    [Fact]
    public void AddFileFromBase64String_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var base64String =
            Convert.ToBase64String(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test.txt")));

        // url 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromBase64String(null!, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(" ", null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromBase64String(base64String, null!, null!, null!));
        Assert.Throws<ArgumentException>(
            () => builder.AddFileFromBase64String(base64String, string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(base64String, " ", null!, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFileFromBase64String(base64String, "test", null, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileFromBase64String(base64String, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(base64String, "test", null, " "));
    }

    [Fact]
    public void AddFileFromBase64String_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var base64String =
            Convert.ToBase64String(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test.txt")));

        builder.AddFileFromBase64String(base64String, "test", "test.txt");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("application/octet-stream", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.True(builder._partContents[0].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[0].FileName);
        Assert.Equal(21, builder._partContents[0].FileSize);
        Assert.Null(builder._httpRequestBuilder.Disposables);
    }

    [Fact]
    public void AddFileAsStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var filePathNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // filePath 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsStream(null!, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(string.Empty, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(" ", null!, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsStream(filePath, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(filePath, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(filePath, " "));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsStream(filePath, "test", null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(filePath, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsStream(filePath, "test", null, " "));

        // 文件路径不存在
        var exception =
            Assert.Throws<FileNotFoundException>(() => builder.AddFileAsStream(filePathNotFound, "test"));
        Assert.Equal($"The specified file `{filePathNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddFileAsStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        builder.AddFileAsStream(filePath, "test", null, "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.True(builder._partContents[0].RawContent is FileStream);
        Assert.Equal("test.txt", builder._partContents[0].FileName);
        Assert.Equal(21, builder._partContents[0].FileSize);
        Assert.NotNull(builder._httpRequestBuilder.Disposables);
        Assert.Single(builder._httpRequestBuilder.Disposables);
        Assert.Equal(typeof(FileStream), builder._httpRequestBuilder.Disposables[0].GetType());

        builder.AddFileAsStream(filePath, "test", null, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is FileStream);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddFileAsStream(filePath, "test", null, "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is FileStream);
        Assert.Equal("test.txt", builder._partContents[2].FileName);

        builder.AddFileAsStream(filePath, "test", "mytest.txt", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal("image/jpeg", builder._partContents[3].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[3].ContentEncoding);
        Assert.NotNull(builder._partContents[3].RawContent);
        Assert.True(builder._partContents[3].RawContent is FileStream);
        Assert.Equal("mytest.txt", builder._partContents[3].FileName);
    }

    [Fact]
    public void AddFileWithProgressAsStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var filePathNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // filePath 为空
        Assert.Throws<ArgumentNullException>(
            () => builder.AddFileWithProgressAsStream(null!, null!, null!, null, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileWithProgressAsStream(string.Empty, null!, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileWithProgressAsStream(" ", null!, null!, null, null!));

        // progressChannel 为空
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFileWithProgressAsStream(filePath, null!, null!, null, null!));

        // content-type 为空
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();

        // name 为空
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFileWithProgressAsStream(filePath, progressChannel, null!, null, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileWithProgressAsStream(filePath, progressChannel, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileWithProgressAsStream(filePath, progressChannel, " ", null, null!));

        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(
            () => builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, " "));

        // 文件路径不存在
        var exception = Assert.Throws<FileNotFoundException>(() =>
            builder.AddFileWithProgressAsStream(filePathNotFound, progressChannel, "test"));
        Assert.Equal($"The specified file `{filePathNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddFileWithProgressAsStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();

        builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.True(builder._partContents[0].RawContent is ProgressFileStream);
        Assert.Equal("test.txt", builder._partContents[0].FileName);
        Assert.Equal(21, builder._partContents[0].FileSize);
        Assert.NotNull(builder._httpRequestBuilder.Disposables);
        Assert.Single(builder._httpRequestBuilder.Disposables);
        Assert.Equal(typeof(ProgressFileStream), builder._httpRequestBuilder.Disposables[0].GetType());

        builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is ProgressFileStream);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", null, "image/jpeg;charset=utf-8",
            Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is ProgressFileStream);
        Assert.Equal("test.txt", builder._partContents[2].FileName);

        builder.AddFileWithProgressAsStream(filePath, progressChannel, "test", "mytest.txt", "image/jpeg;charset=utf-8",
            Encoding.UTF32);
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal("image/jpeg", builder._partContents[3].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[3].ContentEncoding);
        Assert.NotNull(builder._partContents[3].RawContent);
        Assert.True(builder._partContents[3].RawContent is ProgressFileStream);
        Assert.Equal("mytest.txt", builder._partContents[3].FileName);
    }

    [Fact]
    public void AddFileAsByteArray_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var filePathNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // filePath 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsByteArray(null!, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(string.Empty, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(" ", null!, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsByteArray(filePath, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(filePath, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(filePath, " ", null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileAsByteArray(filePath, "test", null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(filePath, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileAsByteArray(filePath, "test", null, " "));

        // 文件路径不存在
        var exception =
            Assert.Throws<FileNotFoundException>(() => builder.AddFileAsByteArray(filePathNotFound, "test"));
        Assert.Equal($"The specified file `{filePathNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddFileAsByteArray_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        builder.AddFileAsByteArray(filePath, "test", null, "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.True(builder._partContents[0].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[0].FileName);
        Assert.Equal(21, builder._partContents[0].FileSize);

        builder.AddFileAsByteArray(filePath, "test", null, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddFileAsByteArray(filePath, "test", null, "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[2].FileName);

        builder.AddFileAsByteArray(filePath, "test", "mytest.txt", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal("image/jpeg", builder._partContents[3].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[3].ContentEncoding);
        Assert.NotNull(builder._partContents[3].RawContent);
        Assert.True(builder._partContents[3].RawContent is byte[]);
        Assert.Equal("mytest.txt", builder._partContents[3].FileName);
    }

    [Fact]
    public void AddFile_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.AddFile(null!));
    }

    [Fact]
    public void AddFile_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        builder.AddFile(MultipartFile.CreateFromByteArray([]));
        Assert.Single(builder._partContents);

        using var stream = new MemoryStream();
        builder.AddFile(MultipartFile.CreateFromStream(stream));
        Assert.Equal(2, builder._partContents.Count);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        builder.AddFile(MultipartFile.CreateFromPath(filePath));
        Assert.Equal(3, builder._partContents.Count);

        var base64String = Convert.ToBase64String(File.ReadAllBytes(filePath));
        builder.AddFile(MultipartFile.CreateFromBase64String(base64String));
        Assert.Equal(4, builder._partContents.Count);

        builder.AddFile(MultipartFile.CreateFromRemote("https://furion.net/img/furionlogo.png"));
        Assert.Equal(5, builder._partContents.Count);
    }

    [Fact]
    public void AddStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        using var stream = new MemoryStream();

        // stream 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(null!, null!, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(stream, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, " ", null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(stream, "test", null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, "test", null, " "));
    }

    [Fact]
    public void AddStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        using var stream = new MemoryStream();

        builder.AddStream(stream, "test", "image.jpg", "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Equal(stream, builder._partContents[0].RawContent);
        Assert.Equal("image.jpg", builder._partContents[0].FileName);

        builder.AddStream(stream, "test", "image.jpg", "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.Equal(stream, builder._partContents[1].RawContent);
        Assert.Equal("image.jpg", builder._partContents[1].FileName);

        builder.AddStream(stream, "test", "image.jpg", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.Equal(stream, builder._partContents[2].RawContent);
        Assert.Equal("image.jpg", builder._partContents[2].FileName);

        builder.AddStream(stream, "test", "image.jpg");
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal("application/octet-stream", builder._partContents[3].ContentType);
        Assert.Null(builder._partContents[3].ContentEncoding);
        Assert.Equal(stream, builder._partContents[3].RawContent);
        Assert.Equal("image.jpg", builder._partContents[3].FileName);

        builder.AddStream(stream, "test");
        Assert.Equal(5, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[4].Name);
        Assert.Equal("application/octet-stream", builder._partContents[4].ContentType);
        Assert.Null(builder._partContents[4].ContentEncoding);
        Assert.Equal(stream, builder._partContents[4].RawContent);
        Assert.Null(builder._partContents[4].FileName);

        builder.AddStream(stream, "test", "image.jpg", fileSize: 21);
        Assert.Equal(6, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[5].Name);
        Assert.Equal("application/octet-stream", builder._partContents[5].ContentType);
        Assert.Null(builder._partContents[5].ContentEncoding);
        Assert.Equal(stream, builder._partContents[5].RawContent);
        Assert.Equal("image.jpg", builder._partContents[5].FileName);
        Assert.Equal(21, builder._partContents[5].FileSize);
    }

    [Fact]
    public void AddByteArray_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var bytes = Array.Empty<byte>();

        // stream 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(null!, null!, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(bytes, null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, string.Empty, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, " ", null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(bytes, "test", null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, "test", null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, "test", null, " "));
    }

    [Fact]
    public void AddByteArray_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var bytes = Array.Empty<byte>();

        builder.AddByteArray(bytes, "test", "image.jpg", "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[0].RawContent);
        Assert.Equal("image.jpg", builder._partContents[0].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg", "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[1].RawContent);
        Assert.Equal("image.jpg", builder._partContents[1].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[2].RawContent);
        Assert.Equal("image.jpg", builder._partContents[2].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg");
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal("application/octet-stream", builder._partContents[3].ContentType);
        Assert.Null(builder._partContents[3].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[3].RawContent);
        Assert.Equal("image.jpg", builder._partContents[3].FileName);

        builder.AddByteArray(bytes, "test");
        Assert.Equal(5, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[4].Name);
        Assert.Equal("application/octet-stream", builder._partContents[4].ContentType);
        Assert.Null(builder._partContents[4].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[4].RawContent);
        Assert.Null(builder._partContents[4].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg", fileSize: 21);
        Assert.Equal(6, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[5].Name);
        Assert.Equal("application/octet-stream", builder._partContents[5].ContentType);
        Assert.Null(builder._partContents[5].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[5].RawContent);
        Assert.Equal("image.jpg", builder._partContents[5].FileName);
        Assert.Equal(21, builder._partContents[5].FileSize);
    }

    [Fact]
    public void AddFormUrlEncoded_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddFormUrlEncoded(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFormUrlEncoded(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFormUrlEncoded(null, " "));
    }

    [Fact]
    public void AddFormUrlEncoded_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddFormUrlEncoded(new Dictionary<string, string>(), "test");

        Assert.Single(builder._partContents);
        Assert.Equal("application/x-www-form-urlencoded", builder._partContents[0].ContentType);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);

        builder.AddFormUrlEncoded(new Dictionary<string, string>(), "test", Encoding.UTF8);

        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("application/x-www-form-urlencoded", builder._partContents[1].ContentType);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
    }

    [Fact]
    public void AddAddMultipartFormData_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddMultipartFormData(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddMultipartFormData(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddMultipartFormData(null, " "));
    }

    [Fact]
    public void AddAddMultipartFormData_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddMultipartFormData(new { }, "test");

        Assert.Single(builder._partContents);
        Assert.Equal("multipart/form-data", builder._partContents[0].ContentType);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);

        builder.AddMultipartFormData(new { }, "test", Encoding.UTF8);

        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("multipart/form-data", builder._partContents[1].ContentType);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
    }

    [Fact]
    public void Add_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var stringContent = new StringContent("test");
        stringContent.Headers.ContentType = null;

        Assert.Throws<ArgumentNullException>(() => builder.Add(null!, null, null));
        Assert.Throws<ArgumentNullException>(() => builder.Add(stringContent, null, null));
        Assert.Throws<ArgumentNullException>(() => builder.Add(stringContent, "test", null));
        Assert.Throws<ArgumentException>(() => builder.Add(stringContent, "test", string.Empty));
        Assert.Throws<ArgumentException>(() => builder.Add(stringContent, "test", " "));
    }

    [Fact]
    public void Add_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var stringContent = new StringContent("test");
        builder.Add(stringContent, "test", null);

        Assert.Single(builder._partContents);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal(Encoding.UTF8, builder._partContents[0].ContentEncoding);
        Assert.Equal(stringContent, builder._partContents[0].RawContent);
        Assert.Null(builder._partContents[0].FileName);

        builder.Add(stringContent, "test", "application/json");

        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("application/json", builder._partContents[1].ContentType);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Null(builder._partContents[1].ContentEncoding);
        Assert.Equal(stringContent, builder._partContents[1].RawContent);
        Assert.Null(builder._partContents[1].FileName);

        builder.Add(stringContent, "test", "application/json;charset=utf-8");

        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("application/json", builder._partContents[2].ContentType);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal(Encoding.UTF8, builder._partContents[2].ContentEncoding);
        Assert.Equal(stringContent, builder._partContents[2].RawContent);
        Assert.Null(builder._partContents[2].FileName);

        builder.Add(stringContent, "test", "application/json;charset=utf-8", Encoding.UTF32);

        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("application/json", builder._partContents[3].ContentType);
        Assert.Equal("test", builder._partContents[3].Name);
        Assert.Equal(Encoding.UTF32, builder._partContents[3].ContentEncoding);
        Assert.Equal(stringContent, builder._partContents[3].RawContent);
        Assert.Null(builder._partContents[3].FileName);

        stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "test" };
        builder.Add(stringContent, null, "application/json;charset=utf-8", Encoding.UTF32);

        Assert.Equal(5, builder._partContents.Count);
        Assert.Equal("application/json", builder._partContents[4].ContentType);
        Assert.Equal("test", builder._partContents[4].Name);
        Assert.Equal(Encoding.UTF32, builder._partContents[4].ContentEncoding);
        Assert.Equal(stringContent, builder._partContents[4].RawContent);
        Assert.Null(builder._partContents[4].FileName);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!, null!, null));
        Assert.Throws<ArgumentNullException>(() => builder.Build(new HttpRemoteOptions(), null!, null));
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        using var serviceProvider = services.BuildServiceProvider();
        var httpRemoteOptions = new HttpRemoteOptions();
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var httpContentProcessorFactory = new HttpContentProcessorFactory(serviceProvider, []);

        var multipartFormDataContent = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.Null(multipartFormDataContent);

        builder.AddFormItem(new { }, "test");
        var multipartFormDataContent1 = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.NotNull(multipartFormDataContent1);
        Assert.Single(multipartFormDataContent1);

        builder.AddFormItem(null, "test");
        var multipartFormDataContent2 = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.NotNull(multipartFormDataContent2);
        Assert.Single(multipartFormDataContent2);

        builder.AddFormItem(new { }, "test");
        var multipartFormDataContent3 =
            builder.Build(httpRemoteOptions, httpContentProcessorFactory, new CustomStringContentProcessor());
        Assert.NotNull(multipartFormDataContent3);
        Assert.Equal(2, multipartFormDataContent3.Count());
    }

    [Fact]
    public void BuildHttpContent_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => HttpMultipartFormDataBuilder.BuildHttpContent(null!, null!, null));
        Assert.Throws<ArgumentNullException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(new MultipartFormDataItem("test"), null!, null));
        Assert.Throws<ArgumentNullException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(new MultipartFormDataItem("test"),
                new HttpContentProcessorFactory(serviceProvider, []), null));
        Assert.Throws<ArgumentException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = string.Empty },
                new HttpContentProcessorFactory(serviceProvider, []),
                null));
        Assert.Throws<ArgumentException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = " " },
                new HttpContentProcessorFactory(serviceProvider, []),
                null));
    }

    [Fact]
    public void BuildHttpContent_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddOptions<HttpRemoteOptions>();
        using var serviceProvider = services.BuildServiceProvider();
        var httpContentProcessorFactory = new HttpContentProcessorFactory(serviceProvider, []);

        var httpContent1 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = "text/plain" }, httpContentProcessorFactory, null)!;
        Assert.Null(httpContent1);

        var httpContent2 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = "text/plain", RawContent = new { } },
                httpContentProcessorFactory, null)!;
        Assert.NotNull(httpContent2);

        var httpContent3 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test")
                {
                    ContentType = "text/plain", RawContent = new StringContent("test"), FileName = "text.txt"
                },
                httpContentProcessorFactory, null)!;
        Assert.NotNull(httpContent3);
        Assert.Null(httpContent3.Headers.ContentDisposition);

        var httpContent4 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test")
                {
                    ContentType = "application/octet-stream",
                    RawContent = new ByteArrayContent([]),
                    FileName = "text.txt"
                },
                httpContentProcessorFactory, null)!;
        Assert.NotNull(httpContent4);
        Assert.NotNull(httpContent4.Headers.ContentDisposition);
        Assert.Equal("test", httpContent4.Headers.ContentDisposition.Name);
        Assert.Equal("text.txt", httpContent4.Headers.ContentDisposition.FileName);

        using var stream = new MemoryStream();
        var httpContent5 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test")
                {
                    ContentType = "application/octet-stream",
                    RawContent = new StreamContent(stream),
                    FileName = "text.txt"
                },
                httpContentProcessorFactory, null)!;
        Assert.NotNull(httpContent5);
        Assert.NotNull(httpContent5.Headers.ContentDisposition);
        Assert.Equal("test", httpContent5.Headers.ContentDisposition.Name);
        Assert.Equal("text.txt", httpContent5.Headers.ContentDisposition.FileName);

        var httpContent6 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test")
                {
                    ContentType = "application/x-www-form-urlencoded",
                    RawContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()),
                    FileName = "text.txt"
                },
                httpContentProcessorFactory, null)!;
        Assert.NotNull(httpContent6);
        Assert.Null(httpContent6.Headers.ContentDisposition);

        var httpContent7 =
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test")
                {
                    ContentType = "application/x-www-form-urlencoded",
                    RawContent = new StringContent("test"),
                    FileName = "text.txt"
                },
                httpContentProcessorFactory, new CustomStringContentProcessor())!;
        Assert.NotNull(httpContent7);
        Assert.Null(httpContent7.Headers.ContentDisposition);
    }

    [Fact]
    public void ParseContentType_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpMultipartFormDataBuilder.ParseContentType(null!, null, out _));
        Assert.Throws<ArgumentException>(() =>
            HttpMultipartFormDataBuilder.ParseContentType(string.Empty, null, out _));
        Assert.Throws<ArgumentException>(() => HttpMultipartFormDataBuilder.ParseContentType(" ", null, out _));
    }

    [Fact]
    public void ParseContentType_ReturnOK()
    {
        var contentType = HttpMultipartFormDataBuilder.ParseContentType("text/plain", null, out var encoding);
        Assert.NotNull(contentType);
        Assert.Equal("text/plain", contentType);
        Assert.Null(encoding);

        var contentType2 =
            HttpMultipartFormDataBuilder.ParseContentType("text/plain;charset=utf-8", null, out var encoding2);
        Assert.NotNull(contentType2);
        Assert.Equal("text/plain", contentType2);
        Assert.Equal(Encoding.UTF8, encoding2);

        var contentType3 =
            HttpMultipartFormDataBuilder.ParseContentType("text/plain;charset=utf-8", Encoding.UTF32,
                out var encoding3);
        Assert.NotNull(contentType3);
        Assert.Equal("text/plain", contentType3);
        Assert.Equal(Encoding.UTF32, encoding3);
    }
}