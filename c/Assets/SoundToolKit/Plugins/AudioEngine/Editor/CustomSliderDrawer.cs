/// \author Marcin Misiek
/// \date 17.08.2018

#if UNITY_EDITOR

using System.Diagnostics;
using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    public enum Scale
    {
        Dbfs,
        Linear
    }

    public static class CustomSliderDrawer
    {
        #region public static constructor

        static CustomSliderDrawer()
        {
            MAX_LINEAR_VALUE = LogarithmicConverter.ConvertToLinear(MAX_DECIBEL_VALUE);
        }

        #endregion

        #region public static methods

        public static void DrawSlider(SerializedProperty serializedProperty, Scale scale)
        {
            switch (scale)
            {
                case Scale.Linear:
                    DrawLinearSlider(serializedProperty);
                    break;
                case Scale.Dbfs:
                    DrawLogarithmicSlider(serializedProperty);
                    break;
                default:
                    Debug.Assert(false, "[SoundToolKit.Editor] An attempt to convert volume to an unknown scale detected");
                    break;
            }
        }

        /// <summary>
        /// This method will draw Logarithmic Slider with scale of -100.0 - 12.0 [dBFs]
        /// -100 [dBFs] value will be converted to NagativeInfinity which will be 0 in linear scale
        /// </summary>
        /// <param name="serializedProperty"></param>
        public static void DrawLogarithmicSlider(SerializedProperty serializedProperty)
        {
            var decibels = LogarithmicConverter.ConvertToDecibels(serializedProperty.floatValue);

            // Convert to decibels for display purpose. 
            // Note: This won't be updated until serializedObject.ApplyModifiedProperties() is called 
            serializedProperty.floatValue = decibels <= MIN_DECIBEL_VALUE ? float.NegativeInfinity : decibels;
            EditorGUILayout.Slider(serializedProperty, MIN_DECIBEL_VALUE, MAX_DECIBEL_VALUE);

            // Convert to linear value
            serializedProperty.floatValue = LogarithmicConverter.ConvertToLinear(serializedProperty.floatValue);
        }

        /// <summary>
        /// This will draw Linear Slider with scale 0.0 to 12.0 [dBFs] in linear scale 
        /// </summary>
        /// <param name="serializedProperty"></param>
        public static void DrawLinearSlider(SerializedProperty serializedProperty)
        {
            EditorGUILayout.Slider(serializedProperty, MIN_LINEAR_VALUE, MAX_LINEAR_VALUE);
        }

        #endregion

        #region private members

        private static readonly float MAX_LINEAR_VALUE;

        private const float MIN_LINEAR_VALUE = 0.0f;

        private const float MAX_DECIBEL_VALUE = 12.0f;

        private const float MIN_DECIBEL_VALUE = -100.0f;

        #endregion
    }
}

#endif