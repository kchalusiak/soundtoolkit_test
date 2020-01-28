/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 02.04.2019
///
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitGeometry))]
    public class SoundToolKitGeometryEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_meshClustersProp, true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var buttonGuiContent = new GUIContent("Serialize Geometry", "Serializes all of the MeshClusters currently " +
                "contained in this SoundToolKitGeometry. \n\n" +
                "Serializing the geometry enables restoring the acoustic scene on scene load instead of creating it from " +
                "scratch, greatly increasing the speed of SoundToolKit scene initialization. \n\n" +
                "If the mesh content of a given serialized MeshCluster changes, it's serialized data becomes " +
                "incompatible and that MeshCluster must be serialized again. \n\n" +
                "It is advised that valid serialization is performed at least once before building the game.");

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button(buttonGuiContent))
                {
                    var geometry = (SoundToolKitGeometry)target;
                    geometry.SerializeGeometry();
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(buttonGuiContent);
                GUI.enabled = true;

                EditorGUILayout.HelpBox("Geometry Serialization available only in PlayMode", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }


        #endregion

        #region private methods

        private void OnEnable()
        {
            m_meshClustersProp = serializedObject.FindProperty("m_meshClusters");
        }

        #endregion

        #region private members

        private SerializedProperty m_meshClustersProp;

        #endregion
    }
}

#endif