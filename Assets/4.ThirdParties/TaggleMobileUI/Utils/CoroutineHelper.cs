using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    public static List<GameObject> Call(params IEnumerator[] coroutines)
    {
        List<GameObject> goCoroutines = new List<GameObject>();

        if (coroutines == null || coroutines.Length == 0)
        {
            return goCoroutines;
        }

        for (int i = 0; i < coroutines.Length; i++)
        {
            GameObject go = new GameObject("coroutine");
            CoroutineHelper view = go.AddComponent<CoroutineHelper>();
            view.Do(coroutines[i]);

            goCoroutines.Add(go);
        }

        return goCoroutines;
    }

    private void Do(IEnumerator coroutine)
    {
        StartCoroutine(Wait(coroutine));
    }

    private IEnumerator Wait(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
        Destroy(gameObject);
    }
}