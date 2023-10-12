using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(ManaPipe))]
[CanEditMultipleObjects]
public class MyCustomScriptEditor : Editor
{
    private List<SerializedProperty> properties;

    private void OnEnable()
    {
        string[] hiddenProperties = new string[] { "m_input", "m_output", "m_interactMessage", "m_bridge" }; //fields you do not want to show go here
        properties = EditorHelper.GetExposedProperties(this.serializedObject, hiddenProperties);
    }

    public override void OnInspectorGUI()
    {
        //We draw only the properties we want to display here
        foreach (SerializedProperty property in properties)
        {
            EditorGUILayout.PropertyField(property, true);
        }
        serializedObject.ApplyModifiedProperties();
    }
}