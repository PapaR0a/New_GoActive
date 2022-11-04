using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HAGOSkinType
{
    Default,
    Kids
}

public class HAGOModel
{
    private static HAGOModel m_api;
    public static HAGOModel Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new HAGOModel();
            }
            return m_api;
        }
    }

    public bool IsLoading { get; set; }
    //
    private HAGOSkinType m_skinType { get; set; } = HAGOSkinType.Default;

    public void CacheAppSkin(HAGOSkinType skinType)
    {
        m_skinType = skinType;
    }

    public HAGOSkinType GetCacheAppSkin()
    {
        return m_skinType;
    }
}