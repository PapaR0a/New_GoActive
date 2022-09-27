using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ColorGroup))]
[CanEditMultipleObjects]
public class ColorGroupEditor : Editor
{

    private SerializedProperty m_color;
    private SerializedProperty m_ignoreParent;

    void Awake()
    {
        m_color = serializedObject.FindProperty("m_color");
        m_ignoreParent = serializedObject.FindProperty("m_ignoreParent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_color);
        EditorGUILayout.PropertyField(m_ignoreParent);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeColor(m_color.colorValue);
        }
        serializedObject.ApplyModifiedProperties();
    }

    void ChangeColor(Color color)
    {
        if (targets == null)
            return;
        foreach (ColorGroup comp in targets)
        {
            foreach (Graphic sp in comp.gameObject.GetComponentsInChildren<Graphic>())
            {
                if (m_ignoreParent.boolValue && sp.gameObject.Equals(comp.gameObject))
                    continue;
                sp.color = color;
            }
        }
    }

}
