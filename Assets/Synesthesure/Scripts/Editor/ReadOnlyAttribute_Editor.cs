using UnityEditor;
using UnityEngine;

namespace Synesthesure
{
    [CustomPropertyDrawer(typeof(ReadOnlyAtribute))]
    public class ReadOnlyAttribute_Editor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}
