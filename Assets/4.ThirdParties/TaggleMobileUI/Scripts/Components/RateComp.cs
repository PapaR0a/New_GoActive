using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Taggle.HealthApp.Components
{
	public class RateComp : MonoBehaviour {
		public CanvasGroup mainCanvas;
		
		public List<Image> imgRate;
		[Range(MIN_RATE,MAX_RATE)]
		public int value;
		public const int MIN_RATE = -1;
		public const int MAX_RATE = 4;
		public const float MIN_ALPHA = 0.15f;
		public const float MAX_ALPHA = 1f;
		
		// Use this for initialization
		void Start () {
			if(mainCanvas == null)
				mainCanvas = gameObject.GetComponent<CanvasGroup>();
		}

		public void SetValue(int v){
			value = Mathf.Clamp(v, MIN_RATE, MAX_RATE);
			RenderView();
		}

		public virtual void RenderView(){
			for(int i=0; i<imgRate.Count;i++){
				Color c = imgRate[i].color;
				c.a = (i <= value?MAX_ALPHA:MIN_ALPHA);
				imgRate[i].color = c;
			}
		}
	}
}