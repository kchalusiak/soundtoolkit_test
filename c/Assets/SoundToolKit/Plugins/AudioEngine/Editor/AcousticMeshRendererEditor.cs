/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 19.06.2019
///
#if UNITY_EDITOR

using SoundToolKit.Unity.Extensions;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(AcousticMeshRenderer))]
    public class AcousticMeshRendererEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_renderAcousticMeshesProp);
            EditorGUILayout.PropertyField(m_hideVisualMeshesProp);

            EditorGUILayout.Space();

            if (EditorApplication.isPlaying && m_renderAcousticMeshesProp.boolValue)
            {
                GUI.enabled = false;

                EditorGUILayout.PropertyField(m_meshClusterCountProp);
                EditorGUILayout.PropertyField(m_meshCountProp);
                EditorGUILayout.PropertyField(m_triangleCountProp);

                GUI.enabled = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region private methods

        private void OnEnable()
        {
            m_renderAcousticMeshesProp = serializedObject.FindProperty("m_renderAcousticMeshes");
            m_hideVisualMeshesProp = serializedObject.FindProperty("m_hideVisualMeshes");

            m_meshClusterCountProp = serializedObject.FindProperty("m_meshClusterCount");
            m_meshCountProp = serializedObject.FindProperty("m_meshCount");
            m_triangleCountProp = serializedObject.FindProperty("m_triangleCount");
        }

        #endregion

        #region private members

        private SerializedProperty m_renderAcousticMeshesProp;
        private SerializedProperty m_hideVisualMeshesProp;

        private SerializedProperty m_meshClusterCountProp;
        private SerializedProperty m_meshCountProp;
        private SerializedProperty m_triangleCountProp;

        #endregion
    }
}

#endif
