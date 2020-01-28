/// \author Michal Majewski
/// \date 16.11.2018

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomPropertyDrawer(typeof(SoundToolKitPlayback))]
    public class PlaybackDrawer : PropertyDrawer
    {
        #region public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;          

            var sampleRect = new Rect(position.x, position.y + (0.05f * position.height / 3.8f), position.width, position.height / 3.8f);
            var volumeSliderRect = new Rect(position.x, position.y + (1.25f * position.height / 3.8f), position.width, position.height / 3.8f);
            var loopRect = new Rect(position.x, position.y + (2.5f * position.height / 3.8f), EditorGUIUtility.labelWidth, position.height / 3.8f);
            var autoPlayRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y + (2.5f * position.height / 3.8f), position.width / 2, position.height / 4.0f);
            var divisorRect = new Rect(position.x, position.y + (3.65f * position.height / 3.8f), position.width, 1);

            EditorGUI.PropertyField(sampleRect, property.FindPropertyRelative("m_sample"));

            EditorGUI.Slider(volumeSliderRect, property.FindPropertyRelative("m_volume"), 0.0f, 4.0f);

            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 5f;
            EditorGUI.PropertyField(loopRect, property.FindPropertyRelative("m_looped"));

            EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 4.2f;
            EditorGUI.PropertyField(autoPlayRect, property.FindPropertyRelative("m_autoPlay"));

            EditorGUI.DrawRect(divisorRect, m_divisorColor);

            EditorGUIUtility.labelWidth = 0;
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 3.8f;
        }

        #endregion

        #region private members

        private Color m_divisorColor = new Color(0.5f, 0.5f, 0.5f, 1);

        #endregion
    }
}

#endif
