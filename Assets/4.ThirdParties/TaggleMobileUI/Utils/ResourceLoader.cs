using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ResourceLoader
{
    private static ResourceLoader m_api;

    public static ResourceLoader api
    {
        get
        {
            if (m_api == null)
                m_api = new ResourceLoader();
            return m_api;
        }
    }

    private ResourceLoader()
    {
        m_cache = new Dictionary<string, object>();
    }

    private Dictionary<string, object> m_cache;//cache resource loaded

    //check cached 
    public bool Cached(string link)
    {
        return m_cache.ContainsKey(link);
    }

    //get texure from cache
    public Texture GetTexture(string link)
    {
        if (!m_cache.ContainsKey(link))
            return null;
        object texture = m_cache[link];
        if (texture == null)
            return null;
        return (Texture)texture;
    }

    //save resource
    public void Cache(string link, object cache, bool replace = false)
    {
        if (replace)//remove if replace = true;
            m_cache.Remove(link);
        if (m_cache.ContainsKey(link))//return if contains
            return;
        m_cache.Add(link, cache);
    }
}

public static class ResourceUtils
{
    //call as extension
    public static void LoadTexture(this RawImage rawImage, string link, bool cache = true)
    {
        if (string.IsNullOrEmpty(link))
            return;
        Uri uriResult;
        bool result = Uri.TryCreate(link, UriKind.Absolute, out uriResult)
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (!result)
        {
            rawImage.texture = Resources.Load<Texture>(link);
            return;
        }
        ResourceHelper.LoadTexture(rawImage, link, cache, false);
    }
}

//load texture and set to component
public class ResourceHelper : MonoBehaviour
{
    private const int MAX_RETRY = 4;

    public static void LoadTexture(RawImage rawImage, string link, bool cache, bool replace)
    {
        if(string.IsNullOrEmpty(link))
            return;
            
        //load from cache if already
        if (!replace && ResourceLoader.api.Cached(link) && cache)
        {
            rawImage.texture = ResourceLoader.api.GetTexture(link);
            return;
        }
        //load from WWW
        GameObject go = new GameObject {name = "load_texture"};
        DontDestroyOnLoad(go);
        ResourceHelper view = go.AddComponent<ResourceHelper>();
        view.Load(rawImage, link, cache, replace);
    }

    private void Load(RawImage rawImage, string link, bool cache, bool replace)
    {
        StartCoroutine(Wait(rawImage, link, cache, replace));
    }

    IEnumerator Wait(RawImage rawImage, string link, bool cache, bool replace)
    {
        //if(rawImage != null)
        //    rawImage.gameObject.SetActive(false);
        int retry = 0;
        bool isRetry = false;
        WWW www;
        do
        {
            www = new WWW(link);
            yield return www;
            retry++;
            isRetry = !string.IsNullOrEmpty(www.error) && retry < MAX_RETRY;
            if (isRetry)
            {
                Debug.Log("Download error:" + www.error);
                Debug.Log("Retry download " + retry + ":" + link);
                yield return new WaitForSeconds(0.1f);
            }
        } while (isRetry);
        
        if (rawImage != null && retry < MAX_RETRY)
        {
            rawImage.texture = www.texture;
            //rawImage.SetNativeSize();
            //rawImage.gameObject.SetActive(true);
            if (cache)//save to cache
                ResourceLoader.api.Cache(link, www.texture, replace);
        }
        else
        {
            if (cache)//save to cache
                ResourceLoader.api.Cache(link, null, replace);
        }
        
        Destroy(gameObject);//destroy game object when complete
    }

}

public class DownloadHelper : MonoBehaviour
{
    public static void DownloadFile(string url, string savePath, Action<string, float> onUpdate, Action<string> onComplete)
    {
        GameObject go = new GameObject { name = "download" };
        DontDestroyOnLoad(go);
        DownloadHelperComponent view = go.AddComponent<DownloadHelperComponent>();
        view.Load(url, savePath, onUpdate, onComplete);
    }

    public static void DownloadFile(string url, string savePath, Action<string> onComplete)
    {
        if (string.IsNullOrEmpty(savePath) || string.IsNullOrEmpty(url) || !IsUrl(url))
        {
            return;
        }
        GameObject go = new GameObject { name = "download" };
        DontDestroyOnLoad(go);
        DownloadHelperComponent view = go.AddComponent<DownloadHelperComponent>();
        view.Load(url, savePath, null, onComplete);
    }

