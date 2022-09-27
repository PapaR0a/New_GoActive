using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Others{
	public static class GP {
        public static int curTab = 0;

		public static float TIME_FADE_ALPHA = 0.25f;
        public static float TIME_CHANGE_TAB = 0.25f;
        public static float TIME_ANIMATION = 1f;

        public static string ToStringK(float value)
        {
            if (value >= 100000f) 
                return (value / 10000f).ToString("F1") + "B";
            else if (value >= 10000f) 
                return (value / 10000f).ToString("F1") + "M";
            else if (value >= 1000f) 
                return (value / 1000f).ToString("F1") + "K";
            else{
                return value.ToString("F1");
            }
        }

        public static void ScrollToTop(ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0, 1);
		}
		
		public static void ScrollToBottom(ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0, 0);
		}
	}

    // declare types
    public enum IconType { normal, warning, error, success};
    public enum PopupType { OK, YesNo, Update, Disclaim };

}

