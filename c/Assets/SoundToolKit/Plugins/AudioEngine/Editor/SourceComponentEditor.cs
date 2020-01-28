/// \author Marcin Misiek
/// \date 06.09.2018

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CanEditMultipleObjects]
    public class SourceComponentEditor : UnityEditor.Editor
    {
        #region protected methods

        protected virtual void OnEnable()
        {
            EditorGUI.showMixedValue = true;

            m_playOnAwakeProp = serializedObject.FindProperty("playOnAwake");
            m_muteProp = serializedObject.FindProperty("m_muted");
            m_volumeProp = serializedObject.FindProperty("m_volume");
        }

        protected void DrawPlayOnAwake()
        {
            EditorGUILayout.PropertyField(m_playOnAwakeProp);
        }

        protected void DrawMute()
        {
            EditorGUILayout.PropertyField(m_muteProp);
        }

        protected void DrawVolume()
        {
            if (!m_volumeProp.hasMultipleDifferentValues)
            {
                m_currentScale = (Scale)EditorGUILayout.EnumPopup(
                    new GUIContent("Volume scale", "Linear\nLogarithmic (dBFs)"),
                    m_currentScale);

                CustomSliderDrawer.DrawSlider(m_volumeProp, m_currentScale);
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.EnumPopup("Volume scale", Scale.Linear);
                GUI.enabled = true;

                CustomSliderDrawer.DrawLinearSlider(m_volumeProp);
            }
        }
       
        #endregion

        #region private members

        private static Scale m_currentScale = Scale.Linear;

        private SerializedProperty m_playOnAwakeProp;

        private SerializedProperty m_muteProp;

        private SerializedProperty m_volumeProp;

        #endregion
    }
}

#endif