/// \author Marcin Misiek
/// \date 29.08.2018

#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitStreamingSource))]
    [CanEditMultipleObjects]
    public class SoundToolKitStreamingSourceEditor : SpatialComponentEditor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            DrawPlayOnAwake();
            DrawMute();
            DrawVolume();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("3D sound settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel = SETTINGS_INDENT_LEVEL;

            DrawSimulationQuality();
            DrawSpatialEffectControl();
            DrawAttenuationControl();

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region protected methods

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        #endregion
        
        #region private fields

        private const int SETTINGS_INDENT_LEVEL = 2;

        #endregion
    }
}

#endif