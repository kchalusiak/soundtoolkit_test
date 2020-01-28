/// \author Marcin Misiek
/// \date 17.08.2018

#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitSpatialSource))]
    [CanEditMultipleObjects]
    public class SoundToolKitSpatialSourceEditor : SpatialComponentEditor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            DrawPlaybacks();
            DrawMute();
            DrawVolume();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("3D sound settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.indentLevel += 1;

            DrawSimulationQuality();

            EditorGUILayout.Space();
            DrawSpatialEffectControl();
            EditorGUILayout.Space();

            DrawAttenuationControl();

            EditorGUILayout.Space();

            EditorGUI.indentLevel -= 1;

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region protected methods

        protected override void OnEnable()
        {
            base.OnEnable();

            m_playbacksProp = serializedObject.FindProperty("m_soundToolKitPlaybacks");
        }

        #endregion

        #region private methods

        private void DrawPlaybacks()
        {
            EditorGUILayout.PropertyField(m_playbacksProp, true);
        }

        #endregion

        #region private fields

        private SerializedProperty m_playbacksProp;

        #endregion
    }
}

#endif