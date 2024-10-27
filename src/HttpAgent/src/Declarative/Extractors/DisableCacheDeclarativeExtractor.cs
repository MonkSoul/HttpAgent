// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="DisableCacheAttribute" /> 特性提取器
/// </summary>
internal sealed class DisableCacheDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否定义了 [DisableCache] 特性
        if (!context.Method.IsDefined(typeof(DisableCacheAttribute), true))
        {
            return;
        }

        httpRequestBuilder.DisableCache(context.Method.GetCustomAttribute<DisableCacheAttribute>(true)!.Disabled);
    }
}