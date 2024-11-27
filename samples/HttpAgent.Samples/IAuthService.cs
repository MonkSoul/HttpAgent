namespace HttpAgent.Samples;

[Authentication]    // 添加全局授权
public interface IAuthService : IHttpDeclarative
{
    [Get("https://furion.net/")]
    Task<string> GetDataAsync();    // 访问这个接口需要授权

    [AllowAnonymous]    // 匿名访问
    [Get("https://furion.net/")]
    Task<string> LoginAsync(string username, string password);
}