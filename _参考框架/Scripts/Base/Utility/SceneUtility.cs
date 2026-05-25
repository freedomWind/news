/*
*FileName:	SceneUtility
*Author:	油菜花
*CreateTime:2022/9/2 11:31:19
*Description:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class SceneUtility
{
    public static string GetSceneObjName(GameObject obj)
    {
        if (obj == null || obj.scene == default)
            throw new Exception("GetObjFullName must be exist in scene");
        if (obj.transform.parent == null)
            return obj.name;
        Stack<Transform> ns = new Stack<Transform>();
        var p = obj.transform;
        do
        {
            ns.Push(p);
            p = p.parent;
        } while (p != null);

        StringBuilder sb = new StringBuilder();
        while (ns.Count != 1)
        {
            sb.Append(ns.Pop().name);
            sb.Append('/');
        }

        sb.Append(ns.Pop().name);
        return sb.ToString();
    }
}
