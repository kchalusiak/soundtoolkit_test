/// \author Michal Majewski
/// \date 19.04.2019

using UnityEngine;
using System;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Binds together SoundToolKitSample and Volume control for Playbacks created from it.
    /// Utilized by SourceSpawner component.
    /// </summary>
    [Serializable]
    public sealed class VolumeControlledSample
    {
        #region public constructors

        public VolumeControlledSample(SoundToolKitSample sample, float volume = 1.0f)
        {
            m_sample = sample;
            m_volume = volume;
            m_awakened = true;
        }

        #endregion

        #region editor fields

        [SerializeField]
        private SoundToolKitSample m_sample = null;

        [SerializeField]
        private float m_volume = 1.0f;

        #endregion

        #region public properties

        /// <summary>
        /// StkSample that will be used to create a StkPlayback.
        /// </summary>
        public SoundToolKitSample Sample
        {
            get { return m_sample; }
            set { m_sample = value; }
        }

        /// <summary>
        /// Controls the volume of a StkPlayback created from this StkSample.
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set { m_volume = value; }
        }

        #endregion

        #region internal methods

        internal void OnValidate()
        {
            if (!m_awakened)
            {
                m_volume = 1.0f;
                m_awakened = true;
            }
        }

        #endregion

        #region private members

        [SerializeField, HideInInspector]
        private bool m_awakened = false;

        #endregion
    }
}
