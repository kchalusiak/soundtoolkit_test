///
/// \author Magdalena Malon <magdalena.malon@techmo.pl>
/// \date 22.11.2018
///

#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitAmbientSource))]
    [CanEditMultipleObjects]
    public class SoundToolKitAmbientSourceEditor : SourceComponentEditor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            DrawPlaybacks();
            DrawMute();
            DrawVolume();

            EditorGUI.indentLevel = SETTINGS_INDENT_LEVEL;

            EditorGUILayout.Space();

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

        private const int SETTINGS_INDENT_LEVEL = 2;

        private SerializedProperty m_playbacksProp;

        #endregion
    }
}

#endif