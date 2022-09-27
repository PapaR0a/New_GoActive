using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ResourceObject : MonoBehaviour
{
    public static ResourceObject instance;
    public List<Object> objects;

    void Awake()
    {
        instance = this;
    }

    public static T GetResource<T>(string name) where T: Object
    {
        string realName = Path.GetFileNameWithoutExtension(name);
        foreach (Object prefab in instance.objects)
        {
            if (prefab.name.Equals(realName))
                return prefab as T;
        }
        
        return default(T);
    }

    public static void LoadTexture(RawImage rawImage, string url)
    {
        Uri uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (!result)
        {
            rawImage.texture = ResourceObject.GetResource<Texture>(Path.GetFileNameWithoutExtension(url));
            return;
        }
        rawImage.LoadTexture(url);
    }
}
