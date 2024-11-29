// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MultipartFileTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var multipartFile = new MultipartFile();
        Assert.Null(multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.Null(multipartFile.Source);
        Assert.Equal(FileSourceType.None, multipartFile.FileSourceType);
    }

    [Fact]
    public void CreateFromByteArray_ReturnOK()
    {
        var multipartFile = MultipartFile.CreateFromByteArray([]);
        Assert.Equal("file", multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.NotNull(multipartFile.Source);
        Assert.True(multipartFile.Source is byte[]);
        Assert.Equal(FileSourceType.ByteArray, multipartFile.FileSourceType);
    }

    [Fact]
    public void CreateFromStream_ReturnOK()
    {
        using var stream = new MemoryStream();
        var multipartFile = MultipartFile.CreateFromStream(stream);
        Assert.Equal("file", multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.NotNull(multipartFile.Source);
        Assert.True(multipartFile.Source is Stream);
        Assert.Equal(FileSourceType.Stream, multipartFile.FileSourceType);
    }

    [Fact]
    public void CreateFromPath_ReturnOK()
    {
        var multipartFile = MultipartFile.CreateFromPath(@"C:\Workspaces\httptest.jpg");
        Assert.Equal("file", multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.NotNull(multipartFile.Source);
        Assert.True(multipartFile.Source is string);
        Assert.Equal(FileSourceType.Path, multipartFile.FileSourceType);
    }

    [Fact]
    public void CreateFromBase64String_ReturnOK()
    {
        var base64String =
            Convert.ToBase64String(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test.txt")));

        var multipartFile = MultipartFile.CreateFromBase64String(base64String);
        Assert.Equal("file", multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.NotNull(multipartFile.Source);
        Assert.True(multipartFile.Source is string);
        Assert.Equal(FileSourceType.Base64String, multipartFile.FileSourceType);
    }

    [Fact]
    public void CreateFromRemote_ReturnOK()
    {
        var multipartFile = MultipartFile.CreateFromRemote("https://furion.net/img/furionlogo.png");
        Assert.Equal("file", multipartFile.Name);
        Assert.Null(multipartFile.FileName);
        Assert.Null(multipartFile.ContentType);
        Assert.Null(multipartFile.ContentEncoding);
        Assert.NotNull(multipartFile.Source);
        Assert.True(multipartFile.Source is string);
        Assert.Equal(FileSourceType.Remote, multipartFile.FileSourceType);
    }
}