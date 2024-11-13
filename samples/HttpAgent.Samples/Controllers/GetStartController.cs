using HttpAgent.Samples.Models;
using System.Text;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController(IHttpRemoteService httpRemoteService) : ControllerBase
{
    /// <summary>
    /// 获取网站内容
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string?> GetWebSiteContent()
    {
        var content = await httpRemoteService.GetAsStringAsync("https://furion.net");

        // 1. 构建器方式

        // 直接获取 String 类型
        var content1 = await httpRemoteService.SendAsStringAsync(HttpRequestBuilder.Get("https://furion.net"));

        // 通过泛型指定 String 类型
        var content2 = await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Get("https://furion.net"));

        // 获取 HttpRemoteResult 类型
        var result = await httpRemoteService.SendAsync<string>(HttpRequestBuilder.Get("https://furion.net"));
        var content3 = result.Result;

        // 获取 HttpResponseMessage 类型
        var httpResponseMessage = await httpRemoteService.SendAsync(HttpRequestBuilder.Get("https://furion.net"));
        var content4 = await httpResponseMessage.Content.ReadAsStringAsync();

        // 2. 请求谓词方式

        // 通过泛型指定 String 类型
        var content5 = await httpRemoteService.GetAsAsync<string>("https://furion.net");

        // 获取 HttpRemoteResult 类型
        var result2 = await httpRemoteService.GetAsync<string>("https://furion.net");
        var content6 = result2.Result;

        // 获取 HttpResponseMessage 类型
        var httpResponseMessage2 = await httpRemoteService.GetAsync("https://furion.net");
        var content7 = await httpResponseMessage2.Content.ReadAsStringAsync();

        return content;
    }

    /// <summary>
    /// 携带请求数据
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<YourRemoteModel?> PostData()
    {
        var content = await httpRemoteService.PostAsAsync<YourRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetJsonContent(new { id = 1, name = "furion" }));  // 设置请求内容

        // 构建器方式
        var content2 = await httpRemoteService.SendAsAsync<YourRemoteModel>(HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddModel")
            .WithQueryParameter("query1", 1) // 设置查询参数（支持单个设置）
            .WithQueryParameter("query2", "furion") // 设置查询参数（支持单个设置）
            .SetJsonContent("{\"id\":1,\"name\":\"furion\"}"));  // 设置请求内容（支持直接传入 JSON 字符串）

        // 更多方式可参考 19.2.1 使用

        // 自定义 Content-Type
        var content3 = await httpRemoteService.PostAsAsync<YourRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetRawContent(new { id = 1, name = "furion" }, "application/json"));  // 设置请求内容

        // 自定义 Content-Type 支持配置 Charset
        var content4 = await httpRemoteService.PostAsAsync<YourRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetRawContent(new { id = 1, name = "furion" }, "application/json;charset=utf-8"));  // 设置请求内容

        // 自定义 Content-Type 支持配置请求编码
        var content5 = await httpRemoteService.PostAsAsync<YourRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder
                .WithQueryParameters(new { query1 = 1, query2 = "furion" }) // 设置查询参数
                .SetRawContent(new { id = 1, name = "furion" }, "application/json;charset=utf-8", Encoding.UTF8));  // 设置请求内容

        return content;
    }

    /// <summary>
    /// Form 表单提交
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<YourRemoteFormResult?> PostForm()
    {
        var content = await httpRemoteService.PostAsAsync<YourRemoteFormResult>("https://localhost:7044/HttpRemote/AddForm?id=1",
            builder => builder.SetMultipartContent(formBuilder => formBuilder   // 设置表单内容
                .AddJson(new { id = 1, name = "furion" })   // 设置常规字段
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "file")));  // 设置文件（支持流方式、字节数组方式、远程 URL 地址和 Base64 字符串

        // 使用构建器模式
        var content2 = await httpRemoteService.SendAsAsync<YourRemoteFormResult>(HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddForm?id=1")
            .SetMultipartContent(formBuilder => formBuilder   // 设置表单内容
                .AddJson(new { id = 1, name = "furion" })   // 设置常规字段
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "file")));  // 设置文件（支持流方式、字节数组方式、远程 URL 地址和 Base64 字符串

        // 更多详细用法可参考第 19.2.1 节

        // 以下是一些 `Form` 表单提交的常见例子
        var content3 = await httpRemoteService.PostAsAsync<YourRemoteFormResult>("https://localhost:7044/HttpRemote/AddForm?id=1",
            builder => builder.SetMultipartContent(formBuilder => formBuilder   // 设置表单内容
                .AddJson(new { id = 1, name = "furion" })   // 设置常规字段
                .AddJsonProperty("age", "Age")  // 支持设置单个值
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "file") // 设置单个文件（对应表单 File 字段）
                                                                        // 支持互联网文件地址
                .AddFileFromRemote("https://furion.net/img/furionlogo.png", "files")    // 设置多个文件（对应表单 Files 字段）
                                                                                        // 支持读取本地文件作为字节数组
                .AddFileAsByteArray("C:\\Workspaces\\httptest.jpg", "files")));  // 设置多个文件（对应表单 Files 字段）

        return content;
    }

    /// <summary>
    /// 下载网络资源
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task DownloadFile()
    {
        // 从指定 URL 下载 ASP.NET Core 运行时，并保存到 C:\Workspaces\ 目录中
        // 如果未指定文件名，系统将自动从下载地址中解析出文件名，例如：aspnetcore-runtime-8.0.10-win-x64.exe
        await httpRemoteService.DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , fileExistsBehavior: FileExistsBehavior.Overwrite);

        // 打印下载进度
        await httpRemoteService.DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , async progress =>
            {
                Console.WriteLine(progress.ToSummaryString());  // 输出简要进度字符串
                await Task.CompletedTask;
            }
            , fileExistsBehavior: FileExistsBehavior.Overwrite);

        // 打印下载进度
        await httpRemoteService.DownloadFileAsync("https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , async progress =>
            {
                Console.WriteLine(progress.ToString());  // 输出带缩进进度字符串
                await Task.CompletedTask;
            }
            , fileExistsBehavior: FileExistsBehavior.Overwrite);

        // 使用构建器模式
        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile("https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe"
            , @"C:\Workspaces\"
            , fileExistsBehavior: FileExistsBehavior.Overwrite));

        // 更多详细用法可参考第 19.2.1 节
    }

    /// <summary>
    /// 上传文件资源
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task UploadFile()
    {
        // 1. 使用 Form 表单方式

        // 上传单个文件
        await httpRemoteService.PostAsync("https://localhost:7044/HttpRemote/AddFile", builder => builder
            .SetMultipartContent(formBuilder => formBuilder
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "file")));

        // 上传多个文件
        await httpRemoteService.PostAsync("https://localhost:7044/HttpRemote/AddFiles", builder => builder
            .SetMultipartContent(formBuilder => formBuilder
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "files")
                .AddFileFromRemote("https://furion.net/img/furionlogo.png", "files")));

        // 使用构建器模式
        await httpRemoteService.SendAsync(HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddFile")
            .SetMultipartContent(formBuilder => formBuilder
                .AddFileAsStream(@"C:\Workspaces\httptest.jpg", "file")));

        // 上传文件带进度
        await httpRemoteService.UploadFileAsync("https://localhost:7044/HttpRemote/AddFile", @"C:\Workspaces\httptest.jpg", "file"
            , async progress =>
            {
                Console.WriteLine(progress.ToSummaryString());  // 输出简要进度字符串
                await Task.CompletedTask;
            });

        // 支持限制文件类型和大小
        await httpRemoteService.SendAsync(HttpRequestBuilder.UploadFile("https://localhost:7044/HttpRemote/AddFile", @"C:\Workspaces\httptest.jpg", "file"
            , async progress =>
            {
                Console.WriteLine(progress.ToSummaryString());  // 输出简要进度字符串
                await Task.CompletedTask;
            })
            .SetAllowedFileExtensions(".jpg;.png")  // 限制只允许 jpg 和 png 类型
            .SetMaxFileSizeInBytes(5 * 1024 * 1024));   // 限制 5MB
    }

    /// <summary>
    /// 压力模拟测试
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task StressTestHarness()
    {
        var stressTestHarnessResult = await httpRemoteService.StressTestHarnessAsync("https://furion.net/");
        Console.WriteLine(stressTestHarnessResult.ToString());  // 打印压力测试结果

        var stressTestHarnessResult1 = await httpRemoteService.SendAsync(HttpRequestBuilder.StressTestHarness("https://furion.net/")
            .SetNumberOfRequests(1000)  // 设置并发请求数量
            .SetNumberOfRounds(5)   // 设置压测轮次
            .SetMaxDegreeOfParallelism(500));   // 设置最大并发度

        // 在大多数情况下，只需要设置并发请求数量即可
        var stressTestHarnessResult2 = await httpRemoteService.StressTestHarnessAsync("https://furion.net/", 500);

        var stressTestHarnessResult3 = await httpRemoteService.SendAsync(HttpRequestBuilder.StressTestHarness("https://furion.net/", 500));
    }
}