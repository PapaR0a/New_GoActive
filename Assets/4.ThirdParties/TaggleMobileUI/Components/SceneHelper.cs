using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper : MonoBehaviour
{

    private static SceneHelper m_api;

    private static SceneHelper api
    {
        get
        {
            if (m_api == null)
            {
                GameObject go = new GameObject();
                DontDestroyOnLoad(go);
                go.name = "LoadSceneHelper";
                m_api = go.AddComponent<SceneHelper>();
                m_api.Init();
            }

            return m_api;
        }
    }

    private Queue<string> m_queueLoad;//queue scene need load
    private Queue<string> m_queueUnload;//queue scene need unload
    private bool m_load;
    private bool m_unload;

    //cache last state
    private List<string> m_lastState;

    //load scene additive
    public static void LoadSceneAdditiveAsync(params string[] scene)
    {
        foreach (string s in scene)
        {
            if (!SceneManager.GetSceneByName(s).isLoaded)
                api.m_queueLoad.Enqueue(s);
            else
                Debug.Log("Scene loaded:" + s);
        }

        if (!api.m_load && api.m_queueLoad.Count > 0)//start coroutine if not start
            api.StartCoroutine(api.Load());
    }

    //unload scene
    public static void UnloadSceneAdditive(params string[] scene)
    {
        UnloadSceneAdditive(false, scene);
    }

    //unload scene
    public static void UnloadSceneAdditive(bool cache, params string[] scene)
    {
        foreach (string s in scene)
        {
            if (SceneManager.GetSceneByName(s).isLoaded)
            {
                api.m_queueUnload.Enqueue(s);
                if(cache)
                    api.m_lastState.Add(s);
            }
            else
                Debug.Log("Secene not loaded:" + s);
        }

        if (!api.m_unload && m_api.m_queueUnload.Count > 0)//start coroutine if not start
            api.StartCoroutine(api.Unload());
    }

    public static void BackState()
    {
        LoadSceneAdditiveAsync(m_api.m_lastState.ToArray());
        api.m_lastState.Clear();
    }

    void Init()
    {
        m_queueLoad = new Queue<string>();
        m_queueUnload = new Queue<string>();
        m_load = false;
        m_unload = false;
        m_lastState = new List<string>();
    }

    //coroutine load scene
    IEnumerator Load()
    {
        m_load = true;
        while (m_load)
        {
            string scene = m_queueLoad.Dequeue();
            AsyncOperation sync = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return sync;
            if (m_queueLoad.Count < 1)//stop if queue empty
                m_load = false;
        }
    }

    //coroutine unload
    IEnumerator Unload()
    {
        m_unload = true;
        while (m_unload)
        {
            string scene = m_queueUnload.Dequeue();
            yield return SceneManager.UnloadSceneAsync(scene);
            if (m_queueUnload.Count < 1)//stop if queue empty
                m_unload = false;
        }
    }


}
