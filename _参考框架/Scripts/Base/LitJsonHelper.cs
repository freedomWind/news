/*
*FileName:	LitJsonHelper
*Author:	油菜花
*CreateTime:2022/6/20 12:56:51
*Description:
*/
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using System;

public class LitJsonHelper : Utility.Json.IJsonHelper
{
    /// <summary>
    /// 将对象序列化为 JSON 字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象。</param>
    /// <returns>序列化后的 JSON 字符串。</returns>
    public string ToJson(object obj)
    {
        return LitJson.JsonMapper.ToJson(obj);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为对象。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="json">要反序列化的 JSON 字符串。</param>
    /// <returns>反序列化后的对象。</returns>
    public T ToObject<T>(string json)
    {
        return LitJson.JsonMapper.ToObject<T>(json);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为对象。
    /// </summary>
    /// <param name="objectType">对象类型。</param>
    /// <param name="json">要反序列化的 JSON 字符串。</param>
    /// <returns>反序列化后的对象。</returns>
    public object ToObject(Type objectType, string json)
    {
        return LitJson.JsonMapper.ToObject(json, objectType);
    }
    
    // public static string LToJson(object obj)
    // {
    //     return LitJson.JsonMapper.ToJson(obj);
    // }
    //
    // /// <summary>
    // /// 将 JSON 字符串反序列化为对象。
    // /// </summary>
    // /// <typeparam name="T">对象类型。</typeparam>
    // /// <param name="json">要反序列化的 JSON 字符串。</param>
    // /// <returns>反序列化后的对象。</returns>
    // public static T LToObject<T>(string json)
    // {
    //     return LitJson.JsonMapper.ToObject<T>(json);
    // }
    //
    // /// <summary>
    // /// 将 JSON 字符串反序列化为对象。
    // /// </summary>
    // /// <param name="objectType">对象类型。</param>
    // /// <param name="json">要反序列化的 JSON 字符串。</param>
    // /// <returns>反序列化后的对象。</returns>
    // public static object LToObject(Type objectType, string json)
    // {
    //     return LitJson.JsonMapper.ToObject(json, objectType);
    // }
}
