using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAControl
{
    private static GAControl m_api;

    public static GAControl Api
    {
        get
        {
            if (m_api == null)
            {
                m_api = new GAControl();
            }
            return m_api;
        }
    }

    public void Init()
    {
        GAMapControl.Api = new GAMapControl();
        GAMissionsControl.Api = new GAMissionsControl();

        SceneHelper.LoadSceneAdditiveAsync(GAConstants.MAIN_SCENE);
    }
}
