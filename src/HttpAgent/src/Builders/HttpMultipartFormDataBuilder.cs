// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="MultipartFormDataContent" /> 构建器
/// </summary>
public sealed class HttpMultipartFormDataBuilder
{
    /// <inheritdoc cref="HttpRequestBuilder" />
    internal readonly HttpRequestBuilder _httpRequestBuilder;

    /// <summary>
    ///     <see cref="MultipartFormDataItem" /> 集合
    /// </summary>
    internal readonly List<MultipartFormDataItem> _partContents;

    /// <summary>
    ///     <inheritdoc cref="HttpMultipartFormDataBuilder" />
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    internal HttpMultipartFormDataBuilder(HttpRequestBuilder httpRequestBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRequestBuilder);

        _httpRequestBuilder = httpRequestBuilder;
        _partContents = [];
    }

    /// <summary>
    ///     多部分内容表单的边界
    /// </summary>
    public string? Boundary { get; set; }

    /// <summary>
    ///     设置多部分内容表单的边界
    /// </summary>
    /// <param name="boundary">多部分内容表单的边界</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder SetBoundary(string boundary)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(boundary);

        Boundary = boundary;

        return this;
    }

    /// <summary>
    ///     添加 JSON 内容
    /// </summary>
    /// <param name="rawJson">JSON 字符串/原始对象</param>
    /// <param name="name">表单名称。该值不为空时作为表单的一项。否则将遍历对象类型的每一个公开属性作为表单的项。</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    /// <exception cref="JsonException"></exception>
    public HttpMultipartFormDataBuilder AddJson(object rawJson, string? name = null, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(rawJson);

        var rawObject = rawJson;

        // 检查是否配置表单名或不是字符串类型
        if (!string.IsNullOrWhiteSpace(name) || rawJson is not string rawString)
        {
            return AddRaw(rawObject, name, MediaTypeNames.Application.Json, contentEncoding);
        }

        // 尝试验证并获取 JsonDocument 实例（需 using）
        var jsonDocument = JsonUtility.Parse(rawString);
        rawObject = jsonDocument;

        // 添加请求结束时需要释放的对象
        _httpRequestBuilder.AddDisposable(jsonDocument);

        return AddRaw(rawObject, name, MediaTypeNames.Application.Json, contentEncoding);
    }

    /// <summary>
    ///     添加 JSON 单个属性值
    /// </summary>
    /// <param name="value">表单值</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddJsonProperty(object? value, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddRaw(value, name, MediaTypeNames.Text.Plain, contentEncoding);
    }

    /// <summary>
    ///     添加 HTML 内容
    /// </summary>
    /// <param name="htmlString">HTML 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddHtml(string? htmlString, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddRaw(htmlString, name, MediaTypeNames.Text.Html, contentEncoding);
    }

    /// <summary>
    ///     添加 XML 内容
    /// </summary>
    /// <param name="xmlString">XML 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddXml(string? xmlString, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddRaw(xmlString, name, MediaTypeNames.Application.Xml, contentEncoding);
    }

    /// <summary>
    ///     添加文本内容
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddText(string? text, string name, Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return AddRaw(text, name, MediaTypeNames.Text.Plain, contentEncoding);
    }

    /// <summary>
    ///     添加原始内容（字符串/对象）
    /// </summary>
    /// <param name="rawObject">字符串/原始对象</param>
    /// <param name="name">表单名称。该值不为空时作为表单的一项。否则将遍历对象类型的每一个公开属性作为表单的项。</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddRaw(object? rawObject, string? name = null,
        string contentType = "text/plain", Encoding? contentEncoding = null)
    {
        // 解析内容类型字符串
        var mediaType = ParseContentType(contentType, contentEncoding, out var encoding);

        // 检查是否配置表单名
        if (!string.IsNullOrWhiteSpace(name))
        {
            _partContents.Add(new MultipartFormDataItem(name)
            {
                ContentType = mediaType, RawContent = rawObject, ContentEncoding = encoding
            });

            return this;
        }

        // 空检查
        ArgumentNullException.ThrowIfNull(rawObject);

        // 将对象转换为 MultipartFormDataItem 集合再追加
        _partContents.AddRange(rawObject.ObjectToDictionary()!.Select(u =>
            new MultipartFormDataItem(u.Key.ToCultureString(CultureInfo.InvariantCulture)!)
            {
                ContentType = MediaTypeNames.Text.Plain, RawContent = u.Value, ContentEncoding = encoding
            }));

        return this;
    }

    /// <summary>
    ///     从互联网 URL 中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>50MB</c> 以内。</remarks>
    /// <param name="url">互联网 URL 地址</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileFromRemote(string url, string name, string? fileName = null,
        string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 尝试获取文件的名称
        var newFileName = fileName ?? Helpers.GetFileNameFromUri(new Uri(url, UriKind.Absolute));

        // 从互联网 URL 地址中加载流
        var (fileStream, fileLength) = Helpers.GetStreamFromRemote(url);

        // 添加文件流到请求结束时需要释放的集合中
        _httpRequestBuilder.AddDisposable(fileStream);

        return AddStream(fileStream, name, newFileName, fileLength, contentType, contentEncoding);
    }

    /// <summary>
    ///     从 Base64 字符串中添加文件
    /// </summary>
    /// <remarks>文件大小限制在 <c>50MB</c> 以内。</remarks>
    /// <param name="base64String">Base64 字符串</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileFromBase64String(string base64String, string name,
        string? fileName = null, string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(base64String);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 将 Base64 字符串转换成字节数组
        var bytes = Convert.FromBase64String(base64String);

        // 获取字节数组长度
        var fileLength = bytes.Length;

        return AddByteArray(bytes, name, fileName, fileLength, contentType, contentEncoding);
    }

    /// <summary>
    ///     从本地路径中添加文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileAsStream(string filePath, string name, string? fileName = null,
        string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件流（没有 using）
        var fileStream = File.OpenRead(filePath);

        // 获取文件信息
        var fileInfo = new FileInfo(filePath);
        var fileLength = fileInfo.Length;

        // 添加文件流到请求结束时需要释放的集合中
        _httpRequestBuilder.AddDisposable(fileStream);

        return AddStream(fileStream, name, newFileName, fileLength, contentType, contentEncoding);
    }

    /// <summary>
    ///     从本地路径中添加文件（带上传进度）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="progressChannel">文件传输进度信息的通道</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileWithProgressAsStream(string filePath, string name,
        Channel<FileTransferProgress> progressChannel, string? fileName = null,
        string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(progressChannel);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件流（没有 using）
        var fileStream = File.OpenRead(filePath);

        // 获取文件信息
        var fileInfo = new FileInfo(filePath);
        var fileLength = fileInfo.Length;

        // 初始化带读写进度的文件流
        var progressFileStream = new ProgressFileStream(fileStream, filePath, fileLength, progressChannel, newFileName);

        // 添加文件流到请求结束时需要释放的集合中
        _httpRequestBuilder.AddDisposable(progressFileStream);

        return AddStream(progressFileStream, name, newFileName, fileLength, contentType, contentEncoding);
    }

    /// <summary>
    ///     从本地路径中添加文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFileAsByteArray(string filePath, string name, string? fileName = null,
        string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The specified file `{filePath}` does not exist.");
        }

        // 获取文件的名称
        var newFileName = fileName ?? Path.GetFileName(filePath);

        // 读取文件字节数组
        var bytes = File.ReadAllBytes(filePath);

        // 获取文件信息
        var fileInfo = new FileInfo(filePath);
        var fileLength = fileInfo.Length;

        return AddByteArray(bytes, name, newFileName, fileLength, contentType, contentEncoding);
    }

    /// <summary>
    ///     添加流
    /// </summary>
    /// <param name="stream">
    ///     <see cref="Stream" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="fileSize">文件大小</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddStream(Stream stream, string name, string? fileName = null,
        long? fileSize = null, string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 解析内容类型字符串
        var mediaType = ParseContentType(contentType, contentEncoding, out var encoding);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = mediaType,
            RawContent = stream,
            ContentEncoding = encoding,
            FileName = fileName,
            FileSize = fileSize
        });

        return this;
    }

    /// <summary>
    ///     添加字节数组
    /// </summary>
    /// <param name="byteArray">字节数组</param>
    /// <param name="name">表单名称</param>
    /// <param name="fileName">文件的名称</param>
    /// <param name="fileSize">文件大小</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddByteArray(byte[] byteArray, string name, string? fileName = null,
        long? fileSize = null, string contentType = "application/octet-stream", Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(byteArray);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 解析内容类型字符串
        var mediaType = ParseContentType(contentType, contentEncoding, out var encoding);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = mediaType,
            RawContent = byteArray,
            ContentEncoding = encoding,
            FileName = fileName,
            FileSize = fileSize
        });

        return this;
    }

    /// <summary>
    ///     添加 URL 编码的键值对表单
    /// </summary>
    /// <param name="rawObject">原始对象</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="useStringContent">
    ///     是否使用 <see cref="StringContent" /> 构建
    ///     <see cref="FormUrlEncodedContent" />。默认 <c>false</c>。
    /// </param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddFormUrlEncoded(object? rawObject, string name,
        Encoding? contentEncoding = null, bool useStringContent = false)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = MediaTypeNames.Application.FormUrlEncoded,
            RawContent = rawObject,
            ContentEncoding = contentEncoding
        });

        // 检查是否启用 StringContent 方式构建 application/x-www-form-urlencoded 请求内容
        if (useStringContent)
        {
            _httpRequestBuilder.AddHttpContentProcessors(() => [new StringContentForFormUrlEncodedContentProcessor()]);
        }

        return this;
    }

    /// <summary>
    ///     添加多部分内容表单
    /// </summary>
    /// <param name="rawObject">原始对象</param>
    /// <param name="name">表单名称</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder AddMultipartFormData(object? rawObject, string name,
        Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _partContents.Add(new MultipartFormDataItem(name)
        {
            ContentType = MediaTypeNames.Multipart.FormData,
            RawContent = rawObject,
            ContentEncoding = contentEncoding
        });

        return this;
    }

    /// <summary>
    ///     添加 <see cref="HttpContent" />
    /// </summary>
    /// <param name="httpContent">
    ///     <see cref="HttpContent" />
    /// </param>
    /// <param name="name">表单名称</param>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <returns>
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </returns>
    public HttpMultipartFormDataBuilder Add(HttpContent httpContent, string? name, string? contentType,
        Encoding? contentEncoding = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpContent);

        // 尝试从 ContentDisposition 中解析 Name
        var formName = string.IsNullOrWhiteSpace(name) ? httpContent.Headers.ContentDisposition?.Name : name;

        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(formName, nameof(name));

        string? mediaType;
        Encoding? encoding = null;
        MediaTypeHeaderValue? mediaTypeHeaderValue = null;

        // 空检查
        if (contentType is not null)
        {
            mediaType = ParseContentType(contentType, contentEncoding, out encoding);
        }
        else
        {
            mediaTypeHeaderValue = httpContent.Headers.ContentType;
            mediaType = mediaTypeHeaderValue?.MediaType;
        }

        // 空检查
        ArgumentNullException.ThrowIfNull(mediaType);

        // 设置或解析内容编码
        encoding = contentEncoding ?? encoding ?? (string.IsNullOrWhiteSpace(mediaTypeHeaderValue?.CharSet)
            ? null
            : Encoding.GetEncoding(mediaTypeHeaderValue.CharSet));

        _partContents.Add(new MultipartFormDataItem(formName)
        {
            ContentType = mediaType, RawContent = httpContent, ContentEncoding = encoding
            // FileName = httpContent.Headers.ContentDisposition?.FileName
        });

        return this;
    }

    /// <summary>
    ///     构建 <see cref="MultipartFormDataContent" /> 实例
    /// </summary>
    /// <param name="httpRemoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 集合</param>
    /// <returns>
    ///     <see cref="MultipartFormDataContent" />
    /// </returns>
    internal MultipartFormDataContent? Build(HttpRemoteOptions httpRemoteOptions,
        IHttpContentProcessorFactory httpContentProcessorFactory,
        params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpRemoteOptions);
        ArgumentNullException.ThrowIfNull(httpContentProcessorFactory);

        // 空检查
        if (_partContents.IsNullOrEmpty())
        {
            return null;
        }

        // 获取多部分内容表单的边界；注意：这里可能出现前后双引号问题
        var boundary = Boundary?.TrimStart('"').TrimEnd('"');

        // 初始化 multipartFormDataContent 实例
        var multipartFormDataContent = string.IsNullOrWhiteSpace(boundary)
            ? new MultipartFormDataContent()
            : new MultipartFormDataContent(boundary);

        // 逐条遍历添加
        foreach (var dataItem in _partContents)
        {
            // 构建 HttpContent 实例
            var httpContent = BuildHttpContent(dataItem, httpContentProcessorFactory, processors);

            // 空检查
            if (httpContent is not null)
            {
                multipartFormDataContent.Add(httpContent, dataItem.Name);
            }
        }

        return multipartFormDataContent;
    }

    /// <summary>
    ///     构建 <see cref="HttpContent" /> 实例
    /// </summary>
    /// <param name="multipartFormDataItem">
    ///     <see cref="MultipartFormDataItem" />
    /// </param>
    /// <param name="httpContentProcessorFactory">
    ///     <see cref="IHttpContentProcessorFactory" />
    /// </param>
    /// <param name="processors"><see cref="IHttpContentProcessor" /> 集合</param>
    /// <returns>
    ///     <see cref="HttpContent" />
    /// </returns>
    internal static HttpContent? BuildHttpContent(MultipartFormDataItem multipartFormDataItem,
        IHttpContentProcessorFactory httpContentProcessorFactory, params IHttpContentProcessor[]? processors)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(multipartFormDataItem);
        ArgumentNullException.ThrowIfNull(httpContentProcessorFactory);

        // 空检查
        var contentType = multipartFormDataItem.ContentType;
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        // 构建 HttpContent 实例
        var httpContent = httpContentProcessorFactory.BuildHttpContent(multipartFormDataItem.RawContent, contentType,
            multipartFormDataItem.ContentEncoding, processors);

        // 处理 ByteArrayContent、StreamContent 和 ReadOnlyMemoryContent 类型文件的名称
        if (httpContent is ByteArrayContent and not (FormUrlEncodedContent or StringContent) or StreamContent
                or ReadOnlyMemoryContent && httpContent.Headers.ContentDisposition is null &&
            !string.IsNullOrWhiteSpace(multipartFormDataItem.FileName))
        {
            httpContent.Headers.ContentDisposition =
                new ContentDispositionHeaderValue(Constants.FORM_DATA_DISPOSITION_TYPE)
                {
                    Name = multipartFormDataItem.Name,
                    FileName = multipartFormDataItem.FileName,
                    Size = multipartFormDataItem.FileSize
                };
        }

        return httpContent;
    }

    /// <summary>
    ///     解析内容类型字符串
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    /// <param name="encoding">内容编码</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? ParseContentType(string contentType, Encoding? contentEncoding, out Encoding? encoding)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        // 解析内容类型字符串
        var mediaTypeHeaderValue = MediaTypeHeaderValue.Parse(contentType);

        // 解析/设置内容编码
        encoding = contentEncoding ?? (!string.IsNullOrWhiteSpace(mediaTypeHeaderValue.CharSet)
            ? Encoding.GetEncoding(mediaTypeHeaderValue.CharSet)
            : contentEncoding);

        return mediaTypeHeaderValue.MediaType;
    }
}