using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Components
{
	public class NotifiNumber : MonoBehaviour {
		public CanvasGroup PanelCanvas { get { return GetComponent<CanvasGroup>(); } }
		public PanelCommon PnlCommon { get { return GetComponent<PanelCommon>(); } }
		public Text txtNumber;
		public int value;

		// Use this for initialization
		void Start () {
			PanelCanvas.alpha = 0f;
			gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one * 0.5f;
		}
		
		public void SetValue(int _value){
			if(value == _value)
				return;

			value = _value;
			PnlCommon.ScaleOutPnl().FadeOutPnl(()=>{
				if(value > 0){
					txtNumber.text = value>99?"99+":value.ToString();
					PnlCommon.ScaleInPnl().FadeInPnl();
				}
			});
		}
	}
}