using System.Collections;
using System.Collections.Generic;
using TaggleTemplate.Comm;
using UnityEngine;

public class CPEAPIService
{
    private static APIUnity m_api;
    public static APIUnity Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new APIUnity();
            }
            return m_api;
        }
        set
        {
            m_api = value;
        }
    }
}
