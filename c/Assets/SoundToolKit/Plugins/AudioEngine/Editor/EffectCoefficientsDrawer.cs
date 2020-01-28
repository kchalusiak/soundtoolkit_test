/// \author Michal Majewski
/// \date 19.02.2019

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomPropertyDrawer(typeof(EffectCoefficients))]
    public class EffectCoefficientsDrawer : PropertyDrawer
    {
        #region public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            for (int i = 0; i < FREQUENCY_BANDS.Length; i++)
            {
                var rectangle = new Rect(position.x, position.y + i * position.height, position.width, position.height);
                var bandName = "m_band" + FREQUENCY_BANDS[i];
                var bandProp = property.FindPropertyRelative(bandName);

                bandProp.floatValue = DrawFloatSlider(rectangle, bandProp.floatValue, FREQUENCY_BANDS[i]);
            }

            EditorGUI.EndProperty();
        }

        #endregion

        #region private methods

        private float DrawFloatSlider(Rect rectangle, float freqBand, string bandName)
        {
            return EditorGUI.Slider(rectangle, new GUIContent(bandName, "Value of an effect coefficient in a " +
                bandName + " frequency band ranging from 0 - no effect to 1 - full effect."),
                freqBand, 0f, 1f);
        }

        #endregion

        #region private members

        private static readonly string[] FREQUENCY_BANDS =
            new string[] { "125Hz", "250Hz", "500Hz", "1000Hz", "2000Hz", "4000Hz", "8000Hz", "16000Hz" }; 

        #endregion
    }
}

#endif
