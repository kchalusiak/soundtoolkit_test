/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 04.03.2019
///
#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitMeshCluster))]
    public class SoundToolKitMeshClusterEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Contains " + m_meshesProp.arraySize.ToString() +
                " SoundToolKit Meshes:", EditorStyles.boldLabel);

            EditorGUI.indentLevel += 1;

            for (int i = 0; i < m_meshesProp.arraySize; i++)
            {
                var meshName = m_meshesProp.GetArrayElementAtIndex(i).stringValue;
                if (meshName != null)
                {
                    EditorGUILayout.LabelField(meshName);
                }
            }

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var cluster = target as SoundToolKitMeshCluster;
            if (cluster.Serialized)
            {
                EditorGUILayout.HelpBox("The geometry of this MeshCluster has been serialized correctly " +
                    "and can be restored when applicable.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("The geometry of this MeshCluster has not been serialized yet " +
                    "or was invalidated by mutating the contents of this MeshCluster and requires " +
                    "serialization in order to enable geometry restoring.", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region private methods

        private void OnEnable()
        {
            m_meshesProp = serializedObject.FindProperty("m_meshes");
        }

        #endregion

        #region private members

        private SerializedProperty m_meshesProp;

        #endregion
    }
}

#endif