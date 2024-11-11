﻿// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Core.Extensions;

/// <summary>
///     <see cref="IDictionary{TKey, TValue}" /> 拓展类
/// </summary>
internal static class IDictionaryExtensions
{
    /// <summary>
    ///     添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="concatDictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="allowDuplicates">是否允许重复添加。默认值为：<c>true</c>。</param>
    /// <param name="replace">是否值已存在时则采用替换的方式，否则采用追加方式。默认值为 <c>false</c>。</param>
    internal static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary,
        IDictionary<TKey, TValue> concatDictionary, bool allowDuplicates = true, bool replace = false)
        where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(concatDictionary);

        // 逐条遍历合并更新
        foreach (var (key, newValue) in concatDictionary)
        {
            // 检查键是否存在
            if (!dictionary.TryGetValue(key, out var values))
            {
                values = [];
                dictionary.Add(key, values);
            }

            // 检查是否启用重复值检查
            if (!allowDuplicates && values.Contains(newValue))
            {
                continue;
            }

            // 检查是否采用替换的方式
            if (replace)
            {
                values.Clear();
            }

            if (newValue is null || !newValue.GetType().IsArrayOrCollection(out _))
            {
                values.Add(newValue);
            }
            else
            {
                values.AddRange(((IEnumerable)newValue).Cast<TValue>());
            }
        }
    }

    /// <summary>
    ///     添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="concatDictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    internal static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IDictionary<TKey, TValue> concatDictionary)
        where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(concatDictionary);

        // 逐条遍历合并更新
        foreach (var (key, value) in concatDictionary)
        {
            // 检查键是否存在
            dictionary[key] = value;
        }
    }

    /// <summary>
    ///     尝试添加
    /// </summary>
    /// <remarks>其中键是由值通过给定的选择器函数生成的。</remarks>
    /// <param name="dictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="values">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <param name="keySelector">键选择器</param>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    internal static void TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TValue>? values, Func<TValue, TKey> keySelector)
        where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keySelector);

        // 空检查
        if (values is null)
        {
            return;
        }

        // 逐条遍历尝试添加
        foreach (var value in values)
        {
            dictionary.TryAdd(keySelector(value), value);
        }
    }

    /// <summary>
    ///     在指定的位置尝试添加
    /// </summary>
    /// <remarks>其中键是由值通过给定的选择器函数生成的。</remarks>
    /// <param name="dictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="values">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <param name="index">指定位置索引</param>
    /// <param name="keySelector">键选择器</param>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    internal static void TryAddAt<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TValue>? values, Func<TValue, TKey> keySelector, int index)
        where TKey : notnull
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(keySelector);

        // 空检查
        if (values is null)
        {
            return;
        }

        // 复制现有的键值对到列表中
        var keyValuePairs = new List<KeyValuePair<TKey, TValue>>(dictionary);

        // 收集需要插入的键值对
        var toInsert = (from value in values
            let key = keySelector(value)
            where !dictionary.TryGetValue(key, out _)
            select new KeyValuePair<TKey, TValue>(key, value)).ToArray();

        // 空检查
        if (toInsert is { Length: 0 })
        {
            return;
        }

        // 在指定位置插入新的键值对
        keyValuePairs.InsertRange(index, toInsert);

        // 重新构建字典
        dictionary.Clear();
        foreach (var kvp in keyValuePairs)
        {
            dictionary.TryAdd(kvp.Key, kvp.Value);
        }
    }
}