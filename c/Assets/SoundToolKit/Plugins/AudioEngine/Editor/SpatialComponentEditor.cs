///
/// \author Magdalena Malon <magdalena.malon@techmo.pl>
/// \date 22.11.2018
///

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CanEditMultipleObjects]
    public class SpatialComponentEditor : SourceComponentEditor
    {
        #region protected methods

        protected override void OnEnable()
        {
            base.OnEnable();

            m_simulationQualityProp = serializedObject.FindProperty("m_simulationQuality");

            m_hrtfSpatializationEnabledProp = serializedObject.FindProperty("m_hrtfSpatializationEnabled");
            m_attenuationEnabledProp = serializedObject.FindProperty("m_attenuationEnabled");
            m_reflectionEnabledProp = serializedObject.FindProperty("m_reflectionEnabled");
            m_scatteringEnabledProp = serializedObject.FindProperty("m_scatteringEnabled");
            m_transmissionEnabledProp = serializedObject.FindProperty("m_transmissionEnabled");
            m_diffractionEnabledProp = serializedObject.FindProperty("m_diffractionEnabled");
            m_reverbEnabledProp = serializedObject.FindProperty("m_reverbEnabled");

            m_maxDistanceProp = serializedObject.FindProperty("m_maxDistance");
            m_minDistanceProp = serializedObject.FindProperty("m_minDistance");
            m_attenuationProp = serializedObject.FindProperty("m_attenuation");
            m_attenuationCurveProp = serializedObject.FindProperty("m_attenuationCurve");
        }

        protected void DrawSimulationQuality()
        {
            EditorGUILayout.Slider(m_simulationQualityProp, 0.0f, MAX_SIMULATION_QUALITY);
        }

        protected void DrawSpatialEffectControl()
        {
            m_showSpatialEffectControl = EditorGUILayout.Foldout(m_showSpatialEffectControl, "Spatial Effects Control");

            if (m_showSpatialEffectControl)
            {
                EditorGUI.indentLevel += 1;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 3 / 4;

                EditorGUILayout.PropertyField(m_hrtfSpatializationEnabledProp);
                EditorGUILayout.PropertyField(m_attenuationEnabledProp);
                EditorGUILayout.PropertyField(m_reflectionEnabledProp);
                EditorGUILayout.PropertyField(m_scatteringEnabledProp);
                EditorGUILayout.PropertyField(m_transmissionEnabledProp);
                EditorGUILayout.PropertyField(m_diffractionEnabledProp);
                EditorGUILayout.PropertyField(m_reverbEnabledProp);

                EditorGUIUtility.labelWidth = 0;
                EditorGUI.indentLevel -= 1;
            }
        }

        protected void DrawAttenuationControl()
        {
            if (m_attenuationEnabledProp.boolValue)
            {
                DrawAttenuation();
                DrawMinDistance();
                DrawMaxDistance();
                DrawAttenuationEditor();
            }
        }


        #endregion

        #region private methods
        
        private void DrawMaxDistance()
        {
            var currentAttenuation = (SoundAttenuation)m_attenuationProp.enumValueIndex;

            if (currentAttenuation == SoundAttenuation.Linear || currentAttenuation == SoundAttenuation.ReverseLog ||
                currentAttenuation == SoundAttenuation.Logarithmic || currentAttenuation == SoundAttenuation.Inverse ||
                currentAttenuation == SoundAttenuation.Custom)
            {
                EditorGUILayout.PropertyField(m_maxDistanceProp);
            }
        }

        private void DrawMinDistance()
        {
            var currentAttenuation = (SoundAttenuation)m_attenuationProp.enumValueIndex;

            if (currentAttenuation != SoundAttenuation.Custom)
            {
                EditorGUILayout.PropertyField(m_minDistanceProp);
            }
        }

        private void DrawAttenuation()
        {
            EditorGUILayout.PropertyField(m_attenuationProp);
        }

        private void DrawAttenuationEditor()
        {
            var currentAttenuation = (SoundAttenuation)m_attenuationProp.enumValueIndex;

            if (currentAttenuation == SoundAttenuation.Custom)
            {
                // width of the CurveField editor depends on the max distance property - distance over which attenuation occurs
                // height of CurveField is hardcoded as 1 since it is maximum value of Attenuation Factor (AF=1 means no attenuation)
                EditorGUILayout.CurveField(m_attenuationCurveProp, new UnityEngine.Color(.044f, .206f, .209f), new UnityEngine.Rect(0, 0, m_maxDistanceProp.floatValue, 1));
            }
        }

        #endregion

        #region private members

        private const float MAX_SIMULATION_QUALITY = 1.0f;

        private SerializedProperty m_simulationQualityProp;

        private SerializedProperty m_hrtfSpatializationEnabledProp;
        private SerializedProperty m_attenuationEnabledProp;
        private SerializedProperty m_reflectionEnabledProp;
        private SerializedProperty m_scatteringEnabledProp;
        private SerializedProperty m_transmissionEnabledProp;
        private SerializedProperty m_diffractionEnabledProp;
        private SerializedProperty m_reverbEnabledProp;

        private SerializedProperty m_maxDistanceProp;
        private SerializedProperty m_minDistanceProp;
        private SerializedProperty m_attenuationProp;
        private SerializedProperty m_attenuationCurveProp;

        private static bool m_showSpatialEffectControl = false;

        #endregion
    }
}

#endif