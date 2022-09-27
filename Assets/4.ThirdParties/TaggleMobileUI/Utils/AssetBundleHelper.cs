
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleHelper
{
    public static AssetBundle LoadFromFile(string path)
    {
        if (s_bundles.ContainsKey(path))
            return s_bundles[path];
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        s_bundles.Add(path, bundle);
        return bundle;
    }

    public static void Unload(string path)
    {
        if(!s_bundles.ContainsKey(path))
            return;
        AssetBundle bundle = s_bundles[path];
        s_bundles.Remove(path);
        bundle.Unload(true);
    }

    private static Dictionary<string, AssetBundle> s_bundles = new Dictionary<string, AssetBundle>();
}