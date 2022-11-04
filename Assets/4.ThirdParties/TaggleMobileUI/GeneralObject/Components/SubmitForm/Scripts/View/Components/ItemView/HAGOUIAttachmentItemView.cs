using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HAGOUIAttachmentItemView : MonoBehaviour
{
	private Button m_btnDelete;
	private RawImage m_rimgItem;
	private AudioSource m_audSource;
	private Button m_btnAudio;
	private GameObject m_durationObj;
	private Image m_imgAudioDuration;
	private Image m_imgIcon;

	//param
	private HAGOUIAttachmentItemDTO m_data;
	private bool m_isEditMode;
	private Coroutine m_coroutinePlayAudio;
	private Color m_colorIconAudioDefault;
	private Color m_colorIconAudioActive;

	public void Init(bool isEditMode, HAGOUIAttachmentItemDTO data)
	{
		m_data = data;
		m_isEditMode = isEditMode;

		//find reference
		m_btnDelete = transform.Find("BtnDelete").GetComponent<Button>();
		m_rimgItem = transform.Find("RimgPhoto").GetComponent<RawImage>();
		m_audSource = transform.Find("Audio").GetComponent<AudioSource>();
		m_btnAudio = transform.Find("Audio").GetComponent<Button>();
		m_durationObj = transform.Find("Audio/Duration").gameObject;
		m_imgAudioDuration = transform.Find("Audio/Duration/ImgFill").GetComponent<Image>();
		m_imgIcon = transform.Find("Audio/ImgIcon").GetComponent<Image>();

		//handle value
		m_colorIconAudioDefault = m_imgIcon.color;
		m_colorIconAudioActive = new Color(35f/255f, 177f/255f, 136f/255f, 255f/255f);
		//
		SwitchView();
		HandleViewByType();

		//add listener
		m_btnDelete.onClick.AddListener(DeleteOnClick);
		m_btnAudio.onClick.AddListener(AudioOnClick);
	}

    private void AudioOnClick()
    {
		// Debug.Log("AudioOnClick is null: " + (m_coroutinePlayAudio == null));
		
        if(m_coroutinePlayAudio != null)
		{
			StopCoroutine(m_coroutinePlayAudio);
			ResetAudioView();
			m_coroutinePlayAudio = null;
		}
		else
		{
			m_coroutinePlayAudio = StartCoroutine(PlayAudio());
		}
    }

    private void DeleteOnClick()
    {
        gameObject.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() => { Destroy(gameObject); });
    }

	private void SwitchView()
	{
		m_rimgItem.gameObject.SetActive(m_data.Type == HAGOUIAttachmentType.Image);
		m_btnAudio.gameObject.SetActive(m_data.Type == HAGOUIAttachmentType.Audio);
	}

	private void HandleViewByType()
	{
		if(m_isEditMode)
		{
			switch(m_data.Type)
			{
				case HAGOUIAttachmentType.Image:
					m_rimgItem.texture = (Texture2D)m_data.Resource;
					break;

				case HAGOUIAttachmentType.Audio:
					break;

				default:
					break;
			}
		}
		else
		{
			switch(m_data.Type)
			{
				case HAGOUIAttachmentType.Image:
					m_rimgItem.LoadTexture(m_data.Url);
					break;

				case HAGOUIAttachmentType.Audio:
					if(m_data.Resource == null)
					{
						StartCoroutine(DownloadAudio(m_data.Url));
					}

					//test
					// StartCoroutine(DownloadAudio("https://vnso-zn-16-tf-mp3-s1-zmp3.zadn.vn/520737eb16acfff2a6bd/6972486596562639576?authen=exp=1577843890~acl=/520737eb16acfff2a6bd/*~hmac=44d1a1486969e413337233c4a8f13924&filename=Banh-Mi-Khong-Dat-G-DuUyen.mp3"));
					break;

				default:
					break;
			}
		}
	}

	private IEnumerator DownloadAudio(string url)
	{
		WWW www = new WWW (url);
		Debug.Log ("loading...");
		yield return www;
		Debug.Log ("done.");

		AudioClip clip = www.GetAudioClipCompressed(true, AudioType.MPEG);
		m_data.Resource = clip;
	}

	private IEnumerator PlayAudio()
	{
		//reset view
		ResetAudioView();

		//handle value
		AudioClip clip = m_data.Resource as AudioClip;
		float duration = clip.length;

		//play audio & animation
		m_audSource.PlayOneShot(clip);
		m_imgAudioDuration.DOFillAmount(1f, duration).SetEase(Ease.Linear);
		m_imgIcon.color = m_colorIconAudioActive;
		m_durationObj.SetActive(true);

		yield return new WaitForSeconds(duration);

		ResetAudioView();

		yield return new WaitForEndOfFrame();
	}

	private void ResetAudioView()
	{
		//kill tween
		DOTween.Complete(m_imgAudioDuration);
		m_imgAudioDuration.fillAmount = 0f;
		//
		m_imgIcon.color = m_colorIconAudioDefault;
		m_audSource.Stop();
		m_durationObj.SetActive(false);
	}

	public long GetID()
    {
        return m_data != null ? m_data.ID : transform.GetSiblingIndex();
    }

	public string GetJsonValue()
	{
		return GetValue().ToString();
	}

    public object GetValue()
    {
        switch(m_data.Type)
		{
			case HAGOUIAttachmentType.Image:
				return HAGOUtils.GetBase64StringFromTexture(m_data.Resource as Texture2D);

			case HAGOUIAttachmentType.Audio:
				return HAGOUtils.GetDataFromAudioClip(m_data.Resource as AudioClip);

			default:
				return null;
		}
    }
}
