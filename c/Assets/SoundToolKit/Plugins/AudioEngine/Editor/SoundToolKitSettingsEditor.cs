/// \author Marcin Misiek
/// \date 14.08.2018

#if UNITY_EDITOR

using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Editor
{
    [CustomEditor(inspectedType: typeof(SoundToolKitSettings))]
    public class SoundToolKitSettingsEditor : UnityEditor.Editor
    {
        #region public methods

        public override void OnInspectorGUI()
        {
            var soundToolKitSettings = (SoundToolKitSettings)target;
            var serializedSettings = soundToolKitSettings.SerializedSettings;

            EditorGUIUtility.labelWidth = Screen.width / SCREEN_WIDTH_TO_LABEL_WIDTH_RATIO;

            if (serializedSettings.Count > 0)
            {
                var comparisonDelegate = new Comparison<SerializedSetting>((x, y) => x.Name.CompareTo(y.Name));
                serializedSettings.Sort(comparisonDelegate);

                m_searchText = EditorGUILayout.TextField(new GUIContent("Search bar: "), m_searchText);
                EditorGUILayout.Space();

                var settingsDataFiltered = FilterSettings(serializedSettings, m_searchText);
                DisplaySettings(settingsDataFiltered);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                if (GUILayout.Button("Save settings"))
                {
                    EditorUtility.SetDirty(soundToolKitSettings);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Restore default settings"))
                {
                    serializedSettings.ForEach(x => x.CurrentValue = x.DefaultValue);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Place new settings in SoundToolKitManager to initialize them", MessageType.Info);
            }
        }

        #endregion

        #region private methods

        private void DisplaySettings(IEnumerable<SerializedSetting> settingsData)
        {
            var renderedSettings = new List<SerializedSetting>();

            foreach (var serializedSetting in settingsData)
            {
                if (!renderedSettings.Contains(serializedSetting))
                {
                    var domainPrefix = DomainBuilderHelper.GetDomainPrefix(serializedSetting.Name, serializedSetting.ShortName);
                    var domainTitle = DomainBuilderHelper.GetDomainTitle(domainPrefix);

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(DomainBuilderHelper.ToTitleCase(domainTitle), EditorStyles.boldLabel);

                    var inDomainSettings = settingsData.Where(x =>
                    {
                        var inDomain = DomainBuilderHelper.InDomain(rootDomain: domainPrefix, other: DomainBuilderHelper.GetDomainPrefix(x.Name, x.ShortName));
                        var displayed = renderedSettings.Contains(x);

                        return inDomain && !displayed;
                    });

                    foreach (var inDomainSetting in inDomainSettings)
                    {
                        DisplaySetting(inDomainSetting);

                        renderedSettings.Add(inDomainSetting);
                    }
                }
            }
        }

        private void DisplaySetting(SerializedSetting serializedSetting)
        {
            EditorGUILayout.Space();

            switch (serializedSetting.Type)
            {
                case SettingType.Boolean:
                    DisplayBoolToggle(serializedSetting);
                    break;

                case SettingType.Enumeration:
                    DisplayLabelField(serializedSetting);
                    break;

                case SettingType.Integer:
                    DisplayIntSlider(serializedSetting);
                    break;

                case SettingType.Float:
                    DisplayFloatSlider(serializedSetting);
                    break;

                case SettingType.ListOfIntegers:
                    DisplayList(serializedSetting);
                    break;

                case SettingType.ListOfFloats:
                    DisplayList(serializedSetting);
                    break;

                default:
                    DisplayTextField(serializedSetting);
                    break;
            }
        }

        private IEnumerable<SerializedSetting> FilterSettings(List<SerializedSetting> settingsData, string filterName)
        {
            if (!String.IsNullOrEmpty(filterName))
            {
                return settingsData.Where(x => x.Name.ToLower().Contains(filterName));
            }

            return settingsData;
        }

        private void ParseRangeAllowedValues<T>(string allowedValues, out T minimumValue, out T maximumValue) where T : IConvertible
        {
            var dividerIndex = allowedValues.IndexOf(",", StringComparison.InvariantCulture);

            var minimumValueString = allowedValues.Substring(1, dividerIndex - 1);
            var maximumValueString = allowedValues.Substring(dividerIndex + 1, allowedValues.Length - dividerIndex - 2);

            minimumValue = (T)Convert.ChangeType(minimumValueString, typeof(T));
            maximumValue = (T)Convert.ChangeType(maximumValueString, typeof(T));
        }

        private void DisplayText(SerializedSetting serializedSetting)
        {
            if (ContainsAdmissibleValues(serializedSetting))
            {
                DisplayLabelField(serializedSetting);
            }
            else
            {
                DisplayTextField(serializedSetting);
            }
        }

        private void DisplayList(SerializedSetting serializedSetting)
        {
            var dataArray = serializedSetting.CurrentValue.Trim().Split(' ');

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(
                new GUIContent(serializedSetting.ShortName,
                serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description));

            switch (serializedSetting.Type)
            {
                case SettingType.ListOfFloats:
                    {
                        for (var dataIndex = 0; dataIndex < dataArray.Length; dataIndex++)
                        {
                            float number = float.Parse(dataArray[dataIndex], CultureInfo.InvariantCulture);
                            dataArray[dataIndex] = Convert.ToString(EditorGUILayout.FloatField(number));
                        }
                        break;
                    }

                case SettingType.ListOfIntegers:
                    {
                        for (var dataIndex = 0; dataIndex < dataArray.Length; dataIndex++)
                        {
                            int number = int.Parse(dataArray[dataIndex], CultureInfo.InvariantCulture);
                            dataArray[dataIndex] = Convert.ToString(EditorGUILayout.IntField(number));
                        }
                        break;
                    }
            }

            EditorGUILayout.EndHorizontal();
            serializedSetting.CurrentValue = String.Join(" ", dataArray);
        }

        private void DisplayIntSlider(SerializedSetting serializedSetting)
        {
            int minimumValue;
            int maximumValue;
            ParseRangeAllowedValues(serializedSetting.AllowedValues, out minimumValue, out maximumValue);

            serializedSetting.CurrentValue = EditorGUILayout.IntSlider(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                Convert.ToInt32(serializedSetting.CurrentValue), minimumValue, maximumValue).ToString();
        }

        private void DisplayUIntSlider(SerializedSetting serializedSetting)
        {
            int minimumValue;
            int maximumValue;
            ParseRangeAllowedValues(serializedSetting.AllowedValues, out minimumValue, out maximumValue);

            serializedSetting.CurrentValue = EditorGUILayout.IntSlider(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                (int)Convert.ToUInt32(serializedSetting.CurrentValue),
                minimumValue, maximumValue).ToString();
        }

        private void DisplayFloatSlider(SerializedSetting serializedSetting)
        {
            float minimumValue;
            float maximumValue;
            ParseRangeAllowedValues(serializedSetting.AllowedValues, out minimumValue, out maximumValue);

            serializedSetting.CurrentValue = EditorGUILayout.Slider(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                Convert.ToSingle(serializedSetting.CurrentValue), minimumValue, maximumValue).ToString();
        }

        private void DisplayBoolToggle(SerializedSetting serializedSetting)
        {
            serializedSetting.CurrentValue = EditorGUILayout.Toggle(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                StringToBool(serializedSetting.CurrentValue)).ToString();
        }

        private void DisplayTextField(SerializedSetting serializedSetting)
        {
            serializedSetting.CurrentValue = EditorGUILayout.TextField(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                serializedSetting.CurrentValue);
        }

        private void DisplayLabelField(SerializedSetting serializedSetting)
        {
            EditorGUILayout.LabelField(
                new GUIContent(
                    serializedSetting.ShortName,
                    serializedSetting.Name + TOOLTIP_SEPERATOR + serializedSetting.Description),
                new GUIContent(serializedSetting.CurrentValue));
        }

        private bool ContainsAdmissibleValues(SerializedSetting serializedSetting)
        {
            return String.IsNullOrEmpty(serializedSetting.AllowedValues);
        }

        private bool StringToBool(string boolString)
        {
            bool boolValue;
            if (boolString.Equals("1") || boolString.Equals("0"))
            {
                boolValue = boolString.Equals("1") ? true : false;
            }
            else
            {
                boolValue = Convert.ToBoolean(boolString);
            }

            return boolValue;
        }

        #endregion

        #region private members

        private const string TOOLTIP_SEPERATOR = "\n\n";

        private const float SCREEN_WIDTH_TO_LABEL_WIDTH_RATIO = 3.0f;

        private string m_searchText;

        #endregion
    }
}

#endif