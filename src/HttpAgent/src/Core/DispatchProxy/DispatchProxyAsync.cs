// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable

using static System.Reflection.AsyncDispatchProxyGenerator;

namespace System.Reflection;

public abstract class DispatchProxyAsync
{
    public static T Create<T, TProxy>() where TProxy : DispatchProxyAsync =>
        (T)CreateProxyInstance(typeof(TProxy), typeof(T));

    public static object Create(Type type, Type proxyType) =>
        CreateProxyInstance(proxyType, type);

    public abstract object Invoke(MethodInfo method, object[] args);

    public abstract Task InvokeAsync(MethodInfo method, object[] args);

    public abstract Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args);
}