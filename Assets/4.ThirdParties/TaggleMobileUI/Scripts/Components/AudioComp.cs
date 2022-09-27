using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Taggle.HealthApp.Others;

namespace Taggle.HealthApp.Components
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioComp : MonoBehaviour
	{
		public AudioSource audSource;
		public AudioClip[] clipBG;
		public AudioClip clipClick;

		public static AudioComp Instance;

		void Awake(){
			audSource = GetComponent<AudioSource>();
		}
		void Start ()
		{
			Instance = this;
			if(clipBG.Length != 0 && clipBG[0] != null){
				PlayBG(0);
			}
		}

		public void PlayBG(int i)
		{
			audSource.DOFade(0, GP.TIME_FADE_ALPHA).OnComplete(()=>{
				audSource.Stop();
				audSource.clip = clipBG[i];
				audSource.Play();
				audSource.DOFade(0, GP.TIME_FADE_ALPHA);
			});
		}

		public void PlayClipClick()
		{
			audSource.PlayOneShot(clipClick);
		}

	}
}