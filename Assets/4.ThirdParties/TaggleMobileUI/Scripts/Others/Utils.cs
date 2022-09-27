using DG.Tweening;
using Honeti;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Others{
    public static class Utils
    {
        public static string FullUUID (string _FullUID, string uuid)
        {
            return _FullUID.Replace ("****", uuid);
        }

        public static bool IsEqual (string _FullUID, string uuid1, string uuid2)
        {
            if (uuid1.Length == 4)
            {
                uuid1 = FullUUID (_FullUID, uuid1);
            }
            if (uuid2.Length == 4)
            {
                uuid2 = FullUUID (_FullUID, uuid2);
            }
            return (uuid1.ToUpper ().CompareTo (uuid2.ToUpper ()) == 0);
        }
        public static bool IsEqual (string _FullUID, string uuid1, List<string> uuid2)
        {
            for (int i = 0; i < uuid2.Count; ++i)
            {
                if (IsEqual (_FullUID, uuid1, uuid2 [i]))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsEqual (string uuid1, string uuid2)
        {
            return (uuid1.ToUpper ().CompareTo (uuid2.ToUpper ()) == 0);
        }
        public static bool IsEqual (string uuid1, List<string> uuid2)
        {
            for (int i = 0; i < uuid2.Count; ++i)
            {
                if (IsEqual (uuid1, uuid2 [i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static T GetLastOfList<T> (List<T> t)
        {
            if (t.Count == 0)
            {
                return default (T);
            }
            return t [t.Count - 1];
        }

        public static T GetRandomFromList<T> (List<T> t)
        {
            if (t.Count == 0)
            {
                return default (T);
            }
            return t [Random.Range (0, t.Count)];
        }

        public static void UpdateLocalisedText(Text changingTxt, string _translationKey, string[] param = null)
        {
            changingTxt.text = _translationKey;
            var localistTxt = changingTxt.GetComponent<I18NText>();
            localistTxt.SetKey(_translationKey);
            if (param != null)
                localistTxt.UpdateParameters(param);
            
            localistTxt.updateTranslation(true);
        }

        public static void ResetTransText(Text changingTxt){
            var localistTxt = changingTxt.GetComponent<I18NText>();
        }

        public static string GetValueTrans(string key){
            return I18N.instance.getValue(key);
        }

        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}
