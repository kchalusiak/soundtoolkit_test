/// \author Michal Majewski
/// \date 19.04.2019

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomPropertyDrawer(typeof(VolumeControlledSample))]
    public class VolumeControlledSampleDrawer : PropertyDrawer
    {
        #region public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;          

            var sampleRect = new Rect(position.x, position.y + (0.05f * position.height / 2.2f), position.width, position.height / 2.2f);
            var volumeSliderRect = new Rect(position.x, position.y + (1.1f * position.height / 2.2f), position.width, position.height / 2.2f);

            EditorGUI.PropertyField(sampleRect, property.FindPropertyRelative("m_sample"));

            EditorGUI.Slider(volumeSliderRect, property.FindPropertyRelative("m_volume"), 0.0f, 4.0f);

            EditorGUIUtility.labelWidth = 0;
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2.2f;
        }

        #endregion
    }
}

#endif
