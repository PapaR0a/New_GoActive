using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAGONativeGalleryControl
{
    private static HAGONativeGalleryControl m_api;
    public static HAGONativeGalleryControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGONativeGalleryControl();
            }
            return m_api;
        }
    }

    public void CheckPermission(Action callback)
    {
        NativeGallery.Permission permission = NativeGallery.CheckPermission();
        Debug.Log("NativeGalleryControl - Check camera permission result: " + permission.ToString());
        switch (permission)
        {
            case NativeGallery.Permission.Denied:
            case NativeGallery.Permission.ShouldAsk:
                NativeGallery.Permission result =  NativeGallery.RequestPermission();
                if (result == NativeGallery.Permission.Granted)
                {
                    Debug.Log("NativeGalleryControl - RequestPermission: " + result);
                    callback?.Invoke();
                }
                break;
            case NativeGallery.Permission.Granted:
                callback?.Invoke();
                break;
            default:
                break;
        }
    }

    public void PickImage(Action<Texture2D> callback)
    {
        CoroutineHelper.Call(RunPickImage(callback));
    }

    private IEnumerator RunPickImage(Action<Texture2D> callback)
    {
        yield return new WaitForSeconds(0.3f);

        int maxSize = 512;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false, false,false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                }
                callback?.Invoke(texture);
            }
            else
            {
                callback?.Invoke(null);
            }
        }, "Select a jpg image", "image/jpg", maxSize);

        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.Log("Permission result: " + permission);
            callback?.Invoke(null);
        }
    }

    public IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "My img {0}.png"));
    }

    public void PickVideo(Action callback)
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
#if !UNITY_STANDALONE
                // Play the selected video
                Handheld.PlayFullScreenMovie("file://" + path);
#endif
            }

            callback?.Invoke();
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
        callback?.Invoke();
    }
}