using UnityEditor;

namespace Synesthesure
{
    [CustomEditor(typeof(ParticleControl))]
    public class ParticleControl_Editor : Editor
    {
        static bool showColorInfo = false;
        static bool showAmountInfo = false;
        static bool showSpeedInfo = false;
        static bool showNoiseInfo = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("note"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("particles"));

            showColorInfo = EditorGUILayout.Foldout(showColorInfo, "Color");
            if (showColorInfo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colorControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colorResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gradients"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gradientToUse"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gradient"));
            }

            showAmountInfo = EditorGUILayout.Foldout(showAmountInfo, "Amount");
            if (showAmountInfo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amountControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_amountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("amountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAmount"));
            }

            showSpeedInfo = EditorGUILayout.Foldout(showSpeedInfo, "Speed");
            if (showSpeedInfo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speedControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_speedResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_speedResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSpeed"));
            }

            showNoiseInfo = EditorGUILayout.Foldout(showNoiseInfo, "Noise");
            if (showNoiseInfo)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseStrengthControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseStrengthResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseStrengthResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseStrength"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseStrength"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseFrequencyControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseFrequencyResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseFrequencyResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseFrequency"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseFrequency"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseSpeedControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseSpeedResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseSpeedResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseSpeed"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseOctaveScaleControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseOctaveScaleResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseOctaveScaleResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseOctaveScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseOctaveScale"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noisePositionAmountControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noisePositionAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noisePositionAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoisePositionAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoisePositionAmount"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseRotationAmountControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseRotationAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseRotationAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseRotationAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseRotationAmount"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseSizeAmountControlSource"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_noiseSizeAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseSizeAmountResponseCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minNoiseSizeAmount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNoiseSizeAmount"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

}