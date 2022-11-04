using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DG.Tweening;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class HAGOUtils
{
    public static string GetMonthName(int index)
    {
        string monthName = string.Empty;
        switch (index)
        {
            case 1:
                monthName = HAGOLangConstant.JANUARY;
                break;
            case 2:
                monthName = HAGOLangConstant.FEBRUARY;
                break;
            case 3:
                monthName = HAGOLangConstant.MARCH;
                break;
            case 4:
                monthName = HAGOLangConstant.APRIL;
                break;
            case 5:
                monthName = HAGOLangConstant.MAY;
                break;
            case 6:
                monthName = HAGOLangConstant.JUNE;
                break;
            case 7:
                monthName = HAGOLangConstant.JULY;
                break;
            case 8:
                monthName = HAGOLangConstant.AUGUST;
                break;
            case 9:
                monthName = HAGOLangConstant.SEPTEMBER;
                break;
            case 10:
                monthName = HAGOLangConstant.OCTOBER;
                break;
            case 11:
                monthName = HAGOLangConstant.NOVEMBER;
                break;
            case 12:
                monthName = HAGOLangConstant.DECEMBER;
                break;
            default:
                return string.Empty;
        }
        
        return I18N.instance.getValue(monthName);
    }

    public static HAGOConnectType ParseConnectType(string connectType)
    {
        switch(connectType)
        {
            case HAGOServiceKey.PARAM_IHEALTH_SDK:
                return HAGOConnectType.IHEALTH_SDK;

            case TaggleTemplate.Comm.DeviceConnectionType.VIATOM_SDK:
                return HAGOConnectType.VIATOM_SDK;

            case HAGOServiceKey.PARAM_STANDARD_PROFILE:
            default:
                return HAGOConnectType.BLUETOOTH_PROFILE;
        }
    }

    public static DateTime GetDateTimeFromEpoch(long seconds)
    {
        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return date.AddSeconds(seconds).ToLocalTime();
    }

    public static DateTime GetDateTimeFromEpochUtc(long seconds)
    {
        DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return date.AddSeconds(seconds);
    }

    public static long GetEpochTimeFromDateTime(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long) Math.Floor(diff.TotalSeconds);
    }

    public static bool IsLangKey(string title)
    {
        return title?.StartsWith("^") ?? false;
    }

    public static Type GetComponentType(string compKey)
    {
        if(HAGOSubmitFormModel.Api.ComponentDict.ContainsKey(compKey))
        {
            return HAGOSubmitFormModel.Api.ComponentDict[compKey];
        }

        return null;
    }

    public static string GetComponentKey(Type type)
    {
        foreach(KeyValuePair<string, Type> comp in HAGOSubmitFormModel.Api.ComponentDict)
        {
            if(comp.Value == type)
            {
                return comp.Key;
            }
        }

        return string.Empty;
    }

    public static Type GetComponentViewType(string compKey)
    {
        if(HAGOSubmitFormModel.Api.ComponentViewDict.ContainsKey(compKey))
        {
            return HAGOSubmitFormModel.Api.ComponentViewDict[compKey];
        }

        return null;
    }

    public static MethodInfo GetComponentViewMethod(GameObject go, out string typeStr, out Type type)
    {
        type = null;
        typeStr = string.Empty;
        // var comp = go.GetComponent<IFoo>();
        foreach(var compView in HAGOSubmitFormModel.Api.ComponentViewDict)
        {

            type = compView.Value;
            typeStr = compView.Key.ToString();
            Debug.Log("=====typeStr " + typeStr);

            MethodInfo method = type.GetMethod("ExportView", new Type[]{ typeof(int) });
            return method;
        }

        return null;
    }

    public static Color ParseColorFromString(string color)
    {
        Color result;
        if (ColorUtility.TryParseHtmlString(color, out result))
        {
            return result;
        }
        return Color.white;
    }

    public static void ScrollVerticalTo(ScrollRect scrollRect, RectTransform contentPanel, RectTransform target, Action callback = null)
    {
        Canvas.ForceUpdateCanvases();

        DOTween.Complete(contentPanel);
        contentPanel.DOAnchorPosY(
            ((Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)).y
            - ((Vector2)scrollRect.transform.InverseTransformPoint(target.position)).y
            - 48f,
            0.3f
        ).OnComplete(() => {
            callback?.Invoke();
        });
    }
    
    public static void ScrollTo(ScrollRect scrollRect, RectTransform target, Action callback = null)
    {
        Canvas.ForceUpdateCanvases();

        DOTween.Complete(scrollRect.content);
        scrollRect.content.DOAnchorPos(
            (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position),
            0.3f
        ).OnComplete(() => {
            callback?.Invoke();
        });
    }

    public static string GetBase64StringFromTexture(Texture2D texture)
    {
        if(texture == null)
        {
            return string.Empty;
        }
        
        byte[] bytes = texture.EncodeToJPG();
        return "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
    }

    public static Byte[] GetDataFromAudioClip(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        Byte[] bytesData = new Byte[samples.Length * 2];

        int rescaleFactor = 32767; 

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }

    public static string ReplaceEmojiName(string unicodeRaw) {
        var chars = new List<char>();
        // some characters are multibyte in UTF32, split them
        foreach (var point in unicodeRaw.Split('-'))
        {
            // parse hex to 32-bit unsigned integer (UTF32)
            uint unicodeInt = uint.Parse(point, System.Globalization.NumberStyles.HexNumber);
            // convert to bytes and get chars with UTF32 encoding
            chars.AddRange(Encoding.UTF32.GetChars(BitConverter.GetBytes(unicodeInt)));
        }
        // this is resulting emoji
        return new string(chars.ToArray());
    }
}
