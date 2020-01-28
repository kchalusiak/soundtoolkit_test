/// \author Michal Majewski
/// \date 23.11.2018

#if UNITY_EDITOR

using UnityEditor;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitSourceSpawner))]
    [CanEditMultipleObjects]
    public class SoundToolKitSourceSpawnerEditor : SpatialComponentEditor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            DrawSamples();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sources' volume settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();


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

        #region private methods

        protected override void OnEnable()
        {
            base.OnEnable();
            EditorGUI.showMixedValue = true;

            m_samplesProp = serializedObject.FindProperty("m_samples");          
        }

        private void DrawSamples()
        {
            EditorGUILayout.PropertyField(m_samplesProp, true);
        }     

        #endregion

        #region private members     

        private SerializedProperty m_samplesProp;
      
        #endregion
    }
}

#endif