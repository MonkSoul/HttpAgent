﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ConstantsTests
{
    [Fact]
    public void X_TRACE_ID_HEADER() =>
        Assert.Equal("X-Trace-ID", Constants.X_TRACE_ID_HEADER);

    [Fact]
    public void UNKNOWN_USER_AGENT_VERSION() => Assert.Equal("unknown", Constants.UNKNOWN_USER_AGENT_VERSION);

    [Fact]
    public void FORM_DATA_DISPOSITION_TYPE() => Assert.Equal("form-data", Constants.FORM_DATA_DISPOSITION_TYPE);

    [Fact]
    public void BASIC_AUTHENTICATION_SCHEME() => Assert.Equal("Basic", Constants.BASIC_AUTHENTICATION_SCHEME);

    [Fact]
    public void JWT_BEARER_AUTHENTICATION_SCHEME() =>
        Assert.Equal("Bearer", Constants.JWT_BEARER_AUTHENTICATION_SCHEME);

    [Fact]
    public void DIGEST_AUTHENTICATION_SCHEME() =>
        Assert.Equal("Digest", Constants.DIGEST_AUTHENTICATION_SCHEME);

    [Fact]
    public void TEXT_PLAIN_MIME_TYPE() =>
        Assert.Equal("text/plain", Constants.TEXT_PLAIN_MIME_TYPE);

    [Fact]
    public void X_END_OF_STREAM_HEADER() =>
        Assert.Equal("X-End-Of-Stream", Constants.X_END_OF_STREAM_HEADER);

    [Fact]
    public void X_ORIGINAL_URL_HEADER() =>
        Assert.Equal("X-Original-URL", Constants.X_ORIGINAL_URL_HEADER);

    [Fact]
    public void X_FORWARD_TO_HEADER() =>
        Assert.Equal("X-Forward-To", Constants.X_FORWARD_TO_HEADER);

    [Fact]
    public void X_STRESS_TEST_HEADER() =>
        Assert.Equal("X-Stress-Test", Constants.X_STRESS_TEST_HEADER);

    [Fact]
    public void X_STRESS_TEST_VALUE() =>
        Assert.Equal("Harness", Constants.X_STRESS_TEST_VALUE);

    [Fact]
    public void DISABLED_PROFILER_KEY() =>
        Assert.Equal("__Disabled_Profiler__", Constants.DISABLED_PROFILER_KEY);

    [Fact]
    public void DECLARATIVE_METHOD_KEY() =>
        Assert.Equal("__DECLARATIVE_METHOD__", Constants.DECLARATIVE_METHOD_KEY);

    [Fact]
    public void USER_AGENT_OF_BROWSER() =>
        Assert.Equal(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36 Edg/130.0.0.0",
            Constants.USER_AGENT_OF_BROWSER);

    [Fact]
    public void USER_AGENT_OF_MOBILE_BROWSER() =>
        Assert.Equal(
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Mobile Safari/537.36 Edg/130.0.0.0",
            Constants.USER_AGENT_OF_MOBILE_BROWSER);
}