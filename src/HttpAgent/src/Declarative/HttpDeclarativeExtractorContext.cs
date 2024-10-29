// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式提取器上下文
/// </summary>
public sealed class HttpDeclarativeExtractorContext
{
    /// <summary>
    ///     特殊参数类型
    /// </summary>
    internal static Type[] _specialParameterTypes =
    [
        typeof(Action<HttpRequestBuilder>), typeof(Action<HttpMultipartFormDataBuilder>), typeof(HttpCompletionOption),
        typeof(CancellationToken)
    ];

    /// <summary>
    ///     <inheritdoc cref="HttpDeclarativeExtractorContext" />
    /// </summary>
    /// <param name="method">被调用方法</param>
    /// <param name="args">被调用方法的参数值数组</param>
    internal HttpDeclarativeExtractorContext(MethodInfo method, object?[] args)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(args);

        Method = method;
        Args = args;

        // 初始化被调用方法的参数键值字典
        Parameters = method.GetParameters().Select((p, i) => new { Parameter = p, Value = args[i] })
            .ToDictionary(u => u.Parameter, u => u.Value).AsReadOnly();
    }

    /// <summary>
    ///     被调用方法
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    ///     被调用方法的参数值数组
    /// </summary>
    public object?[] Args { get; }

    /// <summary>
    ///     被调用方法的参数键值字典
    /// </summary>
    public IReadOnlyDictionary<ParameterInfo, object?> Parameters { get; }

    /// <summary>
    ///     过滤特殊参数类型
    /// </summary>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool FilterSpecialParameter(ParameterInfo parameter)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(parameter);

        return !_specialParameterTypes.Contains(parameter.ParameterType);
    }
}