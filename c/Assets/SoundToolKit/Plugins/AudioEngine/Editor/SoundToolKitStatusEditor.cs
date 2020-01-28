/// \author Marcin Misiek
/// \date 09.08.2018

#if UNITY_EDITOR

using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitStatus))]
    public class SoundToolKitStatusEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            var soundToolKitStatus = (SoundToolKitStatus)target;

            EditorGUIUtility.labelWidth = Screen.width / SCREEN_WIDTH_TO_LABEL_WIDTH_RATIO;

            var serializedStatusIndicators = soundToolKitStatus.SerializedStatusIndicators;
            var serializedLogs = soundToolKitStatus.SerializedLogs;

            if (serializedStatusIndicators.Count > 0 || serializedLogs.Count > 0)
            {
                m_statusIndicatorsFolded = EditorGUILayout.Foldout(m_statusIndicatorsFolded, "Status indicators");

                if (m_statusIndicatorsFolded)
                {
                    if (serializedStatusIndicators != null)
                    {
                        DisplayStatusIndicators(serializedStatusIndicators);
                    }
                }

                m_logsFolded = EditorGUILayout.Foldout(m_logsFolded, "Logs");

                if (m_logsFolded)
                {
                    if (serializedLogs != null)
                    {
                        DisplayLogs(serializedLogs);
                    }
                }

                m_requiresRepaint = true;
            }
            else
            {
                EditorGUILayout.HelpBox("SoundToolKit is not initialized.", MessageType.Info);
                m_requiresRepaint = false;
            }
        }

        #endregion

        #region public methods

        public override bool RequiresConstantRepaint()
        {
            return m_requiresRepaint;
        }

        #endregion

        #region private methods

        private void DisplayStatusIndicators(List<SerializedStatusIndicator> serializedStatusIndicators)
        {
            var comparisonDelegate = new Comparison<SerializedStatusIndicator>((x, y) => x.Name.CompareTo(y.Name));
            serializedStatusIndicators.Sort(comparisonDelegate);

            m_searchText = EditorGUILayout.TextField(new GUIContent("Search bar: "), m_searchText);
            EditorGUILayout.Space();

            var indicatorsFiltered = FilterStatusIndicators(serializedStatusIndicators, m_searchText);
            var renderedIndicators = new List<SerializedStatusIndicator>();

            foreach (var serializedIndicators in indicatorsFiltered)
            {
                if (!renderedIndicators.Contains(serializedIndicators))
                {
                    var domainPrefix = DomainBuilderHelper.GetDomainPrefix(serializedIndicators.Name, serializedIndicators.ShortName);
                    var domainTitle = DomainBuilderHelper.GetDomainTitle(domainPrefix);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(DomainBuilderHelper.ToTitleCase(domainTitle), EditorStyles.boldLabel);

                    var inDomainIndicators = indicatorsFiltered.Where(x =>
                    {
                        var inDomain = DomainBuilderHelper.InDomain(rootDomain: domainPrefix, other: DomainBuilderHelper.GetDomainPrefix(x.Name, x.ShortName));
                        var displayed = renderedIndicators.Contains(x);

                        return inDomain && !displayed;
                    });

                    foreach (var inDomainIndicator in inDomainIndicators)
                    {
                        EditorGUILayout.LabelField(
                            inDomainIndicator.ShortName,
                            inDomainIndicator.Value,
                            EditorStyles.wordWrappedLabel);

                        renderedIndicators.Add(inDomainIndicator);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void DisplayLogs(List<string> serializedLogs)
        {
            foreach (var log in serializedLogs)
            {
                EditorGUILayout.LabelField(log);
            }
        }

        private IEnumerable<SerializedStatusIndicator> FilterStatusIndicators(List<SerializedStatusIndicator> serializedStatusIndicators, string filterName)
        {
            if (!String.IsNullOrEmpty(filterName))
            {
                return serializedStatusIndicators.Where(x => x.Name.ToLower().Contains(filterName));
            }

            return serializedStatusIndicators;
        }

        #endregion

        #region private members

        private const float SCREEN_WIDTH_TO_LABEL_WIDTH_RATIO = 2.0f;

        private static bool m_logsFolded = true;

        private static bool m_statusIndicatorsFolded = true;

        private string m_searchText;

        private bool m_requiresRepaint;

        #endregion
    }
}

#endif