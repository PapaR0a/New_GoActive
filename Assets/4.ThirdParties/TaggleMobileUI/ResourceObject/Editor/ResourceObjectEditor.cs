using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ResourceObject), true)]
public class ResourceObjectEditor : Editor {

    private SerializedProperty m_monitorObject;

    private ReorderableList m_monitorContent;

    protected void OnEnable()
    {
        this.m_monitorObject = this.serializedObject.FindProperty("objects");

        this.m_monitorContent =
            new ReorderableList(this.serializedObject, this.m_monitorObject, false, false, true, true)
            {
                drawElementCallback = DrawElementCallBack,
            };
    }

    private void DrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = this.m_monitorContent.serializedProperty.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element, GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        EditorGUILayout.BeginVertical();
        //atlas content
        EditorGUI.BeginChangeCheck();
        this.m_monitorContent.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            this.serializedObject.ApplyModifiedProperties();
        }
        EditorGUILayout.EndVertical();
        this.serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
