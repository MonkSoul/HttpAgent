// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

/// <summary>
///     <c>application/msgpack</c> 内容处理器
/// </summary>
public class MessagePackContentProcessor : IHttpContentProcessor
{
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
        var content = rawContent as byte[] ?? MessagePackSerializer.Serialize(rawContent);

        // 初始化 ByteArrayContent 实例
        var byteArrayContent = new ByteArrayContent(content);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(contentType)
        {
            CharSet = encoding?.BodyName ?? Constants.UTF8_ENCODING
        };

        return byteArrayContent;
    }
}