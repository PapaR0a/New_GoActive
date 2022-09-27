using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Taggle.HealthApp.Others;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Taggle.HealthApp.Components
{
    public class CameraFoodComp : MonoBehaviour
    {
        public GameObject[] btnBottom;
        public RawImage rawimage;
        WebCamTexture webCamTexture;

        void Start() 
        {
            ChangeBottom(true);
        }
        
        public void StartCam()
        {
            webCamTexture = new WebCamTexture();
            for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
            {
                // We want the back camera
                if (!WebCamTexture.devices[cameraIndex].isFrontFacing)
                {
                    webCamTexture = new WebCamTexture(cameraIndex, (int)(Screen.width *0.8f), (int)(Screen.height * 0.8f));

                    // Here we flip the GuiTexture by applying a localScale transformation
                    // works only in Landscape mode
                    //rawimage.transform.localScale = new Vector3(-1, 1, 1);
                    //rawimage.transform.Rotate(Vector3.forward, -90f, Space.Self);
                }
            }
            float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
            rawimage.transform.localScale = new Vector3(1, scaleY, 1f);
            RectTransform rectRaw =  rawimage.GetComponent<RectTransform>();
            rectRaw.localEulerAngles = new Vector3(0, 0, -90f);
            rectRaw.sizeDelta = new Vector2(rectRaw.sizeDelta.y,rectRaw.sizeDelta.x);
            rawimage.texture = null;
            webCamTexture.Play();
            rawimage.material.mainTexture = webCamTexture;
        }
    
        public void StopCamera()
        {
            webCamTexture.Stop();
        }

        public void ChangeBottom(bool b)
        {
            btnBottom[0].SetActive(b);
            btnBottom[1].SetActive(!b);
        }

        public void OnClickTake()
        {
            StartCoroutine(TakePhoto());
        }

        public void OnClickBack()
        {
            OnCloseFoodInfo();
        }

        public void OnClickExit()
        {
            OnCloseFoodInfo();
            rawimage.texture = null;
            StopCamera();
        }

        IEnumerator TakePhoto()
        {
            yield return new WaitForEndOfFrame();
            // it's a rare case where the Unity doco is pretty clear,
            // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
            // be sure to scroll down to the SECOND long example on that doco page 
            Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
            photo.SetPixels(webCamTexture.GetPixels());
            photo.Apply();

            //Encode to a PNG
            byte[] bytes = photo.EncodeToPNG();
            //Write out the PNG. Of course you have to substitute your_path for something sensible
            if (!Directory.Exists( Application.persistentDataPath + "/CamFoods"))
                Directory.CreateDirectory( Application.persistentDataPath + "/CamFoods");
            File.WriteAllBytes( Application.persistentDataPath + "/CamFoods/food.png", bytes);

            rawimage.texture = photo;
            StopCamera();
            ProcessPhotoFood();
        }

        public PanelCommon pnlFoodInfo;
        public GameObject[] pnlInfoContent;
        public Text txtFoodInfo;
        public void ProcessPhotoFood(){
            ChangeInfoContent(true);
            OnOpenFoodInfo();
            StartCoroutine(WaitDemo());
        }

        public void onInitComplete(){
            ChangeBottom(false);
            txtFoodInfo.text = string.Format("<b>{0}</b>\nCarbs {1}g\nProtein {2}g\nFiber {3}g\nFats {4}g","Char Kway Teow",30,5,16,16);
            ChangeInfoContent(false);
        }

        public void ChangeInfoContent(bool b){
            pnlInfoContent[0].SetActive(b);
            pnlInfoContent[1].SetActive(!b);
        }

        public void OnOpenFoodInfo()
        {
            pnlFoodInfo.ScaleInPnl().FadeInPnl();
        }

        public void OnCloseFoodInfo()
        {
            pnlFoodInfo.ScaleOutPnl().FadeOutPnl(()=>{
                ChangeBottom(true);
                StartCam();
            });
        }

        public IEnumerator LoadStreamingAsset(string path)
        { 
            WWW www = new WWW("file://" + System.IO.Path.Combine( Application.persistentDataPath, "IMG_3020.JPG"));
            while (!www.isDone) {
                yield return null;
            }

            if (!string.IsNullOrEmpty(www.error)) {
                Debug.Log (www.error);
                yield break; 
            } 
            else {
                rawimage.texture = www.texture;
            }

            yield return 0;
        }

        IEnumerator WaitDemo()
        {
            yield return new WaitForSeconds(3);
            onInitComplete();
        }
    }
}