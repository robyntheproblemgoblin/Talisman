using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Move))]
public class MoveEditor : Editor
{
    SerializedProperty m_animation;
    SerializedProperty m_hitBoxList;

    public void OnEnable()
    {
        m_animation = serializedObject.FindProperty("m_moveAnimation");
        m_hitBoxList = serializedObject.FindProperty("m_moveHitBoxes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(m_animation);

        if (m_animation.objectReferenceValue != null)
        {
            EditorGUILayout.PropertyField(m_hitBoxList);
        }
        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();

    }
}