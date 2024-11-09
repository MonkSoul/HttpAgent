// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.AspNetCore.Tests;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController : ControllerBase
{
    [HttpGet]
    public string Request1()
    {
        Response.Headers.Append("Framework", "furion");
        return "Hello World";
    }

    [HttpPost]
    public HttpRemoteAspNetCoreModel1 Request2(HttpRemoteAspNetCoreModel1 model1) => model1;

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded")]
    public HttpRemoteAspNetCoreModel1 Request3([FromForm] HttpRemoteAspNetCoreModel1 model) => model;

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> Request4(IFormFile file) => Task.FromResult(file.FileName);

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    public Task<string> Request5(IFormFileCollection files) =>
        Task.FromResult(string.Join(';', files.Select(u => u.FileName)));

    [HttpPost]
    public Task<string> Request6([FromForm] HttpRemoteAspNetCoreMultipartModel1 model) =>
        Task.FromResult($"{model.Id};{model.Name};{model.File?.FileName}");

    [HttpPost]
    // [Consumes("text/plain")]
    public string Request7([FromBody] string str) => str;

    [HttpGet]
    public IActionResult Request8() => RedirectToAction("RedirectFrom", "HttpRemote");

    [HttpGet]
    public IActionResult RedirectFrom() => Content("Redirect");

    [HttpPost]
    public IActionResult Request9() =>
        new FileStreamResult(System.IO.File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")),
            "application/octet-stream") { FileDownloadName = "test.txt" };

    [HttpGet]
    public int Request10(int number) => number + 1;
}