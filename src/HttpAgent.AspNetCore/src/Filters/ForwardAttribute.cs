// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpContext" /> 转发操作筛选器
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ForwardAttribute : ActionFilterAttribute
{
    /// <summary>
    ///     <inheritdoc cref="ForwardAttribute" />
    /// </summary>
    /// <param name="requestUri">转发地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    public ForwardAttribute(string? requestUri) => RequestUri = requestUri;

    /// <summary>
    ///     <inheritdoc cref="ForwardAttribute" />
    /// </summary>
    /// <param name="requestUri">转发地址。若为空则尝试从请求标头 <c>X-Forward-To</c> 中获取目标地址。</param>
    /// <param name="httpMethod">转发方式</param>
    public ForwardAttribute(string? requestUri, HttpMethod httpMethod)
        : this(requestUri) =>
        Method = httpMethod;

    /// <summary>
    ///     转发地址
    /// </summary>
    public string? RequestUri { get; set; }

    /// <summary>
    ///     转发方式
    /// </summary>
    /// <remarks>若未设置，则自动采用当前请求方式作为转发方式。</remarks>
    public HttpMethod? Method { get; set; }

    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    /// <remarks>
    ///     <para>此属性用于指定 <see cref="IHttpClientFactory" /> 创建 <see cref="HttpClient" /> 实例时传递的名称。</para>
    ///     <para>该名称用于标识在服务容器中与特定 <see cref="HttpClient" /> 实例相关的配置。</para>
    /// </remarks>
    public string? HttpClientName { get; set; }

    /// <inheritdoc />
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 获取方法返回值类型
        var returnType = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.ReturnType;

        // 初始化最终返回值类型
        var finalReturnType = returnType == typeof(void) || returnType == typeof(Task)
            ? typeof(VoidContent)
            : typeof(Task<>).IsDefinitionEqual(returnType)
                ? returnType.GenericTypeArguments[0]
                : returnType;

        // 转发并获取结果
        var result = await context.HttpContext.ForwardAsAsync(finalReturnType,
            Method ?? Helpers.ParseHttpMethod(context.HttpContext.Request.Method), RequestUri,
            builder => builder.SetHttpClientName(HttpClientName ?? string.Empty));

        // 设置转发内容
        context.Result = result as IActionResult ?? new ObjectResult(result);
    }
}