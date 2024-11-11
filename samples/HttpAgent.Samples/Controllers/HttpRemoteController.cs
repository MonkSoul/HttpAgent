using HttpAgent.Samples.Models;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController : ControllerBase
{
    [HttpPost]
    public Task<YourRemoteModel> AddModel(int query1, string query2, YourRemoteModel model)
    {
        return Task.FromResult(model);
    }

    [HttpPost]
    public Task<YourRemoteFormResult> AddForm(int id, [FromForm] YourRemoteFormModel model)
    {
        var fileInfo = model.File?.FileName;
        return Task.FromResult(new YourRemoteFormResult
        {
            FileInfo = fileInfo,
            Id = model.Id,
            Name = model.Name
        });
    }

    [HttpPost]
    public Task<string> AddFile(IFormFile file)
    {
        return Task.FromResult(file.FileName);
    }

    [HttpPost]
    public Task<string> AddFiles(IFormFileCollection files)
    {
        return Task.FromResult(string.Join("; ", files.Select(u => u.FileName)));
    }
}