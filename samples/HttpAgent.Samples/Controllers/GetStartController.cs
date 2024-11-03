using HttpAgent.Samples.Models;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController(IHttpRemoteService httpRemoteService) : ControllerBase
{
    [HttpGet]
    public async Task<string?> GetHtml()
    {
        // 方式一
        var html = await httpRemoteService.GetAsAsync<string>("https://furion.net/");

        // 方式二
        var html2 = await httpRemoteService.GetAsStringAsync("https://furion.net/");

        return await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Get("https://furion.net/"));
    }

    [HttpPost]
    public async Task<HttpRemoteModel?> PostJson()
    {
        var model = await httpRemoteService.PostAsAsync<HttpRemoteModel>("https://localhost:7044/HttpRemote/AddModel",
            builder => builder.SetJsonContent(new HttpRemoteModel
            {
                Id = 1,
                Name = "Furion"
            }));

        return await httpRemoteService.SendAsAsync<HttpRemoteModel>(
            HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddModel").SetJsonContent(new HttpRemoteModel
            {
                Id = 1,
                Name = "Furion"
            }));
    }

    [HttpPost]
    public async Task<HttpRemoteFormResult?> PostForm()
    {
        var result = await httpRemoteService.PostAsAsync<HttpRemoteFormResult>(
            "https://localhost:7044/HttpRemote/AddForm",
            builder => builder.SetMultipartContent(formBuilder => formBuilder
                .AddJson(new HttpRemoteModel
                {
                    Id = 1,
                    Name = "Furion"
                }).AddFileStream(@"C:\Workspaces\httptest.jpg", "file")));

        return await httpRemoteService.SendAsAsync<HttpRemoteFormResult>(
            HttpRequestBuilder.Post("https://localhost:7044/HttpRemote/AddForm").SetMultipartContent(formBuilder =>
                formBuilder
                    .AddJson(new HttpRemoteModel
                    {
                        Id = 1,
                        Name = "Furion"
                    }).AddFileStream(@"C:\Workspaces\httptest.jpg", "file")));
    }

    [HttpGet]
    public async Task DownloadFile()
    {
        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile(
            "https://download.visualstudio.microsoft.com/download/pr/a17b907f-8457-45a8-90db-53f2665ee49e/49bccd33593ebceb2847674fe5fd768e/aspnetcore-runtime-8.0.10-win-x64.exe",
            @"C:\Workspaces\"));
    }

    [HttpGet]
    public async Task DownloadFileWithProgress()
    {
        await httpRemoteService.DownloadFileAsync(
            "https://download.visualstudio.microsoft.com/download/pr/6224f00f-08da-4e7f-85b1-00d42c2bb3d3/b775de636b91e023574a0bbc291f705a/dotnet-sdk-8.0.403-win-x64.exe",
            @"C:\Workspaces\", async progress =>
            {
                Console.WriteLine(
                    $"Downloaded {progress.Transferred.ToSizeUnits("MB"):F2} MB of {progress.TotalFileSize.ToSizeUnits("MB"):F2} MB ({progress.PercentageComplete:F2}% complete, Speed: {progress.TransferRate.ToSizeUnits("MB"):F2} MB/s, Time: {progress.TimeElapsed.TotalSeconds:F2}s, ETA: {progress.EstimatedTimeRemaining.TotalSeconds:F2}s), File Name: {progress.FileName}, Destination Path: {progress.FileFullName}");
                await Task.CompletedTask;
            }, FileExistsBehavior.Overwrite); // FileExistsBehavior 可配置本地已存在该文件时的下载行为，此处是替换操作

        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile(
            "https://download.visualstudio.microsoft.com/download/pr/6224f00f-08da-4e7f-85b1-00d42c2bb3d3/b775de636b91e023574a0bbc291f705a/dotnet-sdk-8.0.403-win-x64.exe",
            @"C:\Workspaces\", async progress =>
            {
                Console.WriteLine(
                    $"Downloaded {progress.Transferred.ToSizeUnits("MB"):F2} MB of {progress.TotalFileSize.ToSizeUnits("MB"):F2} MB ({progress.PercentageComplete:F2}% complete, Speed: {progress.TransferRate.ToSizeUnits("MB"):F2} MB/s, Time: {progress.TimeElapsed.TotalSeconds:F2}s, ETA: {progress.EstimatedTimeRemaining.TotalSeconds:F2}s), File Name: {progress.FileName}, Destination Path: {progress.FileFullName}");
                await Task.CompletedTask;
            }, FileExistsBehavior.Overwrite)); // FileExistsBehavior 可配置本地已存在该文件时的下载行为，此处是替换操作
    }
}