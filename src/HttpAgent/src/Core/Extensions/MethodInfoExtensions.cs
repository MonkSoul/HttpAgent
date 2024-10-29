// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="MethodInfo" /> 拓展类
/// </summary>
internal static class MethodInfoExtensions
{
    /// <summary>
    ///     检查是否定义了指定特性
    /// </summary>
    /// <param name="method">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <param name="attribute">
    ///     <typeparamref name="TAttribute" />
    /// </param>
    /// <param name="inherit">是否在基类中搜索</param>
    /// <typeparam name="TAttribute">
    ///     <see cref="Attribute" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsDefined<TAttribute>(this MethodInfo method, [NotNullWhen(true)] out TAttribute? attribute,
        bool inherit = false)
        where TAttribute : Attribute
    {
        // 获取指定特性实例
        attribute = method.GetCustomAttribute<TAttribute>(inherit);

        // 检查是否定义了指定特性
        var isDefined = attribute != null || method.IsDefined(typeof(TAttribute), inherit);
        if (isDefined || !inherit)
        {
            return isDefined;
        }

        // 尝试查找所在声明类是否定义了指定特性
        attribute = method.DeclaringType?.GetCustomAttribute<TAttribute>(inherit);
        isDefined = attribute != null || method.DeclaringType?.IsDefined(typeof(TAttribute), inherit) == true;

        return isDefined;
    }

    /// <summary>
    ///     获取指定特性的所有实例
    /// </summary>
    /// <param name="method">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <param name="inherit">是否在基类中搜索</param>
    /// <param name="methodSearchFirst">是否优先查找 <see cref="MethodInfo"/> 的特性。默认值为：<c>true</c>。</param>
    /// <typeparam name="TAttribute">
    ///     <see cref="Attribute" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static TAttribute[]? GetDefinedCustomAttributes<TAttribute>(this MethodInfo method, bool inherit = false,
        bool methodSearchFirst = true)
        where TAttribute : Attribute
    {
        // 初始化指定特性集合
        var attributes = new List<TAttribute>();

        // 获取指定特性集合
        attributes.AddRange(method.GetCustomAttributes<TAttribute>(inherit));

        // 尝试获取所在声明类上指定特性集合
        // ReSharper disable once InvertIf
        if (inherit && method.DeclaringType is not null)
        {
            var declaringAttributes = method.DeclaringType.GetCustomAttributes<TAttribute>(inherit);

            // 是否优先查找方法特性
            if (methodSearchFirst)
            {
                attributes.AddRange(declaringAttributes);
            }
            // 否则添加到头部
            else
            {
                attributes.InsertRange(0, declaringAttributes);
            }
        }

        return attributes.Count > 0 ? attributes.ToArray() : null;
    }
}