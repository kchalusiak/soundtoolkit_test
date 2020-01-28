/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 14.05.2019
///
#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitMesh))]
    [CanEditMultipleObjects]
    public class SoundToolKitMeshEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_acousticMaterialProp);
            EditorGUILayout.PropertyField(m_enabledAsAcousticObstacleProp);
            EditorGUILayout.PropertyField(m_meshClusterProp);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region private methods

        private void OnEnable()
        {
            m_acousticMaterialProp = serializedObject.FindProperty("m_acousticMaterial");
            m_enabledAsAcousticObstacleProp = serializedObject.FindProperty("m_enabledAsAcousticObstacle");
            m_meshClusterProp = serializedObject.FindProperty("m_meshCluster");

            m_targetName = target.name;
            m_targetCluster = ((SoundToolKitMesh)target).MeshCluster;
        }

        private void OnDestroy()
        {
            if (!EditorApplication.isPlaying && target == null && m_targetCluster != null)
            {
                m_targetCluster.Remove(m_targetName);
            }
        }

        #endregion

        #region private members

        private SerializedProperty m_acousticMaterialProp;
        private SerializedProperty m_enabledAsAcousticObstacleProp;
        private SerializedProperty m_meshClusterProp;

        private string m_targetName;
        private SoundToolKitMeshCluster m_targetCluster;

        #endregion
    }
}

#endif