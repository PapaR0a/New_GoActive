using System.Collections;
using System.Collections.Generic;
using Taggle.HealthApp.Others;
using TaggleTemplate.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Taggle.HealthApp.Components{
	public class DownloadManagerComp : MonoBehaviour {
		public static DownloadManagerComp Instance;

		void Start(){
			Instance = this;
		}
		

		public IEnumerator GetAssetBundle(string url, UnityAction<AssetBundle> callback) {
			UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
			yield return www.Send();

			if(www.isNetworkError) {
				Debug.Log(www.error);
			}
			else {
				AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
				callback.Invoke(bundle);
			}
   		}
	}
}