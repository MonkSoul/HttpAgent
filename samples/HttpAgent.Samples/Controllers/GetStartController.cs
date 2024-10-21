namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController(IHttpRemoteService httpRemoteService) : ControllerBase
{
    [HttpGet]
    public async Task<string?> GetBaiduHtml()
    {
        return await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Get("https://www.baidu.com"));
    }

    [HttpGet]
    public async Task<string?> GetBaiduHtml2()
    {
        return await httpRemoteService.GetAsAsync<string>("https://www.baidu.com");
    }

    [HttpGet]
    public async Task DownloadFile()
    {
        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile(
            "https://cdn-cn.xterminal.cn/downloads/XTerminal-1.31.4-win-x64-installer.exe", @"C:\Workspaces\",
            fileExistsBehavior: FileExistsBehavior.Overwrite));
    }

    [HttpGet]
    public async Task DownloadFile2()
    {
        await httpRemoteService.SendAsync(HttpRequestBuilder.DownloadFile(
            "https://cdn-cn.xterminal.cn/downloads/XTerminal-1.31.4-win-x64-installer.exe", @"C:\Workspaces\",
            async progress =>
            {
                Console.WriteLine(
                    $"Downloaded {progress.Transferred.ToSizeUnits("MB"):F2} MB of {progress.TotalFileSize.ToSizeUnits("MB"):F2} MB ({progress.PercentageComplete:F2}% complete, Speed: {progress.TransferRate.ToSizeUnits("MB"):F2} MB/s, Time: {progress.TimeElapsed.TotalSeconds:F2}s, ETA: {progress.EstimatedTimeRemaining.TotalSeconds:F2}s), File Name: {progress.FileName}, Destination Path: {progress.FileFullName}");
                await Task.CompletedTask;
            }, FileExistsBehavior.Overwrite));
    }

    [HttpGet]
    public async Task DownloadFile3()
    {
        await httpRemoteService.DownloadFileAsync(
            "https://cdn-cn.xterminal.cn/downloads/XTerminal-1.31.4-win-x64-installer.exe", @"C:\Workspaces\",
            async progress =>
            {
                Console.WriteLine(
                    $"Downloaded {progress.Transferred.ToSizeUnits("MB"):F2} MB of {progress.TotalFileSize.ToSizeUnits("MB"):F2} MB ({progress.PercentageComplete:F2}% complete, Speed: {progress.TransferRate.ToSizeUnits("MB"):F2} MB/s, Time: {progress.TimeElapsed.TotalSeconds:F2}s, ETA: {progress.EstimatedTimeRemaining.TotalSeconds:F2}s), File Name: {progress.FileName}, Destination Path: {progress.FileFullName}");
                await Task.CompletedTask;
            }, FileExistsBehavior.Overwrite);
    }

    [HttpGet]
    public async Task<string?> Profiler()
    {
        return await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Get("https://www.baidu.com").Profiler());
    }
}