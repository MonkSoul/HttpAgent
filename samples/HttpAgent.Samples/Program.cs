using System.Reflection;
using HttpAgent.Samples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpRemote(options =>
{
    // 注册单个 HTTP 声明式请求接口
    options.AddHttpDeclarative<ISampleService>();

    // 扫描程序集批量注册 HTTP 声明式请求接口（推荐此方式注册）
    options.AddHttpDeclarativeFromAssemblies([Assembly.GetEntryAssembly()]);
});

var app = builder.Build();

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