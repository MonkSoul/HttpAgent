﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="MultipartAttribute" /> 特性提取器
/// </summary>
internal sealed class MultipartDeclarativeExtractor : IFrozenHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeExtractorContext context)
    {
        // 查找所有贴有 [Multipart] 特性的参数
        var multipartParameters = context.UnFrozenParameters
            .Where(u => u.Key.IsDefined(typeof(MultipartAttribute), true)).ToArray();

        // 空检查
        if (multipartParameters is { Length: 0 })
        {
            return;
        }

        // 初始化 HttpMultipartFormDataBuilder 实例
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(httpRequestBuilder);

        // 设置多部分表单内容的边界
        SetBoundary(context.Method, httpMultipartFormDataBuilder);

        // 遍历所有贴有 [Multipart] 特性的参数
        foreach (var (parameter, value) in multipartParameters)
        {
            // 添加多部分表单内容
            AddMultipart(parameter, value, httpMultipartFormDataBuilder);
        }

        // 设置多部分表单内容
        httpRequestBuilder.SetMultipartContent(httpMultipartFormDataBuilder);
    }

    /// <inheritdoc />
    public int Order => 3;

    /// <summary>
    ///     设置多部分表单内容的边界
    /// </summary>
    /// <param name="method">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    internal static void SetBoundary(MethodInfo method, HttpMultipartFormDataBuilder httpMultipartFormDataBuilder)
    {
        // 检查方法是否定义了 MultipartFormAttribute 特性
        if (!method.IsDefined(typeof(MultipartFormAttribute), true))
        {
            return;
        }

        // 获取 MultipartFormAttribute 实例
        var multipartFormAttribute = method.GetCustomAttribute<MultipartFormAttribute>(true)!;

        // 空检查
        if (!string.IsNullOrWhiteSpace(multipartFormAttribute.Boundary))
        {
            // 多部分表单内容的边界
            httpMultipartFormDataBuilder.SetBoundary(multipartFormAttribute.Boundary);
        }
    }

    /// <summary>
    ///     添加多部分表单内容
    /// </summary>
    /// <param name="parameter">
    ///     <see cref="ParameterInfo" />
    /// </param>
    /// <param name="value">参数值</param>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    internal static void AddMultipart(ParameterInfo parameter, object? value,
        HttpMultipartFormDataBuilder httpMultipartFormDataBuilder)
    {
        // 判断参数是否为冻结参数
        if (HttpDeclarativeExtractorContext.IsFrozenParameter(parameter))
        {
            return;
        }

        // 获取 MultipartAttribute 实例
        var multipartAttribute = parameter.GetCustomAttribute<MultipartAttribute>(true)!;

        // 获取表单名称
        var name = multipartAttribute.Name ?? parameter.Name!;

        // 获取内容编码
        var contentEncoding = multipartAttribute.ContentEncoding is null
            ? null
            : Encoding.GetEncoding(multipartAttribute.ContentEncoding);

        switch (value)
        {
            // 添加流
            case Stream stream:
                httpMultipartFormDataBuilder.AddStream(stream, name, multipartAttribute.FileName,
                    multipartAttribute.ContentType ?? MediaTypeNames.Application.Octet, contentEncoding);
                break;
            // 添加字节数组
            case byte[] byteArray:
                httpMultipartFormDataBuilder.AddByteArray(byteArray, name, multipartAttribute.FileName,
                    multipartAttribute.ContentType ?? MediaTypeNames.Application.Octet, contentEncoding);
                break;
            // 添加 MultipartFile
            case MultipartFile multipartFile:
                httpMultipartFormDataBuilder.AddFile(multipartFile);
                break;
            // 添加 HttpContent
            case HttpContent httpContent:
                httpMultipartFormDataBuilder.Add(httpContent, name, multipartAttribute.ContentType,
                    contentEncoding);
                break;
            // 添加文件
            case string fileSource when multipartAttribute.AsFileFrom is not FileSourceType.None:
                AddFileFromSource(fileSource, name, multipartAttribute, httpMultipartFormDataBuilder, contentEncoding);
                break;
            // 添加单个表单项或对象
            default:
                AddFormItemOrObject(value, name, parameter.ParameterType, multipartAttribute,
                    httpMultipartFormDataBuilder, contentEncoding);
                break;
        }
    }

    /// <summary>
    ///     添加文件
    /// </summary>
    /// <param name="fileSource">文件的来源</param>
    /// <param name="name">表单名称</param>
    /// <param name="multipartAttribute">
    ///     <see cref="MultipartAttribute" />
    /// </param>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="contentEncoding">内容编码</param>
    internal static void AddFileFromSource(string fileSource, string name, MultipartAttribute multipartAttribute,
        HttpMultipartFormDataBuilder httpMultipartFormDataBuilder, Encoding? contentEncoding)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(fileSource, nameof(fileSource));

        // 获取内容类型
        var contentType = multipartAttribute.ContentType ?? MediaTypeNames.Application.Octet;

        // 获取文件的名称
        var fileName = multipartAttribute.FileName;

        switch (multipartAttribute.AsFileFrom)
        {
            // 从本地文件路径中添加
            case FileSourceType.Path:
                httpMultipartFormDataBuilder.AddFileAsStream(fileSource, name, fileName, contentType, contentEncoding);
                break;
            // 从 Base64 字符串文件中添加
            case FileSourceType.Base64String:
                httpMultipartFormDataBuilder.AddFileFromBase64String(fileSource, name, fileName, contentType,
                    contentEncoding);
                break;
            // 从互联网文件地址中添加
            case FileSourceType.Remote:
                httpMultipartFormDataBuilder.AddFileFromRemote(fileSource, name, fileName, contentType,
                    contentEncoding);
                break;
            // 不做任何操作
            case FileSourceType.None:
            case FileSourceType.ByteArray:
            case FileSourceType.Stream:
            default:
                break;
        }
    }

    /// <summary>
    ///     添加单个表单项或对象
    /// </summary>
    /// <param name="value">参数的值</param>
    /// <param name="name">表单名称</param>
    /// <param name="parameterType">参数类型</param>
    /// <param name="multipartAttribute">
    ///     <see cref="MultipartAttribute" />
    /// </param>
    /// <param name="httpMultipartFormDataBuilder">
    ///     <see cref="HttpMultipartFormDataBuilder" />
    /// </param>
    /// <param name="contentEncoding">内容编码</param>
    internal static void AddFormItemOrObject(object? value, string name, Type parameterType,
        MultipartAttribute multipartAttribute, HttpMultipartFormDataBuilder httpMultipartFormDataBuilder,
        Encoding? contentEncoding)
    {
        // 检查类型是否是基本类型或枚举类型或由它们组成的数组或集合类型
        if (parameterType.IsBaseTypeOrEnumOrCollection())
        {
            // 添加单个表单项内容
            httpMultipartFormDataBuilder.AddFormItem(value.ToCultureString(CultureInfo.InvariantCulture), name,
                contentEncoding);
        }
        // 添加原始内容
        else
        {
            httpMultipartFormDataBuilder.AddObject(value, multipartAttribute.AsFormItem ? name : null,
                multipartAttribute.ContentType ?? MediaTypeNames.Text.Plain, contentEncoding);
        }
    }
}