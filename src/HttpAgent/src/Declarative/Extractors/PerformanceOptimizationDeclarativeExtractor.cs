// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="PerformanceOptimizationAttribute" /> 特性提取器
/// </summary>
internal sealed class PerformanceOptimizationDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 检查方法或接口是否贴有 [PerformanceOptimization] 特性
        if (!context.IsMethodDefined<PerformanceOptimizationAttribute>(out var performanceOptimizationAttribute, true))
        {
            return;
        }

        // 设置是否启用性能优化
        httpRequestBuilder.PerformanceOptimization(performanceOptimizationAttribute.Enabled);
    }
}