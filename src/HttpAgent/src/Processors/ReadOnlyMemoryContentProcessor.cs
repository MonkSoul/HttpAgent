// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="ReadOnlyMemory{T}" /> 内容处理器
/// </summary>
public class ReadOnlyMemoryContentProcessor : IHttpContentProcessor
{
    /// <inheritdoc />
    public virtual bool CanProcess(object? rawContent, string contentType) =>
        rawContent is ReadOnlyMemoryContent or ReadOnlyMemory<byte>;

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

        // 检查是否是 ReadOnlyMemory<byte> 类型
        if (rawContent is ReadOnlyMemory<byte> readOnlyMemory)
        {
            // 初始化 ReadOnlyMemoryContent 实例
            var readOnlyMemoryContent = new ReadOnlyMemoryContent(readOnlyMemory);
            readOnlyMemoryContent.Headers.ContentType = new MediaTypeHeaderValue(contentType)
            {
                CharSet = encoding?.BodyName ?? Constants.UTF8_ENCODING
            };

            return readOnlyMemoryContent;
        }

        throw new InvalidOperationException(
            $"Expected a ReadOnlyMemory<byte>, but received an object of type `{rawContent.GetType()}`.");
    }
}