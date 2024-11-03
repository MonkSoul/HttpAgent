﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <c>application/msgpack</c> 内容处理器
/// </summary>
/// <remarks>要使用 <c>application/msgpack</c> 内容处理器需在项目中安装 <c>MessagePack</c> 依赖包。https://www.nuget.org/packages/MessagePack。</remarks>
public class MessagePackContentProcessor : IHttpContentProcessor
{
    /// <summary>
    ///     MessagePack 序列化器委托字典缓存
    /// </summary>
    internal static readonly ConcurrentDictionary<Type, Func<object, byte[]>> _serializerCache = new();

    /// <summary>
    ///     初始化 MessagePack 序列化器委托
    /// </summary>
    internal static readonly Lazy<Func<object, byte[]>> _messagePackSerializerLazy = new(() =>
    {
        // 尝试加载 MessagePack 包中的 MessagePackSerializer 类型
        var messagePackSerializerType = Type.GetType("MessagePack.MessagePackSerializer, MessagePack");

        // 空检查
        if (messagePackSerializerType is null)
        {
            throw new InvalidOperationException("Please ensure the `MessagePack` package is installed.");
        }

        // 查找方法：public static byte[] Serialize<T>(T, MessagePackSerializerOptions?, CancellationToken);
        var serializeMethod = messagePackSerializerType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .SingleOrDefault(u =>
                u is { Name: "Serialize", IsGenericMethod: true } && u.ReturnType == typeof(byte[]) &&
                u.GetParameters().Length == 3 &&
                u.GetGenericArguments().Length == 1)!;

        // 返回调用委托
        return CreateSerializerDelegate(serializeMethod);
    });

    /// <summary>
    ///     MessagePack 序列化器委托
    /// </summary>
    internal Func<object, byte[]> _messagePackSerializer => _messagePackSerializerLazy.Value;

    /// <inheritdoc />
    public virtual bool CanProcess(object? rawContent, string contentType) =>
        contentType.IsIn(["application/msgpack"], StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public virtual HttpContent? Process(object? rawContent, string contentType, Encoding? encoding)
    {
        // 跳过空值和 HttpContent 类型
        switch (rawContent)
        {
            case null:
                return null;
            case HttpContent httpContent:
                // 设置 Content-Type
                httpContent.Headers.ContentType ??=
                    new MediaTypeHeaderValue(contentType) { CharSet = encoding?.BodyName ?? Constants.UTF8_ENCODING };

                return httpContent;
        }

        // 将原始请求内容转换为字节数组
        var content = rawContent as byte[] ?? _messagePackSerializer(rawContent);

        // 初始化 ByteArrayContent 实例
        var byteArrayContent = new ByteArrayContent(content);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType)
        {
            CharSet = encoding?.BodyName ?? Constants.UTF8_ENCODING
        };

        return byteArrayContent;
    }

    /// <summary>
    ///     创建 MessagePack 序列化器委托
    /// </summary>
    /// <param name="serializeMethod">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Func{T1, T2}" />
    /// </returns>
    internal static Func<object, byte[]> CreateSerializerDelegate(MethodInfo serializeMethod)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serializeMethod);

        return obj =>
        {
            // 获取对象类型
            var objType = obj.GetType();

            // 查找 MessagePack 序列化器委托字典缓存是否存在该类型
            if (_serializerCache.TryGetValue(objType, out var serializer))
            {
                return serializer(obj);
            }

            // 创建 MessagePack 序列化器委托
            serializer = o =>
                (byte[])serializeMethod.MakeGenericMethod(objType).Invoke(null, [o, null, default(CancellationToken)])!;

            // 添加到 MessagePack 序列化器委托字典缓存中
            _serializerCache.TryAdd(objType, serializer);

            return serializer(obj);
        };
    }
}