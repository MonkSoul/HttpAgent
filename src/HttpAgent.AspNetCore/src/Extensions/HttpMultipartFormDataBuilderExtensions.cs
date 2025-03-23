// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpMultipartFormDataBuilder" /> 拓展类
/// </summary>
public static class HttpMultipartFormDataBuilderExtensions
{
    /// <summary>
    ///     添加文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="formFile">
    ///     <see cref="IFormFile" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFile(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IFormFile formFile)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formFile);

        // 初始化 MemoryStream 实例
        var memoryStream = new MemoryStream();

        // 将 IFormFile 内容复制到内存流
        formFile.CopyTo(memoryStream);

        // 将内存流的位置重置到起始位置
        memoryStream.Position = 0;

        // 添加文件流
        return httpMultipartFormDataBuilder.AddStream(memoryStream, formFile.Name, formFile.FileName,
            formFile.ContentType, disposeStreamOnRequestCompletion: true);
    }

    /// <summary>
    ///     添加多个文件
    /// </summary>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="formFiles">
    ///     <see cref="IFormFileCollection" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public static HttpMultipartFormDataBuilder AddFiles(this HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        IEnumerable<IFormFile> formFiles)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formFiles);

        // 逐条添加文件
        foreach (var formFile in formFiles)
        {
            httpMultipartFormDataBuilder.AddFile(formFile);
        }

        return httpMultipartFormDataBuilder;
    }
}