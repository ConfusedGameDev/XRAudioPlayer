using UnityEditor;

namespace Synesthesure
{
    [CustomEditor(typeof(PositionModulation))]
    public class PositionModulation_Editor : Editor
    {
        private SerializedProperty note;
        private SerializedProperty theObject;
        private SerializedProperty controlSource;
        private SerializedProperty _responseCurve;
        private SerializedProperty responseCurve;
        private SerializedProperty method;

        private SerializedProperty startPosition;
        private SerializedProperty endPosition;

        private SerializedProperty offsetPosition;

        private SerializedProperty startTransformPosition;
        private SerializedProperty endTransformPosition;

        private void OnEnable()
        {
            // Initialize SerializedProperties
            note = serializedObject.FindProperty("note");
            theObject = serializedObject.FindProperty("theObject");
            controlSource = serializedObject.FindProperty("controlSource");
            _responseCurve = serializedObject.FindProperty("_responseCurve");
            responseCurve = serializedObject.FindProperty("responseCurve");

            method = serializedObject.FindProperty("method");

            startPosition = serializedObject.FindProperty("startPosition");
            endPosition = serializedObject.FindProperty("endPosition");

            offsetPosition = serializedObject.FindProperty("offsetPosition");

            startTransformPosition = serializedObject.FindProperty("startTransformPosition");
            endTransformPosition = serializedObject.FindProperty("endTransformPosition");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display the dropdown list
            EditorGUILayout.PropertyField(note);
            EditorGUILayout.PropertyField(theObject);
            EditorGUILayout.PropertyField(controlSource);
            EditorGUILayout.PropertyField(_responseCurve);
            EditorGUILayout.PropertyField(responseCurve);
            EditorGUILayout.PropertyField(method);

            // Show corresponding float field based on the selected option
            switch (method.intValue)
            {
                case 0:
                    EditorGUILayout.PropertyField(startPosition);
                    EditorGUILayout.PropertyField(endPosition);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(offsetPosition);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(startTransformPosition);
                    EditorGUILayout.PropertyField(endTransformPosition);
                    break;
            }

            // Apply changes to the serializedObject
            serializedObject.ApplyModifiedProperties();
        }
    }
}
