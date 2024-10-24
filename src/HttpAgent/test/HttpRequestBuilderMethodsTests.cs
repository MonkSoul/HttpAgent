﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderMethodsTests
{
    [Fact]
    public void SetTraceIdentifier_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetTraceIdentifier(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetTraceIdentifier(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetTraceIdentifier(" ");
        });
    }

    [Fact]
    public void SetTraceIdentifier_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetTraceIdentifier("furion");
        Assert.Equal("furion", httpRequestBuilder.TraceIdentifier);

        httpRequestBuilder.SetTraceIdentifier("furi on");
        Assert.Equal("furi on", httpRequestBuilder.TraceIdentifier);

        httpRequestBuilder.SetTraceIdentifier("furi on", true);
        Assert.Equal("furi%20on", httpRequestBuilder.TraceIdentifier);
    }

    [Fact]
    public void SetContentType_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetContentType(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetContentType(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetContentType(" ");
        });

        Assert.Throws<FormatException>(() =>
        {
            httpRequestBuilder.SetContentType("unknown");
        });
    }

    [Fact]
    public void SetContentType_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetContentType("text/plain");
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetContentType("text/html;charset=utf-8");
        Assert.Equal("text/html", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetContentType("text/html; charset=unicode");
        Assert.Equal("text/html", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.Unicode, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetContentEncoding_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetContentEncoding((Encoding)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetContentEncoding((string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetContentEncoding(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetContentEncoding(" ");
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetContentEncoding("gbk");
        });
    }

    [Fact]
    public void SetContentEncoding_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetContentEncoding(Encoding.UTF32);
        Assert.Equal(Encoding.UTF32, httpRequestBuilder.ContentEncoding);

        // 该代码会影响全局获取编码测试
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        httpRequestBuilder.SetContentEncoding("gbk");
        Assert.Equal("gb2312", httpRequestBuilder.ContentEncoding?.BodyName);

        httpRequestBuilder.SetContentEncoding("gb2312");
        Assert.Equal("gb2312", httpRequestBuilder.ContentEncoding?.BodyName);
    }

    [Fact]
    public void SetJsonContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetJsonContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Equal("application/json", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetJsonContent(new { id = 1, name = "furion" });
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.Equal("application/json", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetHtmlContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetHtmlContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Equal("text/html", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetHtmlContent("<html><head></head><body></body></html>");
        Assert.Equal("<html><head></head><body></body></html>", httpRequestBuilder.RawContent);
        Assert.Equal("text/html", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetXmlContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetXmlContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Equal("application/xml", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetXmlContent("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        Assert.Equal("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", httpRequestBuilder.RawContent);
        Assert.Equal("application/xml", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetTextContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetTextContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetTextContent("furion");
        Assert.Equal("furion", httpRequestBuilder.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetFormUrlEncodedContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetFormUrlEncodedContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetFormUrlEncodedContent(new { id = 1, name = "furion" });
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);
        Assert.Null(httpRequestBuilder.HttpContentProcessorProviders);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.SetFormUrlEncodedContent(new { id = 1, name = "furion" }, useStringContent: true);
        Assert.NotNull(httpRequestBuilder2.HttpContentProcessorProviders);
        Assert.Single(httpRequestBuilder2.HttpContentProcessorProviders);
        Assert.Equal(typeof(StringContentForFormUrlEncodedContentProcessor),
            httpRequestBuilder2.HttpContentProcessorProviders[0].Invoke().First().GetType());
    }

    [Fact]
    public void SetRawContent_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<FormatException>(() =>
        {
            httpRequestBuilder.SetRawContent(null, "unknown");
        });
    }

    [Fact]
    public void SetRawContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetRawContent(null);
        Assert.Null(httpRequestBuilder.RawContent);
        Assert.Null(httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetRawContent("furion", "text/plain");
        Assert.Equal("furion", httpRequestBuilder.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF8, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetRawContent("furion", "text/plain", Encoding.UTF32);
        Assert.Equal("furion", httpRequestBuilder.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.UTF32, httpRequestBuilder.ContentEncoding);

        httpRequestBuilder.SetRawContent("furion", "text/plain;charset=unicode");
        Assert.Equal("furion", httpRequestBuilder.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder.ContentType);
        Assert.Equal(Encoding.Unicode, httpRequestBuilder.ContentEncoding);
    }

    [Fact]
    public void SetMultipartContent_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetMultipartContent((Action<HttpMultipartFormDataBuilder>)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetMultipartContent((HttpMultipartFormDataBuilder)null!);
        });
    }

    [Fact]
    public void SetMultipartContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetMultipartContent(builder =>
        {
            builder.AddJsonProperty(new { }, "name");
        });

        Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.SetMultipartContent(
            new HttpMultipartFormDataBuilder(httpRequestBuilder2).AddJsonProperty(new { }, "name"));
        Assert.NotNull(httpRequestBuilder2.MultipartFormDataBuilder);
    }

    [Fact]
    public void WithHeaders_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithHeaders(null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithHeaders((object)null!);
        });
    }

    [Fact]
    public void WithHeaders_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.WithHeaders(new Dictionary<string, object?> { ["id"] = 10, ["name"] = "furion" });
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Equal(2, httpRequestBuilder.Headers.Count);
        Assert.Equal("10", httpRequestBuilder.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder.Headers["name"].First());

        httpRequestBuilder.Headers.Clear();

        httpRequestBuilder.WithHeaders(new { id = 10, name = "furion" });
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Equal(2, httpRequestBuilder.Headers.Count);
        Assert.Equal("10", httpRequestBuilder.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder.Headers["name"].First());

        httpRequestBuilder.WithHeaders(new { age = 30 });
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Equal(3, httpRequestBuilder.Headers.Count);
        Assert.Equal("10", httpRequestBuilder.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder.Headers["name"].First());
        Assert.Equal("30", httpRequestBuilder.Headers["age"].First());

        httpRequestBuilder.Headers.Clear();

        httpRequestBuilder.WithHeaders(new { name = "furi on" });
        Assert.Equal("furi on", httpRequestBuilder.Headers["name"].First());

        httpRequestBuilder.Headers.Clear();

        httpRequestBuilder.WithHeaders(new { name = "furi on" }, true);
        Assert.Equal("furi%20on", httpRequestBuilder.Headers["name"].First());

        httpRequestBuilder.Headers.Clear();

        var dateNow = new DateTime(2024, 08, 30, 23, 59, 59, 999, DateTimeKind.Local);
        httpRequestBuilder.WithHeaders(new { date = dateNow }, false, CultureInfo.InvariantCulture);
        Assert.Equal("2024-08-30T23:59:59.9990000+08:00", httpRequestBuilder.Headers["date"].First());

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder2.WithHeaders(new { name = "furion" }, comparer: StringComparer.OrdinalIgnoreCase);
        Assert.Equal("furion", httpRequestBuilder2.Headers!["name"].First());
        Assert.Equal("furion", httpRequestBuilder2.Headers!["Name"].First());

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder3.WithHeaders(new { name = "furion" }).WithHeaders(new { id = 10, name = "furion2" });
        Assert.Equal("10", httpRequestBuilder3.Headers!["id"].First());
        Assert.Equal(2, httpRequestBuilder3.Headers!["name"].Count);
        Assert.Equal("furion", httpRequestBuilder3.Headers!["name"].First());
        Assert.Equal("furion2", httpRequestBuilder3.Headers!["name"].Last());
    }

    [Fact]
    public void RemoveHeaders_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.RemoveHeaders((string[])null!));
    }

    [Fact]
    public void RemoveHeaders_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.RemoveHeaders(null!, string.Empty, " ");

        Assert.NotNull(httpRequestBuilder.HeadersToRemove);
        Assert.Empty(httpRequestBuilder.HeadersToRemove);

        httpRequestBuilder.RemoveHeaders("Set-Cookie");
        httpRequestBuilder.RemoveHeaders("set-cookie");
        Assert.Single(httpRequestBuilder.HeadersToRemove);
    }

    [Fact]
    public void SetFragment_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetFragment(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetFragment(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            httpRequestBuilder.SetFragment(" ");
        });
    }

    [Fact]
    public void SetFragment_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetFragment("furion");
        Assert.Equal("furion", httpRequestBuilder.Fragment);

        httpRequestBuilder.SetFragment("furi on");
        Assert.Equal("furi on", httpRequestBuilder.Fragment);

        httpRequestBuilder.SetFragment("furi on", true);
        Assert.Equal("furi%20on", httpRequestBuilder.Fragment);
    }

    [Fact]
    public void SetTimeout_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            httpRequestBuilder.SetTimeout(-1);
        });

        Assert.Equal("Timeout value must be non-negative. (Parameter 'timeoutMilliseconds')", exception.Message);
    }

    [Fact]
    public void SetTimeout_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetTimeout(TimeSpan.MaxValue);
        Assert.Equal(TimeSpan.MaxValue, httpRequestBuilder.Timeout);

        httpRequestBuilder.SetTimeout(1000);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), httpRequestBuilder.Timeout);

        httpRequestBuilder.SetTimeout(0);
        Assert.Equal(TimeSpan.Zero, httpRequestBuilder.Timeout);
    }

    [Fact]
    public void WithQueryParameters_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithQueryParameters(null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithQueryParameters((object)null!);
        });
    }

    [Fact]
    public void WithQueryParameters_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.WithQueryParameters(new Dictionary<string, object?> { ["id"] = 10, ["name"] = "furion" });
        Assert.NotNull(httpRequestBuilder.QueryParameters);
        Assert.Equal(2, httpRequestBuilder.QueryParameters.Count);
        Assert.Equal("10", httpRequestBuilder.QueryParameters["id"].First());
        Assert.Equal("furion", httpRequestBuilder.QueryParameters["name"].First());

        httpRequestBuilder.QueryParameters.Clear();

        httpRequestBuilder.WithQueryParameters(new { id = 10, name = "furion" });
        Assert.NotNull(httpRequestBuilder.QueryParameters);
        Assert.Equal(2, httpRequestBuilder.QueryParameters.Count);
        Assert.Equal("10", httpRequestBuilder.QueryParameters["id"].First());
        Assert.Equal("furion", httpRequestBuilder.QueryParameters["name"].First());

        httpRequestBuilder.WithQueryParameters(new { age = 30 });
        Assert.NotNull(httpRequestBuilder.QueryParameters);
        Assert.Equal(3, httpRequestBuilder.QueryParameters.Count);
        Assert.Equal("10", httpRequestBuilder.QueryParameters["id"].First());
        Assert.Equal("furion", httpRequestBuilder.QueryParameters["name"].First());
        Assert.Equal("30", httpRequestBuilder.QueryParameters["age"].First());

        httpRequestBuilder.QueryParameters.Clear();

        httpRequestBuilder.WithQueryParameters(new { name = "furi on" });
        Assert.Equal("furi on", httpRequestBuilder.QueryParameters["name"].First());

        httpRequestBuilder.QueryParameters.Clear();

        httpRequestBuilder.WithQueryParameters(new { name = "furi on" }, true);
        Assert.Equal("furi%20on", httpRequestBuilder.QueryParameters["name"].First());

        httpRequestBuilder.QueryParameters.Clear();

        var dateNow = new DateTime(2024, 08, 30, 23, 59, 59, 999, DateTimeKind.Local);
        httpRequestBuilder.WithQueryParameters(new { date = dateNow }, false, CultureInfo.InvariantCulture);
        Assert.Equal("2024-08-30T23:59:59.9990000+08:00", httpRequestBuilder.QueryParameters["date"].First());

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder2.WithQueryParameters(new { name = "furion" }, comparer: StringComparer.OrdinalIgnoreCase);
        Assert.Equal("furion", httpRequestBuilder2.QueryParameters!["name"].First());
        Assert.Equal("furion", httpRequestBuilder2.QueryParameters!["Name"].First());

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder3.WithQueryParameters(new { name = "furion" })
            .WithQueryParameters(new { id = 10, name = "furion2" });
        Assert.Equal("10", httpRequestBuilder3.QueryParameters!["id"].First());
        Assert.Equal(2, httpRequestBuilder3.QueryParameters!["name"].Count);
        Assert.Equal("furion", httpRequestBuilder3.QueryParameters!["name"].First());
        Assert.Equal("furion2", httpRequestBuilder3.QueryParameters!["name"].Last());
    }

    [Fact]
    public void WithPathParameters_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithPathParameters(null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithPathParameters((object)null!);
        });
    }

    [Fact]
    public void WithPathParameters_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.WithPathParameters(new Dictionary<string, object?> { ["id"] = 10, ["name"] = "furion" });
        Assert.NotNull(httpRequestBuilder.PathParameters);
        Assert.Equal(2, httpRequestBuilder.PathParameters.Count);
        Assert.Equal("10", httpRequestBuilder.PathParameters["id"]);
        Assert.Equal("furion", httpRequestBuilder.PathParameters["name"]);

        httpRequestBuilder.PathParameters.Clear();

        httpRequestBuilder.WithPathParameters(new { id = 10, name = "furion" });
        Assert.NotNull(httpRequestBuilder.PathParameters);
        Assert.Equal(2, httpRequestBuilder.PathParameters.Count);
        Assert.Equal("10", httpRequestBuilder.PathParameters["id"]);
        Assert.Equal("furion", httpRequestBuilder.PathParameters["name"]);

        httpRequestBuilder.WithPathParameters(new { age = 30 });
        Assert.NotNull(httpRequestBuilder.PathParameters);
        Assert.Equal(3, httpRequestBuilder.PathParameters.Count);
        Assert.Equal("10", httpRequestBuilder.PathParameters["id"]);
        Assert.Equal("furion", httpRequestBuilder.PathParameters["name"]);
        Assert.Equal("30", httpRequestBuilder.PathParameters["age"]);

        httpRequestBuilder.PathParameters.Clear();

        httpRequestBuilder.WithPathParameters(new { name = "furi on" });
        Assert.Equal("furi on", httpRequestBuilder.PathParameters["name"]);

        httpRequestBuilder.PathParameters.Clear();

        httpRequestBuilder.WithPathParameters(new { name = "furi on" }, true);
        Assert.Equal("furi%20on", httpRequestBuilder.PathParameters["name"]);

        httpRequestBuilder.PathParameters.Clear();

        var dateNow = new DateTime(2024, 08, 30, 23, 59, 59, 999, DateTimeKind.Local);
        httpRequestBuilder.WithPathParameters(new { date = dateNow }, false, CultureInfo.InvariantCulture);
        Assert.Equal("2024-08-30T23:59:59.9990000+08:00", httpRequestBuilder.PathParameters["date"]);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder2.WithPathParameters(new { name = "furion" }, comparer: StringComparer.OrdinalIgnoreCase);
        Assert.Equal("furion", httpRequestBuilder2.PathParameters!["name"]);
        Assert.Equal("furion", httpRequestBuilder2.PathParameters!["Name"]);

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder3.WithPathParameters(new { name = "furion" })
            .WithPathParameters(new { id = 10, name = "furion2" });
        Assert.Equal("10", httpRequestBuilder3.PathParameters!["id"]);
        Assert.Equal(2, httpRequestBuilder3.PathParameters.Count);
        Assert.Equal("furion2", httpRequestBuilder3.PathParameters!["name"]);
    }

    [Fact]
    public void WithObjectPathParameter_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithObjectPathParameter(null!, null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
            httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), null!));
        Assert.Throws<ArgumentException>(() =>
            httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), string.Empty));
        Assert.Throws<ArgumentException>(() =>
            httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), " "));
    }

    [Fact]
    public void WithObjectPathParameter_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), "model");
        httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), "model");
        httpRequestBuilder.WithObjectPathParameter(new ObjectModel(), "Model");

        Assert.NotNull(httpRequestBuilder.ObjectPathParameters);
        Assert.Equal(2, httpRequestBuilder.ObjectPathParameters.Count);
    }

    [Fact]
    public void WithCookies_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithCookies(null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.WithCookies((object)null!);
        });
    }

    [Fact]
    public void WithCookies_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.WithCookies(new Dictionary<string, object?> { ["id"] = 10, ["name"] = "furion" });
        Assert.NotNull(httpRequestBuilder.Cookies);
        Assert.Equal(2, httpRequestBuilder.Cookies.Count);
        Assert.Equal("10", httpRequestBuilder.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder.Cookies["name"]);

        httpRequestBuilder.Cookies.Clear();

        httpRequestBuilder.WithCookies(new { id = 10, name = "furion" });
        Assert.NotNull(httpRequestBuilder.Cookies);
        Assert.Equal(2, httpRequestBuilder.Cookies.Count);
        Assert.Equal("10", httpRequestBuilder.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder.Cookies["name"]);

        httpRequestBuilder.WithCookies(new { age = 30 });
        Assert.NotNull(httpRequestBuilder.Cookies);
        Assert.Equal(3, httpRequestBuilder.Cookies.Count);
        Assert.Equal("10", httpRequestBuilder.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder.Cookies["name"]);
        Assert.Equal("30", httpRequestBuilder.Cookies["age"]);

        httpRequestBuilder.Cookies.Clear();

        httpRequestBuilder.WithCookies(new { name = "furi on" });
        Assert.Equal("furi on", httpRequestBuilder.Cookies["name"]);

        httpRequestBuilder.Cookies.Clear();

        httpRequestBuilder.WithCookies(new { name = "furi on" }, true);
        Assert.Equal("furi%20on", httpRequestBuilder.Cookies["name"]);

        httpRequestBuilder.Cookies.Clear();

        var dateNow = new DateTime(2024, 08, 30, 23, 59, 59, 999, DateTimeKind.Local);
        httpRequestBuilder.WithCookies(new { date = dateNow }, false, CultureInfo.InvariantCulture);
        Assert.Equal("2024-08-30T23:59:59.9990000+08:00", httpRequestBuilder.Cookies["date"]);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder2.WithCookies(new { name = "furion" }, comparer: StringComparer.OrdinalIgnoreCase);
        Assert.Equal("furion", httpRequestBuilder2.Cookies!["name"]);
        Assert.Equal("furion", httpRequestBuilder2.Cookies!["Name"]);

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder3.WithCookies(new { name = "furion" }).WithCookies(new { id = 10, name = "furion2" });
        Assert.Equal("10", httpRequestBuilder3.Cookies!["id"]);
        Assert.Equal(2, httpRequestBuilder3.Cookies.Count);
        Assert.Equal("furion2", httpRequestBuilder3.Cookies!["name"]);
    }

    [Fact]
    public void SetHttpClientFactoryName_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetHttpClientFactoryName(null!);
        });
    }

    [Fact]
    public void SetHttpClientFactoryName_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetHttpClientFactoryName("furion");
        Assert.Equal("furion", httpRequestBuilder.HttpClientFactoryName);
    }

    [Fact]
    public void SetHttpClientProvider_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetHttpClientProvider(null!);
        });
    }

    [Fact]
    public void SetHttpClientProvider_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetHttpClientProvider(() => (new HttpClient(), null));
        Assert.NotNull(httpRequestBuilder.HttpClientProvider);

        httpRequestBuilder.SetHttpClientProvider(() => (new HttpClient(), _ => { }));
        Assert.NotNull(httpRequestBuilder.HttpClientProvider);

        // 运行时将抛异常
        httpRequestBuilder.SetHttpClientProvider(() => (null!, _ => { }));
        Assert.NotNull(httpRequestBuilder.HttpClientProvider);
    }

    [Fact]
    public void AddHttpContentProcessors_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.AddHttpContentProcessors(null!);
        });
    }

    [Fact]
    public void AddHttpContentProcessors_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.AddHttpContentProcessors(() => []);
        Assert.NotNull(httpRequestBuilder.HttpContentProcessorProviders);

        httpRequestBuilder.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(httpRequestBuilder.HttpContentProcessorProviders);

        // 运行时将抛异常
        httpRequestBuilder.AddHttpContentProcessors(() => [null!, new StringContentProcessor()]);
        Assert.NotNull(httpRequestBuilder.HttpContentProcessorProviders);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        httpRequestBuilder2.AddHttpContentProcessors(() => [new StringContentProcessor()]);
        Assert.NotNull(httpRequestBuilder2.HttpContentProcessorProviders);
        Assert.Equal(2, httpRequestBuilder2.HttpContentProcessorProviders.Count);
    }

    [Fact]
    public void AddHttpContentConverters_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.AddHttpContentConverters(null!);
        });
    }

    [Fact]
    public void AddHttpContentConverters_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.AddHttpContentConverters(() => []);
        Assert.NotNull(httpRequestBuilder.HttpContentConverterProviders);

        httpRequestBuilder.AddHttpContentConverters(() =>
            [new StringContentConverter(), new ObjectContentConverter<int>()]);
        Assert.NotNull(httpRequestBuilder.HttpContentConverterProviders);

        // 运行时将抛异常
        httpRequestBuilder.AddHttpContentConverters(() => [null!, new StringContentConverter()]);
        Assert.NotNull(httpRequestBuilder.HttpContentConverterProviders);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        httpRequestBuilder2.AddHttpContentConverters(() => [new StringContentConverter()]);
        Assert.NotNull(httpRequestBuilder2.HttpContentConverterProviders);
        Assert.Equal(2, httpRequestBuilder2.HttpContentConverterProviders.Count);
    }

    [Fact]
    public void SetOnPreSetContent_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetOnPreSetContent(null!);
        });
    }

    [Fact]
    public void SetOnPreSetContent_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetOnPreSetContent(_ => { });
        Assert.NotNull(httpRequestBuilder.OnPreSetContent);
    }

    [Fact]
    public void SetOnPreSetContent_Cascade_ReturnOK()
    {
        var i = 0;
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetOnPreSetContent(_ =>
        {
            i++;
        }).SetOnPreSetContent(_ =>
        {
            i++;
        });
        Assert.NotNull(httpRequestBuilder.OnPreSetContent);

        httpRequestBuilder.OnPreSetContent.Invoke(null);
        Assert.Equal(2, i);
    }

    [Fact]
    public void SetOnPreSendRequest_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetOnPreSendRequest(null!);
        });
    }

    [Fact]
    public void SetOnPreSendRequest_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetOnPreSendRequest(_ => { });
        Assert.NotNull(httpRequestBuilder.OnPreSendRequest);
    }

    [Fact]
    public void SetOnPostSendRequest_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetOnPostSendRequest(null!);
        });
    }

    [Fact]
    public void SetOnPostSendRequest_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetOnPostSendRequest(_ => { });
        Assert.NotNull(httpRequestBuilder.OnPostSendRequest);
    }

    [Fact]
    public void SetOnSendRequestFailed_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() =>
        {
            httpRequestBuilder.SetOnSendRequestFailed(null!);
        });
    }

    [Fact]
    public void SetOnSendRequestFailed_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.SetOnSendRequestFailed((_, _) => { });
        Assert.NotNull(httpRequestBuilder.OnSendRequestFailed);
    }

    [Fact]
    public void EnsureSuccessStatusCode_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        httpRequestBuilder.EnsureSuccessStatusCode();

        Assert.True(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);

        httpRequestBuilder.EnsureSuccessStatusCode(false);
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        httpRequestBuilder.EnsureSuccessStatusCode(true);
        Assert.True(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
    }

    [Fact]
    public void AddBasicAuthentication_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.AddBasicAuthentication(null!, null!));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddBasicAuthentication(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddBasicAuthentication(" ", null!));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.AddBasicAuthentication("furion", null!));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddBasicAuthentication("furion", string.Empty));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddBasicAuthentication("furion", " "));
    }

    [Fact]
    public void AddBasicAuthentication_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.AddBasicAuthentication("furion", "q1w2e3");
        var base64Credentials = Convert.ToBase64String("furion:q1w2e3"u8.ToArray());

        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Basic " + base64Credentials, httpRequestBuilder.AuthenticationHeader.ToString());
        Assert.Equal("Basic", httpRequestBuilder.AuthenticationHeader.Scheme);
    }

    [Fact]
    public void AddJwtBearerAuthentication_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.AddJwtBearerAuthentication(null!));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddJwtBearerAuthentication(string.Empty));
        Assert.Throws<ArgumentException>(() => httpRequestBuilder.AddJwtBearerAuthentication(" "));
    }

    [Fact]
    public void AddJwtBearerAuthentication_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.AddJwtBearerAuthentication("jwtbearer");

        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Bearer jwtbearer", httpRequestBuilder.AuthenticationHeader.ToString());
        Assert.Equal("Bearer", httpRequestBuilder.AuthenticationHeader.Scheme);
    }

    [Fact]
    public void AddAuthentication_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.AddAuthentication(null!));
    }

    [Fact]
    public void AddAuthentication_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.AddAuthentication(new AuthenticationHeaderValue("Bearer", "jwtbearer"));

        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Bearer jwtbearer", httpRequestBuilder.AuthenticationHeader.ToString());
        Assert.Equal("Bearer", httpRequestBuilder.AuthenticationHeader.Scheme);
    }

    [Fact]
    public void DisableCache_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.False(httpRequestBuilder.DisableCacheEnabled);
        httpRequestBuilder.DisableCache();

        Assert.True(httpRequestBuilder.DisableCacheEnabled);

        httpRequestBuilder.DisableCache(false);
        Assert.False(httpRequestBuilder.DisableCacheEnabled);
    }

    [Fact]
    public void UseHttpClientPool_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.False(httpRequestBuilder.HttpClientPoolingEnabled);
        httpRequestBuilder.UseHttpClientPool();

        Assert.True(httpRequestBuilder.HttpClientPoolingEnabled);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.SetEventHandler(null!));
        var exception = Assert.Throws<ArgumentException>(() =>
            httpRequestBuilder.SetEventHandler(typeof(NotImplementRequestEventHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementRequestEventHandler)}` type is not assignable from `{typeof(IHttpRequestEventHandler)}`. (Parameter 'requestEventHandlerType')",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetEventHandler(typeof(CustomRequestEventHandler));

        Assert.Equal(typeof(CustomRequestEventHandler), httpRequestBuilder.RequestEventHandlerType);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.SetEventHandler<CustomRequestEventHandler>();

        Assert.Equal(typeof(CustomRequestEventHandler), httpRequestBuilder2.RequestEventHandlerType);
    }

    [Fact]
    public void SimulateBrowser_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SimulateBrowser();

        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers);
        Assert.Equal(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0",
            httpRequestBuilder.Headers["User-Agent"].First());
    }

    [Fact]
    public void SimulateBrowser_Duplicate_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SimulateBrowser();
        httpRequestBuilder.SimulateBrowser();

        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers["User-Agent"]);
        Assert.Equal(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36 Edg/129.0.0.0",
            httpRequestBuilder.Headers["User-Agent"].First());
    }

    [Fact]
    public void ReleaseResources_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.ReleaseResources();
        Assert.Null(httpRequestBuilder.HttpClientPooling);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;
        httpRequestBuilder.SetHttpClientProvider(() => (new HttpClient(), client =>
        {
            i++;
            client.Dispose();
        }));
        Assert.NotNull(httpRequestBuilder.HttpClientProvider);

        httpRequestBuilder.AddDisposable(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));

        _ = httpRemoteService.CreateHttpClientWithDefaults(httpRequestBuilder);
        httpRequestBuilder.ReleaseResources();
        Assert.Null(httpRequestBuilder.HttpClientPooling);
        Assert.Equal(1, i);

        Assert.NotNull(httpRequestBuilder.Disposables);
        Assert.Empty(httpRequestBuilder.Disposables);

        serviceProvider.Dispose();
    }

    [Fact]
    public void AddDisposable_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.AddDisposable(null!));
    }

    [Fact]
    public void AddDisposable_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.AddDisposable(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));
        Assert.NotNull(httpRequestBuilder.Disposables);
        Assert.Single(httpRequestBuilder.Disposables);
    }

    [Fact]
    public void WithStatusCodeHandler_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.WithStatusCodeHandler(null!, null!));
        var exception =
            Assert.Throws<ArgumentException>(() =>
                httpRequestBuilder.WithStatusCodeHandler(Array.Empty<int>(), null!));

        Assert.Equal(
            "The status codes array cannot be empty. At least one status code must be provided. (Parameter 'statusCodes')",
            exception.Message);

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.WithStatusCodeHandler([200], null!));
    }

    [Fact]
    public void WithStatusCodeHandler_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.WithStatusCodeHandler([200], (_, _) => Task.CompletedTask);
        httpRequestBuilder.WithStatusCodeHandler([200], (_, _) => Task.CompletedTask);
        httpRequestBuilder.WithStatusCodeHandler([200, 204], (_, _) => Task.CompletedTask);
        httpRequestBuilder.WithStatusCodeHandler([200, 204], (_, _) => Task.CompletedTask);
        httpRequestBuilder.WithStatusCodeHandler([200, 209], (_, _) => Task.CompletedTask);

        Assert.NotNull(httpRequestBuilder.StatusCodeHandlers);
        Assert.Equal(5, httpRequestBuilder.StatusCodeHandlers.Count);
    }

    [Fact]
    public void ReleaseDisposables_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        httpRequestBuilder.AddDisposable(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "test.txt")));

        httpRequestBuilder.ReleaseDisposables();
        Assert.NotNull(httpRequestBuilder.Disposables);
        Assert.Empty(httpRequestBuilder.Disposables);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.Profiler();

        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }
}