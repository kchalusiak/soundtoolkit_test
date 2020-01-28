/// \author Michal Majewski
/// \date 09.05.2019
///
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitSceneConfiguration))]
    public class SoundToolKitSceneConfigurationEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Volume control", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawScaleControl();

            EditorGUI.indentLevel += 1;

            CustomSliderDrawer.DrawSlider(m_spatialVolumeProp, m_currentScale);
            CustomSliderDrawer.DrawSlider(m_ambientVolumeProp, m_currentScale);
            CustomSliderDrawer.DrawSlider(m_reverbVolumeProp, m_currentScale);

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial effects control", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 3 / 4;

            EditorGUILayout.PropertyField(m_attenuationEnabledProp);
            EditorGUILayout.PropertyField(m_reflectionEnabledProp);
            EditorGUILayout.PropertyField(m_scatteringEnabledProp);
            EditorGUILayout.PropertyField(m_transmissionEnabledProp);
            EditorGUILayout.PropertyField(m_diffractionEnabledProp);
            EditorGUILayout.PropertyField(m_reverbEnabledProp);

            EditorGUIUtility.labelWidth = 0;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Acoustic medium control", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.Slider(m_speedOfSoundProp, 100.0f, 1000.0f);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_dampingEnabledProp);

            EditorGUILayout.LabelField("Damping Coefficients", EditorStyles.label);
            EditorGUILayout.Space();

            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(m_dampingCoefficientsProp, true);
            EditorGUI.indentLevel -= 1;

            for (int i = 0; i < 20; i++)
            {
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region protected methods

        private void OnEnable()
        {
            m_spatialVolumeProp = serializedObject.FindProperty("m_spatialVolume");
            m_ambientVolumeProp = serializedObject.FindProperty("m_ambientVolume");
            m_reverbVolumeProp = serializedObject.FindProperty("m_reverbVolume");

            m_attenuationEnabledProp = serializedObject.FindProperty("m_attenuationEnabled");
            m_reflectionEnabledProp = serializedObject.FindProperty("m_reflectionEnabled");
            m_scatteringEnabledProp = serializedObject.FindProperty("m_scatteringEnabled");
            m_transmissionEnabledProp = serializedObject.FindProperty("m_transmissionEnabled");
            m_diffractionEnabledProp = serializedObject.FindProperty("m_diffractionEnabled");
            m_reverbEnabledProp = serializedObject.FindProperty("m_reverbEnabled");

            m_speedOfSoundProp = serializedObject.FindProperty("m_speedOfSound");
            m_dampingEnabledProp = serializedObject.FindProperty("m_dampingEnabled");
            m_dampingCoefficientsProp = serializedObject.FindProperty("m_dampingCoefficients");
        }

        #endregion

        #region private methods

        private void DrawScaleControl()
        {
            m_currentScale = (Scale)EditorGUILayout.EnumPopup(
                new GUIContent("Volume scale", "Linear\nLogarithmic (dBFs)"),
                m_currentScale);
        }

        private void DrawCoefficientsProperty(SerializedProperty property, string label)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(label, EditorStyles.label);
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(property, true);
            EditorGUI.indentLevel -= 1;

            for (int i = 0; i < 20; i++)
            {
                EditorGUILayout.Space();
            }
        }

        #endregion

        #region private fields

        private static Scale m_currentScale = Scale.Linear;

        private SerializedProperty m_spatialVolumeProp;
        private SerializedProperty m_ambientVolumeProp;
        private SerializedProperty m_reverbVolumeProp;

        private SerializedProperty m_attenuationEnabledProp;
        private SerializedProperty m_reflectionEnabledProp;
        private SerializedProperty m_scatteringEnabledProp;
        private SerializedProperty m_transmissionEnabledProp;
        private SerializedProperty m_diffractionEnabledProp;
        private SerializedProperty m_reverbEnabledProp;

        private SerializedProperty m_speedOfSoundProp;
        private SerializedProperty m_dampingEnabledProp;
        private SerializedProperty m_dampingCoefficientsProp;

        #endregion
    }
}

#endif