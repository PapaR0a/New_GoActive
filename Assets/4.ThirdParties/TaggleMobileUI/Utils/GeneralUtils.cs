using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class GeneralUtils {

    public static T Get<T>(this Dictionary<string, object> h, string key, bool useDefaultIfNotExist = true)
    {
        return Get<T, object>(h, key, useDefaultIfNotExist);
    }

    public static T Get<T, T1>(this Dictionary<string, T1> h, string key, bool useDefaultIfNotExist = true)
    {
        if (h == null)
        {
            return default(T);
        }

        if (!h.ContainsKey(key))
        {
            return default(T);
        }

        object value = h[key];
        if (value == null) return default(T);

        Type valueT = value.GetType();

        if (valueT == typeof(T)) return (T)value;
        if (valueT.CanCast(typeof(T)))
        {
            try
            {
#if UNITY_WP8
                return (T)Convert.ChangeType(value, typeof(T));
#else
                return (T)Convert.ChangeType(value, Type.GetTypeCode(typeof(T)));
#endif
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        return default(T);
    }

    public static bool ContainsKey(this JObject jobject, string key)
    {
        return jobject[key] != null;
    }

    private static bool CanCast(this Type p_from, Type p_to)
    {
        if (p_to.IsAssignableFrom(p_from)) { return true; }
        return (p_from.IsPrimitive || p_from.IsEnum) && (p_to.IsPrimitive || p_to.IsEnum);
    }
    
    public static Rect GetScreenRect(GameObject gameObject)
    {
        RectTransform transform = gameObject.GetComponent<RectTransform>();

        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= ((1.0f - transform.pivot.y) * size.y);

        return rect;
    }

    public static string DeepTrace(this object p_obj, int p_level = 0)
    {
        string baseStr = Duplicate("\t", p_level);

        if (p_obj == null) { return "null"; }

        if (p_obj.GetType().IsPrimitive) { return p_obj.ToString(); }

        var list1 = p_obj as IList;
        if (list1 != null)
        {
            IList list = list1;
            string str = baseStr + "[";
            for (int i = 0; i < list.Count; i++)
            {
                str += (i > 0 ? ",\n" : "\n") + baseStr + "\t" + DeepTrace(list[i], p_level + 1);
            }
            return str + "]";
        }

        var dictionary = p_obj as IDictionary;
        if (dictionary != null)
        {
            IDictionary dict = dictionary;
            string str = baseStr + "{";
            bool first = true;

            foreach (DictionaryEntry item in dict)
            {
                str += (first ? "\n" : ",\n") + baseStr + "\t"
                       + (item.Key + ":" + DeepTrace(item.Value, p_level + 1));
                first = false;
            }

            return str + "}";
        }

        return baseStr + p_obj;
    }

    public static string Duplicate(string p_str, int p_nTimes)
    {
        string result = "";
        for (int i = 0; i < p_nTimes; i++) { result += p_str; }
        return result;
    }
}
