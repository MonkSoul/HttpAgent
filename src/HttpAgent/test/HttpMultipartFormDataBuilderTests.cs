// 版权归百小僧及百签科技（广东）有限公司所有。
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
    }

    [Fact]
    public void AddJsonProperty_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.AddJsonProperty(null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddJsonProperty(null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddJsonProperty(null, " "));
    }

    [Fact]
    public void AddJsonProperty_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        builder.AddJsonProperty("furion", "name", Encoding.UTF8);
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
    public void AddRaw_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        // content-type 为空情况
        Assert.Throws<ArgumentNullException>(() => builder.AddRaw(null!, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddRaw(null!, null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddRaw(null!, null, " "));

        // rawObject  为空情况
        Assert.Throws<ArgumentNullException>(() => builder.AddRaw(null, null, "application/json"));

        // 不能转换为字典情况
        Assert.Throws<NotSupportedException>(() => builder.AddRaw("", null, "application/json"));
    }

    [Fact]
    public void AddRaw_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        builder.AddRaw(null, "name");
        Assert.Single(builder._partContents);
        Assert.Equal("name", builder._partContents[0].Name);
        Assert.Equal("text/plain", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Null(builder._partContents[0].RawContent);

        builder.AddRaw(new { }, "name");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[1].Name);
        Assert.Equal("text/plain", builder._partContents[1].ContentType);
        Assert.Null(builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);

        builder.AddRaw(new { }, "name", "application/json");
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[2].Name);
        Assert.Equal("application/json", builder._partContents[2].ContentType);
        Assert.Null(builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);

        builder.AddRaw(new { }, "name", "application/json;charset=utf-8");
        Assert.Equal(4, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[3].Name);
        Assert.Equal("application/json", builder._partContents[3].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[3].ContentEncoding);
        Assert.NotNull(builder._partContents[3].RawContent);

        builder.AddRaw(new MultipartModel(), "name", "application/json;charset=utf-8", Encoding.UTF32);
        Assert.Equal(5, builder._partContents.Count);
        Assert.Equal("name", builder._partContents[4].Name);
        Assert.Equal("application/json", builder._partContents[4].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[4].ContentEncoding);
        Assert.NotNull(builder._partContents[4].RawContent);

        builder.AddRaw(new MultipartModel { Id = 1, Name = "furion" }, null, "application/json", Encoding.UTF8);
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
        var url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";
        var hasNotFileNameUrl = "https://furion.net";

        // url 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(null!, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(" ", null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(url, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, " ", null!, null!));

        // fileName 为空
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(hasNotFileNameUrl, "file", null!, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromRemote(url, "test", null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, "test", null!, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromRemote(url, "test", null!, " "));
    }

    [Fact]
    public void AddFileFromRemote_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var url =
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
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(" ", null!, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromBase64String(base64String, null!, null!, null!));
        Assert.Throws<ArgumentException>(
            () => builder.AddFileFromBase64String(base64String, string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(base64String, " ", null!, null!));

        // fileName 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileFromBase64String(base64String, "file", null!, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileFromBase64String(base64String, "file", string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(base64String, "file", " ", null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddFileFromBase64String(base64String, "test", "test.txt", null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddFileFromBase64String(base64String, "test", "test.txt", string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileFromBase64String(base64String, "test", "test.txt", " "));
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
    public void AddFileStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileFullNameNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // fileFullName 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileStream(null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(" ", null!, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileStream(fileFullName, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(fileFullName, string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(fileFullName, " ", null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileStream(fileFullName, "test", null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(fileFullName, "test", string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileStream(fileFullName, "test", " "));

        // 文件路径不存在
        var exception = Assert.Throws<FileNotFoundException>(() => builder.AddFileStream(fileFullNameNotFound, "test"));
        Assert.Equal($"The specified file `{fileFullNameNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddFileStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");

        builder.AddFileStream(fileFullName, "test", "image/jpeg");
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

        builder.AddFileStream(fileFullName, "test", "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is FileStream);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddFileStream(fileFullName, "test", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is FileStream);
        Assert.Equal("test.txt", builder._partContents[2].FileName);
    }

    [Fact]
    public void AddProgressFileStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileFullNameNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // fileFullName 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddProgressFileStream(null!, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddProgressFileStream(string.Empty, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddProgressFileStream(" ", null!, null!, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddProgressFileStream(fileFullName, null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddProgressFileStream(fileFullName, string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddProgressFileStream(fileFullName, " ", null!, null!));

        // progressChannel 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddProgressFileStream(fileFullName, "test", null!, null!));

        // content-type 为空
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        Assert.Throws<ArgumentNullException>(() =>
            builder.AddProgressFileStream(fileFullName, "test", progressChannel, null!));
        Assert.Throws<ArgumentException>(() =>
            builder.AddProgressFileStream(fileFullName, "test", progressChannel, string.Empty));
        Assert.Throws<ArgumentException>(
            () => builder.AddProgressFileStream(fileFullName, "test", progressChannel, " "));

        // 文件路径不存在
        var exception = Assert.Throws<FileNotFoundException>(() =>
            builder.AddProgressFileStream(fileFullNameNotFound, "test", progressChannel));
        Assert.Equal($"The specified file `{fileFullNameNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddProgressFileStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();

        builder.AddProgressFileStream(fileFullName, "test", progressChannel, "image/jpeg");
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

        builder.AddProgressFileStream(fileFullName, "test", progressChannel, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is ProgressFileStream);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddProgressFileStream(fileFullName, "test", progressChannel, "image/jpeg;charset=utf-8",
            Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is ProgressFileStream);
        Assert.Equal("test.txt", builder._partContents[2].FileName);
    }

    [Fact]
    public void AddFileBytes_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileFullNameNotFound = Path.Combine(AppContext.BaseDirectory, "not-found.txt");

        // fileFullName 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileBytes(null!, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(string.Empty, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(" ", null!, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileBytes(fileFullName, null!, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(fileFullName, string.Empty, null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(fileFullName, " ", null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddFileBytes(fileFullName, "test", null!));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(fileFullName, "test", string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddFileBytes(fileFullName, "test", " "));

        // 文件路径不存在
        var exception = Assert.Throws<FileNotFoundException>(() => builder.AddFileBytes(fileFullNameNotFound, "test"));
        Assert.Equal($"The specified file `{fileFullNameNotFound}` does not exist.",
            exception.Message);
    }

    [Fact]
    public void AddFileBytes_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var fileFullName = Path.Combine(AppContext.BaseDirectory, "test.txt");

        builder.AddFileBytes(fileFullName, "test", "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.NotNull(builder._partContents[0].RawContent);
        Assert.True(builder._partContents[0].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[0].FileName);
        Assert.Equal(21, builder._partContents[0].FileSize);

        builder.AddFileBytes(fileFullName, "test", "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.NotNull(builder._partContents[1].RawContent);
        Assert.True(builder._partContents[1].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[1].FileName);

        builder.AddFileBytes(fileFullName, "test", "image/jpeg;charset=utf-8", Encoding.UTF32);
        Assert.Equal(3, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[2].Name);
        Assert.Equal("image/jpeg", builder._partContents[2].ContentType);
        Assert.Equal(Encoding.UTF32, builder._partContents[2].ContentEncoding);
        Assert.NotNull(builder._partContents[2].RawContent);
        Assert.True(builder._partContents[2].RawContent is byte[]);
        Assert.Equal("test.txt", builder._partContents[2].FileName);
    }

    [Fact]
    public void AddStream_Invalid_Parameters()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        using var stream = new MemoryStream();

        // stream 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(null!, null!, null, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(stream, null!, null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, string.Empty, null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, " ", null, null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(stream, "test", null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, "test", null, null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddStream(stream, "test", null, null, " "));
    }

    [Fact]
    public void AddStream_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        using var stream = new MemoryStream();

        builder.AddStream(stream, "test", "image.jpg", null, "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Equal(stream, builder._partContents[0].RawContent);
        Assert.Equal("image.jpg", builder._partContents[0].FileName);

        builder.AddStream(stream, "test", "image.jpg", null, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.Equal(stream, builder._partContents[1].RawContent);
        Assert.Equal("image.jpg", builder._partContents[1].FileName);

        builder.AddStream(stream, "test", "image.jpg", null, "image/jpeg;charset=utf-8", Encoding.UTF32);
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

        builder.AddStream(stream, "test", "image.jpg", 21);
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
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(null!, null!, null, null, null!));

        // name 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(bytes, null!, null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, string.Empty, null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, " ", null, null, null!));

        // content-type 为空
        Assert.Throws<ArgumentNullException>(() => builder.AddByteArray(bytes, "test", null, null, null!));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, "test", null, null, string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddByteArray(bytes, "test", null, null, " "));
    }

    [Fact]
    public void AddByteArray_ReturnOK()
    {
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var bytes = Array.Empty<byte>();

        builder.AddByteArray(bytes, "test", "image.jpg", null, "image/jpeg");
        Assert.Single(builder._partContents);
        Assert.Equal("test", builder._partContents[0].Name);
        Assert.Equal("image/jpeg", builder._partContents[0].ContentType);
        Assert.Null(builder._partContents[0].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[0].RawContent);
        Assert.Equal("image.jpg", builder._partContents[0].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg", null, "image/jpeg;charset=utf-8");
        Assert.Equal(2, builder._partContents.Count);
        Assert.Equal("test", builder._partContents[1].Name);
        Assert.Equal("image/jpeg", builder._partContents[1].ContentType);
        Assert.Equal(Encoding.UTF8, builder._partContents[1].ContentEncoding);
        Assert.Equal(bytes, builder._partContents[1].RawContent);
        Assert.Equal("image.jpg", builder._partContents[1].FileName);

        builder.AddByteArray(bytes, "test", "image.jpg", null, "image/jpeg;charset=utf-8", Encoding.UTF32);
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

        builder.AddByteArray(bytes, "test", "image.jpg", 21);
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
        var httpRemoteOptions = new HttpRemoteOptions();
        var builder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));
        var httpContentProcessorFactory = new HttpContentProcessorFactory([]);

        var multipartFormDataContent = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.Null(multipartFormDataContent);

        builder.AddJsonProperty(new { }, "test");
        var multipartFormDataContent1 = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.NotNull(multipartFormDataContent1);
        Assert.Single(multipartFormDataContent1);

        builder.AddJsonProperty(null, "test");
        var multipartFormDataContent2 = builder.Build(httpRemoteOptions, httpContentProcessorFactory, null);
        Assert.NotNull(multipartFormDataContent2);
        Assert.Single(multipartFormDataContent2);

        builder.AddJsonProperty(new { }, "test");
        var multipartFormDataContent3 =
            builder.Build(httpRemoteOptions, httpContentProcessorFactory, new CustomStringContentProcessor());
        Assert.NotNull(multipartFormDataContent3);
        Assert.Equal(2, multipartFormDataContent3.Count());
    }

    [Fact]
    public void BuildHttpContent_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpMultipartFormDataBuilder.BuildHttpContent(null!, null!, null));
        Assert.Throws<ArgumentNullException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(new MultipartFormDataItem("test"), null!, null));
        Assert.Throws<ArgumentNullException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(new MultipartFormDataItem("test"),
                new HttpContentProcessorFactory([]), null));
        Assert.Throws<ArgumentException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = string.Empty }, new HttpContentProcessorFactory([]),
                null));
        Assert.Throws<ArgumentException>(() =>
            HttpMultipartFormDataBuilder.BuildHttpContent(
                new MultipartFormDataItem("test") { ContentType = " " }, new HttpContentProcessorFactory([]),
                null));
    }

    [Fact]
    public void BuildHttpContent_ReturnOK()
    {
        var httpContentProcessorFactory = new HttpContentProcessorFactory([]);

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