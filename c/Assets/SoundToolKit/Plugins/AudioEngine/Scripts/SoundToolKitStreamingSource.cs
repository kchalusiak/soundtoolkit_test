///
/// \author Jan Wilczek
/// \date 02.08.2018
/// 

using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;

namespace SoundToolKit.Unity
{

    /// <summary>
    /// This component wraps Virtual SpatialSoundSource from SoundToolKit however only allows to play streaming samples.
    /// StkStreamingSource takes into account StkMeshes to generate sound.
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Transform))]
    [AddComponentMenu("SoundToolKit/SoundToolKitStreamingSource")]
    public sealed class SoundToolKitStreamingSource : SpatialComponent
    {
        #region public events

        public event Action<SoundToolKitStreamingSource> OnPaused;

        public event Action<SoundToolKitStreamingSource> OnStarted;

        public event Action<SoundToolKitStreamingSource> OnEnded;

        #endregion

        #region public properties

        public override bool IsPlaying
        {
            get { return Playback != null ? Playback.IsPlaying : false; }
        }

        #endregion

        #region public methods

        #region inherited methods
        public override void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                base.OnStkInitialized(soundToolKitManager);

                var resourcesFactory = SoundToolKitManager.Instance.StkAudioEngine.ResourcesFactory;

                m_streamingSample = resourcesFactory.CreateSampleStreaming(channelCount: STK_UNITY_STREAMING_SAMPLE_CHANNELS);

                if (m_spatialSoundSource != null)
                {
                    Playback = resourcesFactory.CreatePlayback(m_streamingSample, m_spatialSoundSource);
                }

                Initialized = true;
            }
        }

        [ContextMenu("Play")]
        public override void Play()
        {
            if (m_streamingSample != null)
            {
                Playback.Play();
            }
            else
            {
                SoundToolKitDebug.LogWarning("StkStreamingSource is not initialized in: " + gameObject.name);
            }
        }

        [ContextMenu("Stop")]
        public override void Stop()
        {
            if (m_streamingSample != null)
            {
                Playback.Stop();
            }
            else
            {
                SoundToolKitDebug.LogWarning("StkStreamingSource is not initialized in: " + gameObject.name);
            }
        }

        #endregion

        /// <summary>
        /// Streams (enqueues) interleaved samples.
        /// </summary>
        /// <param name="samplesToEnqueue">
        /// Interleaved, raw samples buffer.
        /// </param>
        public void EnqueueSamples(float[] samples)
        {
            m_streamingSample.EnqueueSamplesInterleaved(samples);
        }

        /// <summary>
        /// Streams (enqueues) samples.
        /// </summary>
        /// <param name="samplesToEnqueue">
        /// Buffer of samples.
        /// </param>
        public void EnqueueSamples(float[][] samples)
        {
            SoundToolKitDebug.Assert(samples.Length == STK_UNITY_STREAMING_SAMPLE_CHANNELS, "Invalid number of streamed channels.");

            m_streamingSample.EnqueueSamples(samples);
        }

        #endregion

        #region protected inherited methods

        protected override void Awake()
        {
            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        protected override void OnDisable()
        {
            if (Playback != null)
            {
                if (Playback.IsPlaying)
                {
                    Playback.Pause();
                    m_isPlayingPreviousState = true;
                }
                else
                {
                    m_isPlayingPreviousState = false;
                }
            }
        }

        protected override void OnEnable()
        {
            if (m_isPlayingPreviousState)
            {
                Playback.Play();
            }
        }

        protected override void OnDestroy()
        {
            if (Playback != null)
            {
                Playback.Dispose();
            }
            m_streamingSample.Dispose();

            base.OnDestroy();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif

        #endregion

        #region private methods

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

        #endregion

        #region private properties

        private Playback Playback
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

                        m_playback.Dispose();
                    }

                    m_playback = value;

                    if (m_playback != null)
                    {
                        m_playback.OnEnded += OnPlaybackEnded;
                        m_playback.OnPaused += OnPlaybackPaused;
                        m_playback.OnStarted += OnPlaybackStarted;
                    }
                }
            }
        }

        #endregion

        #region private members

        [NonSerialized]
        private SampleStreaming m_streamingSample;

        [NonSerialized]
        private Playback m_playback;

        private const int STK_UNITY_STREAMING_SAMPLE_CHANNELS = 1;

        #endregion
    }
}
