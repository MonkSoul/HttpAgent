// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="Type" /> 拓展类
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    ///     检查类型是否是基本类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsBasicType(this Type type)
    {
        while (true)
        {
            // 如果是基元类型则直接返回
            if (type.IsPrimitive)
            {
                return true;
            }

            // 处理可空类型
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return type == typeof(string) || type == typeof(decimal) ||
                       type == typeof(Guid) ||
                       type == typeof(DateTime) ||
                       type == typeof(DateTimeOffset) || type == typeof(DateOnly) || type == typeof(TimeSpan) ||
                       type == typeof(TimeOnly) || type == typeof(char) || type == typeof(IntPtr) ||
                       type == typeof(UIntPtr);
            }

            var underlyingType = type.GetGenericArguments()[0];
            type = underlyingType;
        }
    }

    /// <summary>
    ///     检查类型是否是 <see cref="KeyValuePair{TKey,TValue}" /> 类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsKeyValuePair(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);

    /// <summary>
    ///     检查类型是否是键值对集合类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="isKeyValuePairCollection">是否是 <see cref="KeyValuePair{TKey,TValue}" /> 集合类型</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsKeyValueCollection(this Type type, out bool isKeyValuePairCollection)
    {
        isKeyValuePairCollection = false;

        // 如果类型不是一个集合类型则直接返回
        if (!typeof(IEnumerable).IsAssignableFrom(type))
        {
            return false;
        }

        // 如果是 Hashtable 或 NameValueCollection 则直接返回
        if (type == typeof(Hashtable) || type == typeof(NameValueCollection))
        {
            return true;
        }

        // 如果是 IDictionary<,> 类型则直接返回
        if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
        {
            isKeyValuePairCollection = type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          ((i.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                            i.GetGenericArguments()[0].IsKeyValuePair()) ||
                           (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                            i.GetGenericArguments()[0].IsKeyValuePair())));
            return true;
        }

        // 检查是否是 KeyValuePair<,> 数组类型
        if (type.IsArray)
        {
            // 获取数组元素类型
            var elementType = type.GetElementType();

            // 检查元素类型是否是 KeyValuePair<,> 类型
            if (elementType is null || !elementType.IsKeyValuePair())
            {
                return false;
            }

            isKeyValuePairCollection = true;
            return true;
        }

        // 检查是否是 KeyValuePair<,> 集合类型
        if (type is not { IsGenericType: true, GenericTypeArguments.Length: 1 }
            || !type.GenericTypeArguments[0].IsKeyValuePair())
        {
            return false;
        }

        isKeyValuePairCollection = true;
        return true;
    }

    /// <summary>
    ///     获取 <see cref="KeyValuePair{TKey,TValue}" /> 或 <c>Newtonsoft.Json.Linq.JProperty</c> 类型键值属性值访问器
    /// </summary>
    /// <param name="keyValuePairType">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="Tuple{T1,T2}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static (Func<object, object?> KeyGetter, Func<object, object?> ValueGetter)
        GetKeyValuePairOrJPropertyGetters(
            this Type keyValuePairType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keyValuePairType);

        // 检查类型是否是 KeyValuePair<,> 类型或者是 Newtonsoft.Json.Linq.JProperty 类型
        if (keyValuePairType.IsKeyValuePair() || keyValuePairType.FullName == "Newtonsoft.Json.Linq.JProperty")
        {
            // 反射搜索成员方式
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;

            // 创建 Key/Name 和 Value 属性值访问器
            var keyGetter =
                keyValuePairType.CreatePropertyGetter(keyValuePairType.GetProperty("Key", bindingAttr) ??
                                                      keyValuePairType.GetProperty("Name", bindingAttr)!);
            var valueGetter =
                keyValuePairType.CreatePropertyGetter(keyValuePairType.GetProperty("Value",
                    bindingAttr)!);

            return (keyGetter, valueGetter);
        }

        throw new InvalidOperationException(
            $"The type `{keyValuePairType}` is not a `KeyValuePair<,>` or `Newtonsoft.Json.Linq.JProperty` type.");
    }

    /// <summary>
    ///     创建实例属性值访问器
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <param name="propertyInfo">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Func{T1, T2}" />
    /// </returns>
    internal static Func<object, object?> CreatePropertyGetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_获取_属性名
        var dynamicMethod = new DynamicMethod(
            $"{type.FullName}_Get_{propertyInfo.Name}",
            typeof(object),
            [typeof(object)],
            typeof(TypeExtensions).Module,
            true
        );

        // 获取动态方法的 IL 生成器
        var ilGenerator = dynamicMethod.GetILGenerator();

        // 获取属性的获取方法，并允许非公开访问
        var getMethod = propertyInfo.GetGetMethod(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(getMethod);

        // 将目标对象加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);

        // 调用获取方法
        ilGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);

        // 如果属性类型为值类型，则装箱为 object 类型
        if (propertyInfo.PropertyType.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Func 类型
        return (Func<object, object?>)dynamicMethod.CreateDelegate(typeof(Func<object, object?>));
    }
}