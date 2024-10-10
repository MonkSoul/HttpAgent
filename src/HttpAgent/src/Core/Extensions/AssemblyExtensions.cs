// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Assembly" /> 拓展类
/// </summary>
internal static class AssemblyExtensions
{
    /// <summary>
    ///     获取程序集版本
    /// </summary>
    /// <param name="assembly">
    ///     <see cref="Assembly" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static Version? GetVersion(this Assembly assembly) => assembly.GetName().Version;

    /// <summary>
    ///     将程序集转换成指定类型返回
    /// </summary>
    /// <param name="assembly">
    ///     <see cref="Assembly" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    internal static TResult ConvertTo<TResult>(this Assembly assembly, Func<Assembly, TResult> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        return configure.Invoke(assembly);
    }
}