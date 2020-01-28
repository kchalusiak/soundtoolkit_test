using SoundToolKit.Unity.Utils;
using System;
/// \author Marcin Misiek
/// \date 22.06.2018

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// This class is a wrapper for SoundToolKit settings to allow serialization. Also there could be 
    /// multiple instances of this so this allows many configurations to be swapped at run time.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(menuName = "SoundToolKit/SoundToolKitSettings")]
    public class SoundToolKitSettings : ScriptableObject
    {
        #region editor fields

        /// <summary>
        /// List of serialized settings.
        /// When StkManager is present they are synchronized with StkSettings
        /// Those settings will override settings from engine on synchronization.
        /// </summary>
        [SerializeField]
        private List<SerializedSetting> m_serializedSettings = new List<SerializedSetting>();

        #endregion

        #region public properties

        /// <summary>
        /// Those settings allows interactions with SoundToolKit settings.
        /// </summary>
        public List<SerializedSetting> SerializedSettings
        {
            get { return m_serializedSettings; }
            set
            {
                m_serializedSettings = value;

                m_serializedSettings.ForEach(x => x.UpdateValue());
            }
        }

        #endregion

        #region internal properties

        internal ObservableCollection<Setting> StkSettings
        {
            get { return m_settings; }
            set
            {
                if (value != StkSettings)
                {
                    foreach (var setting in m_settings)
                    {
                        setting.Dispose();
                    }
                    m_settings = value;

                    SynchronizeSettings();
                }
            }
        }

        #endregion

        #region public methods

        #endregion

        #region private methods

        internal void UpdateSetting(Setting setting)
        {
            var serializedSetting = SerializedSettings.FirstOrDefault(x => x.Name == setting.Name);

            if (serializedSetting != null)
            {
                var newSetting = m_settings.FirstOrDefault(x => x.Name == serializedSetting.Name);

                serializedSetting.StkSetting = newSetting;
            }
            else
            {
                SerializedSettings.Add(new SerializedSetting(setting));
            }
        }

        internal void SynchronizeSettings()
        {
            if (m_serializedSettings.Count == 0)
            {
                SerializedSettings = CreateSerializedSettings();
            }
            else
            {
                UpdateProperties();

                ReSynchronizeSerializedSettings();
            }
        }

        private void InitializedSerializedSettings()
        {
            foreach (var serializedSetting in m_serializedSettings)
            {
                serializedSetting.Initialize();
            }
        }

        private void ReSynchronizeSerializedSettings()
        {
            foreach (var serializedSetting in m_serializedSettings)
            {
                var setting = m_settings.FirstOrDefault(x => x.Name == serializedSetting.Name);

                serializedSetting.StkSetting = setting;
            }
        }

        private List<SerializedSetting> CreateSerializedSettings()
        {
            var serializedSettings = new List<SerializedSetting>();

            foreach (var setting in m_settings)
            {
                var serializedSetting = new SerializedSetting(setting);

                serializedSettings.Add(serializedSetting);
            }

            return serializedSettings;
        }

        private bool MatchSettingsNames()
        {
            var stkSettingsName = new HashSet<string>(m_settings.Select(x => x.Name));
            var pluginSettingsName = new HashSet<string>(m_serializedSettings.Select(x => x.Name));

            return stkSettingsName.SetEquals(pluginSettingsName);
        }

        #region unity methods

        private void Awake()
        {
            InitializedSerializedSettings();
        }

        private void OnDestroy()
        {
            foreach (var setting in m_settings)
            {
                setting.Dispose();
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            UpdateProperties();
        }
#endif

        #endregion

        private void UpdateProperties()
        {
            SerializedSettings = m_serializedSettings;
        }

        #endregion

        #region private members

        private ObservableCollection<Setting> m_settings = new ObservableCollection<Setting>();

        #endregion
    }
}
