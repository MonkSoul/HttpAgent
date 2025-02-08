var services = new ServiceCollection();

// 注册 HTTP 远程请求服务
services.AddHttpRemote();

// 构建 IServiceProvider 实例并获取 IHttpRemoteService 服务实例
await using var serviceProvider = services.BuildServiceProvider();
var httpRemoteService = serviceProvider.GetRequiredService<IHttpRemoteService>();

// 使用
var content = await httpRemoteService.GetAsStringAsync("https://furion.net");
Console.WriteLine(content);