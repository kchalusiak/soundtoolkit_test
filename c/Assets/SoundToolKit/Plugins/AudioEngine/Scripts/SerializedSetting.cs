/// \author Marcin Misiek
/// \date 13.07.2018

using System;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// This parameter is a wrapper for StkSetting.
    /// It's purpose is to serialize settings even if there is no StkManager.
    /// </summary>
    [Serializable]
    public class SerializedSetting
    {
        #region editor fields

        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_currentValue;

        [SerializeField, HideInInspector]
        private string m_defaultValue;

        [SerializeField]
        private string m_description;

        [SerializeField, HideInInspector]
        private SettingType m_type;

        [SerializeField, HideInInspector]
        private string m_allowedValues;

        [SerializeField, HideInInspector]
        private string m_shortName;

        #endregion

        #region public properties

        public string Name
        {
            get { return m_name; }
            private set { m_name = value; }
        }

        public string Description
        {
            get { return m_description; }
            private set { m_description = value; }
        }

        public string DefaultValue
        {
            get { return m_defaultValue; }
            private set { m_defaultValue = value; }
        }

        public string ShortName
        {
            get { return m_shortName; }
            private set { m_shortName = value; }
        }

        public string AllowedValues
        {
            get { return m_allowedValues; }
            private set { m_allowedValues = value; }
        }

        public SettingType Type
        {
            get { return m_type; }
            private set { m_type = value; }
        }

        public string CurrentValue
        {
            get { return m_setting != null && m_setting.Valid ? m_setting.CurrentValue : m_currentValue; }
            set
            {
                m_currentValue = value;

                if (m_setting != null && m_setting.Valid)
                {
                    m_setting.CurrentValue = m_currentValue;
                }
            }
        }

        #endregion

        #region public methods

        public void Initialize()
        {
            CurrentValue = m_currentValue;
            Name = m_name;
            Description = m_description;
            DefaultValue = m_defaultValue;
            Type = m_type;
            AllowedValues = m_allowedValues;
            ShortName = m_shortName;
        }

        public void UpdateValue()
        {
            CurrentValue = m_currentValue;
        }

        #endregion

        #region internal constructor

        internal SerializedSetting(Setting setting)
        {
            m_setting = setting;

            CurrentValue = m_setting.CurrentValue;
            Name = m_setting.Name;
            Description = m_setting.Description;
            DefaultValue = m_setting.DefaultValue;
            Type = m_setting.Type;
            AllowedValues = m_setting.AllowedValues;
            ShortName = m_setting.Name.Split('.').Last();
        }

        #endregion

        #region internal properties

        internal Setting StkSetting
        {
            get { return m_setting; }
            set
            {
                if (value != StkSetting)
                {
                    m_setting = value;
                    m_setting.CurrentValue = m_currentValue;
                }
            }
        }

        #endregion

        #region private members

        private Setting m_setting;

        #endregion
    }
}
