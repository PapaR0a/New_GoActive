using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageDownloader : MonoBehaviour {
    public Image _img;
	public string url = "";
    public Sprite sprDefault;
    void Start () {
        if(_img == null)
            _img = GetComponent<Image>();

        _img.sprite = sprDefault;
    }

    public void SetUrl(string _url)
    {
        url = _url;
        if(this.isActiveAndEnabled && url != "")
        {
            StartCoroutine(LoadFromWeb(url));
        }
    }
 
    void OnEnable() {
        if(_img.sprite == sprDefault && url != ""){
            StartCoroutine(LoadFromWeb(url));
        }
    }

    IEnumerator LoadFromWeb(string _url)
    {
        UnityWebRequest wr = new UnityWebRequest(_url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        if(!(wr.isNetworkError || wr.isHttpError)) {
            Texture2D t = texDl.texture;
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),
                                     Vector2.zero, 1f);
            _img.sprite = s;
        }
    }
}
