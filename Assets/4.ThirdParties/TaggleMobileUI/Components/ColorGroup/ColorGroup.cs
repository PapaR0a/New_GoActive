using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//change color all child
public class ColorGroup : MonoBehaviour
{
    [SerializeField]
    private Color m_color;
    [SerializeField]
    private bool m_ignoreParent;

    private List<Graphic> m_renderer;

	void Awake () {
        Init();
    }

    void Init()
    {
        m_renderer = new List<Graphic>();
        foreach (Graphic sp in gameObject.GetComponentsInChildren<Graphic>())
        {
            m_renderer.Add(sp);
        }
    }

    void UpdateColor()
    {
        if(m_renderer == null)
            Init();
        foreach (Graphic sp in m_renderer)
        {
            if(m_ignoreParent && sp.gameObject.Equals(gameObject))
                continue;
            sp.color = m_color;
        }
    }

    public void SetColor(Color color)
    {
        m_color = color;
        UpdateColor();
    }

}
