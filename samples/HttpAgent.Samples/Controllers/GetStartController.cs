using HttpAgent.Samples.Models;
using System.Text;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController(IHttpRemoteService httpRemoteService) : ControllerBase
{
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

    [HttpPost]
    public async Task<YourRemoteModel?> PostJson()
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

    [HttpGet]
    public async Task DownloadFile()
    {
        // 下载 ASP.NET Core 运行时并保存到 C:\Workspaces\ 目录中
        // 如果不配置文件名，将自动解析下载地址中的文件名，如：aspnetcore-runtime-8.0.10-win-x64.exe
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
    }
}