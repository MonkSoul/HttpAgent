// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

public class IActionResultContentConverterTests
{
    [Theory]
    [InlineData(200, null)]
    [InlineData(204, typeof(NoContentResult))]
    [InlineData(400, typeof(BadRequestResult))]
    [InlineData(401, typeof(UnauthorizedResult))]
    [InlineData(404, typeof(NotFoundResult))]
    [InlineData(409, typeof(ConflictResult))]
    [InlineData(415, typeof(UnsupportedMediaTypeResult))]
    [InlineData(422, typeof(UnprocessableEntityResult))]
    [InlineData(500, null)]
    public void TryGetStatusCodeResult_ReturnOK(int statusCode, Type? actionResultType)
    {
        var httpResponseMessage = new HttpResponseMessage { StatusCode = (HttpStatusCode)statusCode };
        IActionResultContentConverter.TryGetStatusCodeResult(httpResponseMessage, out var httpStatusCode,
            out var actionResult);

        Assert.True((int)httpStatusCode > 0);
        Assert.Equal(actionResultType, actionResult?.GetType());
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json-patch+json")]
    [InlineData("application/xml")]
    [InlineData("application/xml-patch+xml")]
    [InlineData("text/xml")]
    [InlineData("text/html")]
    [InlineData("text/plain")]
    public void Read_ContentResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var actionResult = actionResultContentConverter.Read(httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is ContentResult);

        var contentResult = actionResult as ContentResult;
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(contentType, contentResult.ContentType);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/octet-stream")]
    [InlineData("image/jpeg")]
    public void Read_FileStreamResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.pdf" };
        var actionResult = actionResultContentConverter.Read(httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is FileStreamResult);
        var fileStreamResult = actionResult as FileStreamResult;

        Assert.NotNull(fileStreamResult);
        Assert.Equal(contentType, fileStreamResult.ContentType);
        Assert.Equal("test.pdf", fileStreamResult.FileDownloadName);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json-patch+json")]
    [InlineData("application/xml")]
    [InlineData("application/xml-patch+xml")]
    [InlineData("text/xml")]
    [InlineData("text/html")]
    [InlineData("text/plain")]
    public async Task ReadAsync_ContentResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var actionResult = await actionResultContentConverter.ReadAsync(httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is ContentResult);

        var contentResult = actionResult as ContentResult;
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(contentType, contentResult.ContentType);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/octet-stream")]
    [InlineData("image/jpeg")]
    public async Task ReadAsync_FileStreamResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.pdf" };
        var actionResult = await actionResultContentConverter.ReadAsync(httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is FileStreamResult);
        var fileStreamResult = actionResult as FileStreamResult;

        Assert.NotNull(fileStreamResult);
        Assert.Equal(contentType, fileStreamResult.ContentType);
        Assert.Equal("test.pdf", fileStreamResult.FileDownloadName);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json-patch+json")]
    [InlineData("application/xml")]
    [InlineData("application/xml-patch+xml")]
    [InlineData("text/xml")]
    [InlineData("text/html")]
    [InlineData("text/plain")]
    public void Read_WithType_ContentResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var actionResult = actionResultContentConverter.Read(typeof(ContentResult), httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is ContentResult);

        var contentResult = actionResult as ContentResult;
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(contentType, contentResult.ContentType);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/octet-stream")]
    [InlineData("image/jpeg")]
    public void Read_WithType_FileStreamResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.pdf" };
        var actionResult = actionResultContentConverter.Read(typeof(FileStreamResult), httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is FileStreamResult);
        var fileStreamResult = actionResult as FileStreamResult;

        Assert.NotNull(fileStreamResult);
        Assert.Equal(contentType, fileStreamResult.ContentType);
        Assert.Equal("test.pdf", fileStreamResult.FileDownloadName);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/json-patch+json")]
    [InlineData("application/xml")]
    [InlineData("application/xml-patch+xml")]
    [InlineData("text/xml")]
    [InlineData("text/html")]
    [InlineData("text/plain")]
    public async Task ReadAsync_WithType_ContentResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var actionResult = await actionResultContentConverter.ReadAsync(typeof(ContentResult), httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is ContentResult);

        var contentResult = actionResult as ContentResult;
        Assert.NotNull(contentResult);
        Assert.Equal(200, contentResult.StatusCode);
        Assert.Equal(contentType, contentResult.ContentType);
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("application/octet-stream")]
    [InlineData("image/jpeg")]
    public async Task ReadAsync_WithType_FileStreamResult_ReturnOk(string contentType)
    {
        var actionResultContentConverter = new IActionResultContentConverter();

        var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        httpResponseMessage.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment") { FileName = "test.pdf" };
        var actionResult = await actionResultContentConverter.ReadAsync(typeof(FileStreamResult), httpResponseMessage);

        Assert.NotNull(actionResult);
        Assert.True(actionResult is FileStreamResult);
        var fileStreamResult = actionResult as FileStreamResult;

        Assert.NotNull(fileStreamResult);
        Assert.Equal(contentType, fileStreamResult.ContentType);
        Assert.Equal("test.pdf", fileStreamResult.FileDownloadName);
    }
}