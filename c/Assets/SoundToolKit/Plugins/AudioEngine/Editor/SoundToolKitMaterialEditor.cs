/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 19.02.2019

#if UNITY_EDITOR

using SoundToolKit.Unity.Utils;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitMaterial))]
    public class SoundToolKitMaterialEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_visualMaterialProp);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Active Effects", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel += 1;

            EditorGUILayout.PropertyField(m_absorptionEnabledProp, new GUIContent("Absorption",
                "Controls if the absorption effect is enabled on this Material"));
            EditorGUILayout.PropertyField(m_scatteringEnabledProp, new GUIContent("Scattering",
                "Controls if the scattering effect is enabled on this Material"));
            EditorGUILayout.PropertyField(m_transmissionEnabledProp, new GUIContent("Transmission",
                "Controls if the transmission effect is enabled on this Material"));

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Material Editor", EditorStyles.boldLabel);

            if (EditorApplication.isPlaying)
            {
                // This will cause the internal StkMaterial to actually load so that the user may create a custom material
                //  based on it even if the inspected material is not on the scene (is not assigned to any mesh)
                if (((SoundToolKitMaterial)target).GetStkMaterial() != null)
                {
                    EditorGUI.indentLevel += 1;

                    DrawCoefficientsProperty(m_absorptionCoeffsProp, "Absorption Coefficients");
                    DrawCoefficientsProperty(m_scatteringCoeffsProp, "Scattering Coefficients");
                    DrawCoefficientsProperty(m_transmissionCoeffsProp, "Transmission Coefficients");

                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Save Custom Material"))
                    {
                        SoundToolKitMaterialAssetLoader.SerializeToFile((SoundToolKitMaterial)target);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Material is not valid for customization", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Material customization available only in PlayMode", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region private methods

        private void OnEnable()
        {
            m_absorptionEnabledProp = serializedObject.FindProperty("m_absorptionEnabled");
            m_scatteringEnabledProp = serializedObject.FindProperty("m_scatteringEnabled");
            m_transmissionEnabledProp = serializedObject.FindProperty("m_transmissionEnabled");

            m_visualMaterialProp = serializedObject.FindProperty("m_visualMaterial");

            m_absorptionCoeffsProp = serializedObject.FindProperty("m_absorptionCoefficients");
            m_scatteringCoeffsProp = serializedObject.FindProperty("m_scatteringCoefficients");
            m_transmissionCoeffsProp = serializedObject.FindProperty("m_transmissionCoefficients");
        }

        private void DrawCoefficientsProperty(SerializedProperty property, string label)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(property, true);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
            WideSpace();
        }

        private void WideSpace()
        {
            // Chosen arbitrarily - this amount of spaces separates the EffectCoefficientDrawers nicely
            for (int i = 0; i < 19; i++)
            {
                EditorGUILayout.Space();
            }
        }

        #endregion

        #region private members

        private SerializedProperty m_absorptionEnabledProp;
        private SerializedProperty m_scatteringEnabledProp;
        private SerializedProperty m_transmissionEnabledProp;

        private SerializedProperty m_visualMaterialProp;

        private SerializedProperty m_absorptionCoeffsProp;
        private SerializedProperty m_scatteringCoeffsProp;
        private SerializedProperty m_transmissionCoeffsProp;

        #endregion
    }
}

#endif