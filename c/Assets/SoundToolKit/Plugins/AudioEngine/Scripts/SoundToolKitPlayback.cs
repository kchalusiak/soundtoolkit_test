/// \author Michal Majewski
/// \date 15.11.2018

using UnityEngine;
using System;
using SoundToolKit.Utils;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Utilized by SoundSources, Playback consist of SoundToolKitSample to be played plus Looped and AutoPlay controls.
    /// </summary>
    [Serializable]
    public sealed class SoundToolKitPlayback
    {
        #region public constructors

        public SoundToolKitPlayback(SoundToolKitSample sample = null, float volume = 1.0f)
        {
            Awake();
            m_sample = sample;
            m_volume = volume;
            UpdateProperties();
        }

        #endregion

        #region editor fields

        [SerializeField]
        private SoundToolKitSample m_sample = null;

        [SerializeField]
        private float m_volume = 1.0f;

        [SerializeField]
        private bool m_looped = false;

        [SerializeField]
        private bool m_autoPlay = false;

        #endregion

        #region public events

        public event Action<SoundToolKitPlayback> OnPaused;

        public event Action<SoundToolKitPlayback> OnStarted;

        public event Action<SoundToolKitPlayback> OnEnded;

        #endregion

        #region public properties

        /// <summary>
        /// StkSample that is used to create this playback
        /// </summary>
        public SoundToolKitSample SoundToolKitSample
        {
            get { return m_sample; }
            set { m_sample = value; }
        }

        /// <summary>
        /// Controls the volume of a single Playback. Works together with the volume of a given SoundSource,
        /// e.g. if volume of Playback is 1.0f but volume of SoundSource that owns it is 0.0f, there will be no sound
        /// produced by that Playback emitted from the SoundSource.
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = value;

                if (Playback != null)
                {
                    Playback.Volume = m_volume;
                }
            }
        }

        /// <summary>
        /// Whether the playback is supposed to be looped
        /// </summary>
        public bool Looped
        {
            get { return Playback != null ? Playback.Parameters.IsLooped() : m_looped; }
            set
            {
                if (value != m_wasLooped)
                {
                    m_looped = value;
                    m_wasLooped = value;

                    if (Playback != null)
                    {
                        ReloadOnLoopingChanged();
                    }
                }

            }
        }

        /// <summary>
        /// Whether the Playback is supposed to play automatically when the scene is initialized or when there are 
        /// changes made to the Playback's editor fields
        /// </summary>
        public bool AutoPlay
        {
            get { return m_autoPlay; }
            set { m_autoPlay = value; }
        }

        public bool IsPlaying
        {
            get { return Playback != null ? Playback.IsPlaying : false; }
        }

        public string Name { get; set; }

        #endregion

        #region internal properties

        /// <summary>
        /// Playback object in STK that this Unity Playback represents
        /// </summary>
        internal Playback Playback
        {
            get { return m_playback; }
            set
            {
                if (value != Playback)
                {
                    if (m_playback != null)
                    {
                        m_playback.OnEnded -= OnPlaybackEnded;
                        m_playback.OnPaused -= OnPlaybackPaused;
                        m_playback.OnStarted -= OnPlaybackStarted;

                        SoundToolKitManager.Instance.ResourceContainer.Remove(playback: this);
                        m_playback.Dispose();
                    }

                    m_playback = value;

                    if (m_playback != null)
                    {
                        m_playback.OnEnded += OnPlaybackEnded;
                        m_playback.OnPaused += OnPlaybackPaused;
                        m_playback.OnStarted += OnPlaybackStarted;

                        SoundToolKitManager.Instance.ResourceContainer.Add(playback: this);
                        m_playback.Volume = Volume;
                    }
                }
            }
        }

        internal bool IsPlayingPreviousState { get; set; }

        #endregion

        #region internal methods

#if UNITY_EDITOR
        internal void OnValidate()
        {
            if (!m_awakened)
            {
                Awake();
            }

            UpdateProperties();
        }
#endif

        #endregion

        #region private methods

        private void ReloadOnLoopingChanged()
        {
            var source = Playback.Source;
            var resourcesFactory = SoundToolKitManager.Instance.StkAudioEngine.ResourcesFactory;
            var samplesBuffer = SoundToolKitSample.GetSamplesBuffer();

            if (m_looped)
            {
                Playback = resourcesFactory.CreatePlayback(resourcesFactory.CreateSampleStatic(samplesBuffer, SoundToolKitSample.AudioClip.channels),
                    Playback.Source, new PlaybackParameters(0, 1, PlaybackParameters.Looped));
            }
            else
            {
                Playback = resourcesFactory.CreatePlayback(resourcesFactory.CreateSampleStatic(samplesBuffer, SoundToolKitSample.AudioClip.channels), source);
            }

            if (AutoPlay)
            {
                Playback.Play();
            }
        }

        private void OnPlaybackEnded(Playback playback)
        {
            if (OnEnded != null)
            {
                OnEnded(this);
            }
        }

        private void OnPlaybackPaused(Playback playback)
        {
            if (OnPaused != null)
            {
                OnPaused(this);
            }
        }

        private void OnPlaybackStarted(Playback playback)
        {
            if (OnStarted != null)
            {
                OnStarted(this);
            }
        }

        private void UpdateProperties()
        {
            SoundToolKitSample = m_sample;
            Volume = m_volume;
            Looped = m_looped;
            AutoPlay = m_autoPlay;
        }

        private void Awake()
        {
            m_wasLooped = m_looped;
            m_volume = 1.0f;
            Name = "Not Initialized";
            m_awakened = true;
        }

        #endregion

        #region private members

        [NonSerialized]
        private Playback m_playback;

        [NonSerialized]
        private bool m_wasLooped;

        [SerializeField, HideInInspector]
        private bool m_awakened = false;

        #endregion
    }
}
