/// \author Marcin Misiek
/// \date 17.08.2018

#if UNITY_EDITOR

using SoundToolKit.Unity.Extensions;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitSamplesLoader))]
    public class SoundToolKitSamplesLoaderEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSamples();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region private methods

        private void OnEnable()
        {
            m_stkSamples = serializedObject.FindProperty("m_soundToolKitSamplesReadOnly");
        }

        private void DrawSamples()
        {
            GUI.enabled = !Application.isPlaying;
            EditorGUILayout.PropertyField(m_stkSamples, new GUIContent("SoundToolKit Samples"), includeChildren: true);
            GUI.enabled = true;
        }

        #endregion

        #region private fields

        private SerializedProperty m_stkSamples;

        #endregion
    }
}

#endif