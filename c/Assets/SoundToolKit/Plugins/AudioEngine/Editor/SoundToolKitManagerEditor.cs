/// \author Marcin Misiek
/// \date 17.08.2018

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitManager))]
    public class SoundToolKitManagerEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_hrtfProp);

            DrawScaleControl();

            EditorGUI.indentLevel += 1;

            CustomSliderDrawer.DrawSlider(m_masterVolumeProp, m_currentScale);

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_settingsProp);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region protected methods

        private void OnEnable()
        {
            m_settingsProp = serializedObject.FindProperty("m_settingsInspector");
            m_hrtfProp = serializedObject.FindProperty("m_hrtfEnabled");
            m_masterVolumeProp = serializedObject.FindProperty("m_masterVolume");
        }

        #endregion

        #region private methods

        private void DrawScaleControl()
        {
            m_currentScale = (Scale)EditorGUILayout.EnumPopup(
                new GUIContent("Volume scale", "Linear\nLogarithmic (dBFs)"),
                m_currentScale);
        }

        #endregion

        #region private fields

        private SerializedProperty m_settingsProp;

        private static Scale m_currentScale = Scale.Linear;

        private SerializedProperty m_hrtfProp;
        private SerializedProperty m_masterVolumeProp;

        #endregion
    }
}

#endif