    public static void DownloadFile(List<string> urls, List<string> paths, int maxThread, Action<string> onComplete, Action onFinish, Action<float> onUpdate = null)
    {
        s_onUpdate = onUpdate;
        s_pathProcess.Clear();
        s_totalPath = urls.Count;
        //
        s_urls.AddRange(urls);
        s_paths.AddRange(paths);
        if (s_downloading.Count > 0)
            return;
        s_onComplete += onFinish;
        int count = 0;
        for (int i = 0; i < s_urls.Count; i++)
        {
            DownloadFile(s_urls[i], s_paths[i], OnUpdateForPath, (data) => OnComplete(data, onComplete));
            s_downloading.Add(s_urls[i]);
            s_urls.RemoveAt(0);
            s_paths.RemoveAt(0);
            count++;
            i--;
            if (count >= maxThread)
                break;
        }
    }
    
    private static void OnComplete(string url, Action<string> onComplete)
    {
        onComplete?.Invoke(url);
        s_downloading.Remove(url);
        if (s_downloading.Count < 1)
        {
            s_onComplete?.Invoke();
            s_onComplete = null;
            return;
        }
        if(s_urls.Count < 1)
            return;
        string nextUrl = s_urls[0];
        string nextPath = s_paths[0];
        s_downloading.Add(nextUrl);
        s_urls.RemoveAt(0);
        s_paths.RemoveAt(0);
        DownloadFile(nextUrl, nextPath, OnUpdateForPath, (data) => OnComplete(data, onComplete));
    }

    private static void OnUpdateForPath(string url, float process)
    {
        if (s_pathProcess.ContainsKey(url))
            s_pathProcess[url] = process;
        else
            s_pathProcess.Add(url, process);
        float part = 1f / s_totalPath;
        float realProcess = 0f;
        foreach (KeyValuePair<string, float> child in s_pathProcess)
        {
            realProcess += child.Value * part;
        }
        s_onUpdate?.Invoke(realProcess);
    }

    private static List<string> s_urls = new List<string>();
    private static List<string> s_paths = new List<string>();
    private static List<string> s_downloading = new List<string>();
    private static Action s_onComplete;
    private static Action<float> s_onUpdate;//update process for path
    private static Dictionary<string, float> s_pathProcess = new Dictionary<string, float>();//path download done
    private static int s_totalPath;//total path

    private static bool IsUrl(string link)
    {
        Uri uriResult;
        return Uri.TryCreate(link, UriKind.Absolute, out uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

public class DownloadHelperComponent :MonoBehaviour
{
    private const int MAX_RETRY = 3;

    private string m_url;
    private UnityWebRequest m_www;
    private Action<string, float> m_onUpdate;

    public void Load(string url, string savePath, Action<string, float> onUpdate, Action<string> onComplete)
    {
        m_onUpdate = onUpdate;
        m_url = url;
        StartCoroutine(Download(url, savePath, onComplete));
    }

    private IEnumerator Download(string url, string savePath, Action<string> onComplete)
    {
        Debug.Log("Start download file:" + url);
        int retry = 0;
        bool isRetry = false;
        do
        {
            m_www = UnityWebRequest.Get(url);
            yield return m_www.SendWebRequest();
            retry++;
            isRetry = !string.IsNullOrEmpty(m_www.error) && retry < MAX_RETRY;
            if (isRetry)
            {
                Debug.Log("Error :" + m_www.error);
                Debug.Log("Retry download :" + url);
            }
        } while (isRetry);

        if (retry < MAX_RETRY)
        {
            //save
            byte[] results = m_www.downloadHandler.data;
            if (File.Exists(savePath))
                File.Delete(savePath);
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            File.WriteAllBytes(savePath, results);
        }
        yield return null;
        onComplete?.Invoke(url);
        Debug.Log("download done: " + url);
        Destroy(gameObject);
    }

    void Update()
    {
        if (m_www == null)
            return;
        if (!m_www.isDone)
            m_onUpdate?.Invoke(m_url, m_www.downloadProgress);
    }
}