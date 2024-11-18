using System.Net;
using System.Reflection;
using HttpAgent;
using HttpAgent.AspNetCore.Extensions;
using HttpAgent.Samples;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var cookieContainer = new CookieContainer();
cookieContainer.Add(new Uri("https://furion.net"), new Cookie("cookieName", "cookieValue"));

// 为默认客户端启用
builder.Services.AddHttpClient(string.Empty)
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        CookieContainer = cookieContainer,
        UseCookies = false,
        // 忽略 SSL 证书验证
        ServerCertificateCustomValidationCallback = HttpRemoteUtility.IgnoreSslErrors,
        SslProtocols = HttpRemoteUtility.AllSslProtocols
    })
    .AddProfilerDelegatingHandler();

// 为特定客户端启用
//builder.Services.AddHttpClient("weixin")
//    .AddProfilerDelegatingHandler();

builder.Services.TryAddTransient<AuthorizationDelegatingHandler>();

builder.Services.AddHttpRemote(options =>
{
    // 注册单个 HTTP 声明式请求接口
    options.AddHttpDeclarative<ISampleService>();

    // 扫描程序集批量注册 HTTP 声明式请求接口（推荐此方式注册）
    options.AddHttpDeclarativeFromAssemblies([Assembly.GetEntryAssembly()]);
});

var app = builder.Build();

app.UseEnableBuffering();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();