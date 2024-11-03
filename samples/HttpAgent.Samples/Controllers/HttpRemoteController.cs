﻿using HttpAgent.Samples.Models;

namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController(IHttpRemoteService httpRemoteService) : ControllerBase
{
    [HttpPost]
    public Task<HttpRemoteModel> AddModel(HttpRemoteModel model)
    {
        return Task.FromResult(model);
    }

    [HttpPost]
    public Task<HttpRemoteFormResult> AddForm([FromForm] HttpRemoteFormModel model)
    {
        var fileInfo = model.File?.FileName;
        return Task.FromResult(new HttpRemoteFormResult
        {
            FileInfo = fileInfo,
            Id = model.Id,
            Name = model.Name
        });
    }
}