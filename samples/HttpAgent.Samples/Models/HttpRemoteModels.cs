namespace HttpAgent.Samples.Models;

public class HttpRemoteModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class HttpRemoteFormModel : HttpRemoteModel
{
    public IFormFile? File { get; set; }
}

public class HttpRemoteFormResult : HttpRemoteModel
{
    public string? FileInfo { get; set; }
}