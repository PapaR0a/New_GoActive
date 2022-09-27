using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour {

    public static void Call(IEnumerator coroutine)
    {
        GameObject go = new GameObject("Coroutine");
        CoroutineHelper view = go.AddComponent<CoroutineHelper>();
        view.Do(coroutine);
    }

    private void Do(IEnumerator coroutine)
    {
        StartCoroutine(Wait(coroutine));
    }

    IEnumerator Wait(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
        Destroy(gameObject);
    }
}
