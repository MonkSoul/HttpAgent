namespace HttpAgent.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class HttpRemoteController(IHttpRemoteService httpRemoteService) : ControllerBase
{
}