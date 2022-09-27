using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ButtonExtension{
	public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick){
		button.onClick.AddListener(delegate{
			OnClick(param);
		});
	}
}

public class AssetBundleDLC : MonoBehaviour {
	public string[] urls = {""};
	[Header("UI Stuff")]
	public Transform rootContainer;
	public Button pref;
	public Text labelText;
	static List<AssetBundle> assetBundle = new List<AssetBundle>();
	static List<string> sceneNames = new List<string>();

	// Use this for initialization
	IEnumerator Start () {
		urls[0] = "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles/bedwetting/scenes");
		urls[1] =  "file://" + System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles/bedwetting/assets");
		if(assetBundle.Count == 0){
			int i=0;
			while(i<urls.Length){
				using (WWW www = new WWW(urls[i])){
					yield return www;
					if(!string.IsNullOrEmpty(www.error)){
						Debug.Log(www.error);
						yield break;
					}
					assetBundle.Add(www.assetBundle);
					sceneNames.AddRange(www.assetBundle.GetAllScenePaths());
				}
				i++;
			}
		}
		
		foreach(string sceneName in sceneNames){
			labelText.text = Path.GetFileNameWithoutExtension(sceneName);
			var clone = Instantiate(pref.gameObject) as GameObject;
			clone.GetComponent<Button>().AddEventListener(labelText.text, LoadAssetBundleScene);
			clone.SetActive(true);
			clone.transform.SetParent(rootContainer);
		}
	}

	public void LoadAssetBundleScene(string sceneName){
		SceneManager.LoadScene(sceneName);
	}

}
