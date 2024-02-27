using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AeroSurface)), CanEditMultipleObjects()]
public class AeroSurfaceEditor : Editor
{
    SerializedProperty config;
    SerializedProperty isControlSurface;
    SerializedProperty inputType;
    SerializedProperty inputMultiplyer;
    AeroSurface surface;

    private void OnEnable()
    {
        config = serializedObject.FindProperty("m_config");
        isControlSurface = serializedObject.FindProperty("m_isControlSurface");
        inputType = serializedObject.FindProperty("m_inputType");
        inputMultiplyer = serializedObject.FindProperty("m_inputMultiplyer");
        surface = target as AeroSurface;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(config);
        EditorGUILayout.PropertyField(isControlSurface);
        if (surface.m_isControlSurface)
        {
            EditorGUILayout.PropertyField(inputType);
            EditorGUILayout.PropertyField(inputMultiplyer);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
