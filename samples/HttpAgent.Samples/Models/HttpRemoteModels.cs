namespace HttpAgent.Samples.Models;

public class YourRemoteModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class YourRemoteFormModel : YourRemoteModel
{
    public IFormFile? File { get; set; }

    public IFormFileCollection? Files { get; set; }
}

public class YourRemoteFormResult : YourRemoteModel
{
    public string? FileInfo { get; set; }
}