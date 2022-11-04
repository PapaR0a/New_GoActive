using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HAGOSkinHelper : MonoBehaviour
{
	private Image m_img;

	[SerializeField]
	private HAGOSkinSprites[] m_images;

	//param
	private bool m_initialized;
	private Sprite m_defaultImage;
	private Color m_defaultColor;
	private float m_defaultWidth;
	private float m_defaultHeight;

	private void OnEnable()
	{
		if (!m_initialized)
		{
			Init();
		}

		UpdateSkin(HAGOModel.Api.GetCacheAppSkin());
	}

	private void OnDestroy()
	{
		if (m_initialized)
		{
			HAGOControl.Api.OnSkinChangedEvent -= OnSkinChangedHandler;
		}
	}

	private void Init()
	{
		m_img = GetComponent<Image>();
		m_defaultImage = m_img.sprite;
		m_defaultColor = m_img.color;
		m_defaultWidth = m_img.rectTransform.sizeDelta.x;
		m_defaultHeight = m_img.rectTransform.sizeDelta.y;

		m_initialized = true;

		HAGOControl.Api.OnSkinChangedEvent += OnSkinChangedHandler;
	}

	private void OnSkinChangedHandler(HAGOSkinType skin)
	{
		UpdateSkin(skin);
	}

	private void UpdateSkin(HAGOSkinType skin)
	{
		if (m_images == null || m_images != null && m_images.Length == 0)
		{
			return;
		}

		Sprite newSprite = m_defaultImage;
		Color newColor = m_defaultColor;
		float newSpriteWidth = m_defaultWidth;
		float newSpriteHeight = m_defaultHeight;

		bool ignoreColor = false;
		for (int i = 0; i < m_images.Length; i++)
		{
			if (m_images[i].skinType == skin)
			{
				ignoreColor = m_images[i].ignoreColor;
				newSprite = m_images[i].image;
				newColor = m_images[i].color;

				if(m_images[i].width > 0f)
					newSpriteWidth = m_images[i].width;

				if(m_images[i].height > 0f)
					newSpriteHeight = m_images[i].height;
				break;
			}
		}

		if(!ignoreColor)
		{
			m_img.color = new Color(newColor.r,newColor.g,newColor.b,newColor.a);
		}
		m_img.sprite = newSprite;
		m_img.rectTransform.sizeDelta = new Vector2(newSpriteWidth, newSpriteHeight);
	}
}

/// <summary>
/// Helper class, containing sprite parameters.
/// </summary>
[Serializable]
public class HAGOSkinSprites
{
	#region PUBLIC VARS

	/// <summary>
	/// Skin type.
	/// </summary>
	public HAGOSkinType skinType;

	/// <summary>
	/// Sprite.
	/// </summary>
	public Sprite image;

	/// <summary>
	/// Color sprite.
	/// </summary>
	public Color color;

	/// <summary>
	/// Color change ignore.
	/// </summary>
	public bool ignoreColor = false;

	/// <summary>
	/// Sprite width.
	/// </summary>
	public float width = 0f;

	/// <summary>
	/// Sprite height.
	/// </summary>
	public float height = 0f;

	#endregion
}
