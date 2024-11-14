﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DeclarativeAttributeTests
{
    [Fact]
    public void BodyAttribute_ReturnOK()
    {
        var attributeUsage = typeof(BodyAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Parameter, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new BodyAttribute();
        Assert.Null(attribute.ContentType);
        Assert.Null(attribute.ContentEncoding);
        Assert.False(attribute.UseStringContent);

        var attribute2 = new BodyAttribute("application/json");
        Assert.Equal("application/json", attribute2.ContentType);
        Assert.Null(attribute2.ContentEncoding);

        var attribute3 = new BodyAttribute("application/json", "utf-32");
        Assert.Equal("application/json", attribute3.ContentType);
        Assert.Equal("utf-32", attribute3.ContentEncoding);
    }

    [Fact]
    public void MultipartAttribute_ReturnOK()
    {
        var attributeUsage = typeof(MultipartAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Parameter, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new MultipartAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.FileName);
        Assert.Null(attribute.ContentType);
        Assert.Null(attribute.ContentEncoding);
        Assert.Equal(FileSourceType.None, attribute.AsFileFrom);
        Assert.True(attribute.AsFormItem);

        var attribute2 = new MultipartAttribute("file");
        Assert.Equal("file", attribute2.Name);
    }

    [Fact]
    public void CookieAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new CookieAttribute(null!));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new CookieAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new CookieAttribute(" ", null));
    }

    [Fact]
    public void CookieAttribute_ReturnOK()
    {
        var attributeUsage = typeof(CookieAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new CookieAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);

        var attribute2 = new CookieAttribute("name");
        Assert.Equal("name", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new CookieAttribute("name", null);
        Assert.Equal("name", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
        Assert.False(attribute3.Escape);
    }

    [Fact]
    public void DisableCacheAttribute_ReturnOK()
    {
        var attributeUsage = typeof(DisableCacheAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new DisableCacheAttribute();
        Assert.True(attribute.Disabled);

        var attribute2 = new DisableCacheAttribute(false);
        Assert.False(attribute2.Disabled);
    }

    [Fact]
    public void EnsureSuccessStatusCodeAttribute_ReturnOK()
    {
        var attributeUsage = typeof(EnsureSuccessStatusCodeAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new EnsureSuccessStatusCodeAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new EnsureSuccessStatusCodeAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void PathAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new PathAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new PathAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new PathAttribute(" ", null));
    }

    [Fact]
    public void PathAttribute_ReturnOK()
    {
        var attributeUsage = typeof(PathAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new PathAttribute("name", "furion");
        Assert.Equal("name", attribute.Name);
        Assert.Equal("furion", attribute.Value);
    }

    [Fact]
    public void HeaderAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HeaderAttribute(null!));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new HeaderAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new HeaderAttribute(" ", null));
    }

    [Fact]
    public void HeaderAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HeaderAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new HeaderAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);
        Assert.False(attribute.Replace);

        var attribute2 = new HeaderAttribute("Set-Cookie");
        Assert.Equal("Set-Cookie", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new HeaderAttribute("Set-Cookie", null);
        Assert.Equal("Set-Cookie", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
        Assert.False(attribute3.Escape);
    }

    [Fact]
    public void HttpClientNameAttribute_ReturnOK()
    {
        var attributeUsage = typeof(HttpClientNameAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new HttpClientNameAttribute(null);
        Assert.Equal(string.Empty, attribute.Name);

        var attribute2 = new HttpClientNameAttribute("client-name");
        Assert.Equal("client-name", attribute2.Name);
    }

    [Fact]
    public void ProfilerAttribute_ReturnOK()
    {
        var attributeUsage = typeof(ProfilerAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new ProfilerAttribute();
        Assert.True(attribute.Enabled);

        var attribute2 = new ProfilerAttribute(false);
        Assert.False(attribute2.Enabled);
    }

    [Fact]
    public void QueryAttribute_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryAttribute(null!));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(string.Empty));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(" "));

        Assert.Throws<ArgumentNullException>(() => new QueryAttribute(null!, null));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(string.Empty, null));
        Assert.Throws<ArgumentException>(() => new QueryAttribute(" ", null));
    }

    [Fact]
    public void QueryAttribute_ReturnOK()
    {
        var attributeUsage = typeof(QueryAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
            attributeUsage.ValidOn);
        Assert.True(attributeUsage.AllowMultiple);

        var attribute = new QueryAttribute();
        Assert.Null(attribute.Name);
        Assert.Null(attribute.Value);
        Assert.Null(attribute.AliasAs);
        Assert.False(attribute.HasSetValue);
        Assert.Null(attribute.Prefix);
        Assert.False(attribute.Replace);

        var attribute2 = new QueryAttribute("name");
        Assert.Equal("name", attribute2.Name);
        Assert.Null(attribute2.Value);
        Assert.Null(attribute2.AliasAs);
        Assert.False(attribute2.HasSetValue);

        var attribute3 = new QueryAttribute("name", null);
        Assert.Equal("name", attribute3.Name);
        Assert.Null(attribute3.Value);
        Assert.Null(attribute3.AliasAs);
        Assert.True(attribute3.HasSetValue);
        Assert.False(attribute3.Escape);
    }

    [Fact]
    public void SimulateBrowserAttribute_ReturnOK()
    {
        var attributeUsage = typeof(SimulateBrowserAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new SimulateBrowserAttribute();
        Assert.False(attribute.Mobile);

        var attribute2 = new SimulateBrowserAttribute { Mobile = true };
        Assert.True(attribute2.Mobile);
    }

    [Fact]
    public void TimeoutAttribute_ReturnOK()
    {
        var attributeUsage = typeof(TimeoutAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new TimeoutAttribute(200);
        Assert.Equal(200, attribute.Timeout);
    }

    [Fact]
    public void TraceIdentifierAttribute_ReturnOK()
    {
        var attributeUsage = typeof(TraceIdentifierAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new TraceIdentifierAttribute("furion");
        Assert.Equal("furion", attribute.Identifier);
    }

    [Fact]
    public void AcceptLanguageAttribute_ReturnOK()
    {
        var attributeUsage = typeof(AcceptLanguageAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsage);
        Assert.Equal(AttributeTargets.Method | AttributeTargets.Interface, attributeUsage.ValidOn);
        Assert.False(attributeUsage.AllowMultiple);

        var attribute = new AcceptLanguageAttribute("fr-FR");
        Assert.Equal("fr-FR", attribute.Language);

        var attribute2 = new AcceptLanguageAttribute(null);
        Assert.Null(attribute2.Language);
    }
}