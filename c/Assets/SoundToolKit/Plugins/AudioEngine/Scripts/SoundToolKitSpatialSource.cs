/// \author Marcin Misiek
/// \date 26.04.2018
/// \author Michal Majewski
/// \date 16.11.2018 - 10.03.2019

using SoundToolKit.Unity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// SoundToolKit Spatial Source, capable of playing multiple SoundToolKit Samples with various Play methods.
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Transform))]
    [AddComponentMenu("SoundToolKit/SoundToolKitSpatialSource")]
    public sealed class SoundToolKitSpatialSource : SpatialComponent
    {
        #region editor fields

        [SerializeField]
        private List<SoundToolKitPlayback> m_soundToolKitPlaybacks;

        #endregion

        #region public properties

        public override bool IsPlaying
        {
            get { return m_playbackComponent.IsPlaying(Playbacks); }
        }

        /// <summary>
        /// List of Playbacks, which consist of StkSample, Looped control, PlayOnAwake control and internal StkPlayback
        /// </summary>
        public List<SoundToolKitPlayback> Playbacks
        {
            get { return m_soundToolKitPlaybacks; }
            set
            {
                m_soundToolKitPlaybacks = value;

                if (m_spatialSoundSource != null)
                {
                    m_playbackComponent.InitializeInternalPlaybacks(Playbacks);
                }
            }
        }

        #endregion

        #region public methods

        public override void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                base.OnStkInitialized(soundToolKitManager);

                m_playbackComponent = new PlaybackComponent(m_spatialSoundSource);

                m_playbackComponent.InitializeInternalPlaybacks(Playbacks);
                m_playbackComponent.PlayOnAwake(Playbacks, gameObject.activeSelf, isActiveAndEnabled);

                Initialized = true;
            }
        }

        /// <summary>
        /// Play all the Playbacks on the Source's list.
        /// </summary>
        [ContextMenu("Play")]
        public override void Play()
        {
            m_playbackComponent.Play(Playbacks);
        }

        /// <summary>
        /// Stops all incoming sounds from that Source.
        /// </summary>
        [ContextMenu("Stop")]
        public override void Stop()
        {
            m_playbackComponent.Stop(Playbacks);
        }

        /// <summary>
        /// Pauses all incoming sounds from that Source.
        /// </summary>
        [ContextMenu("Pause")]
        public void Pause()
        {
            m_playbackComponent.Pause(Playbacks);
        }

        /// <summary>
        /// Play a single Playback from the Source's Playback list.
        /// </summary>
        /// <param name="playbackToPlayIndex"> 
        /// Index of Playback to play by the Source. Defaults to the first Playback on the list.
        /// </param>
        public void PlaySingle(int playbackToPlayIndex = 0)
        {
            m_playbackComponent.PlaySingle(Playbacks, playbackToPlayIndex);
        }

        /// <summary>
        /// Play a specified range of Playbacks from Source's list.
        /// </summary>
        public void PlayRange(int firstPlaybackToPlayIndex, int lastPlaybackToPlayIndex)
        {
            m_playbackComponent.PlayRange(Playbacks, firstPlaybackToPlayIndex, lastPlaybackToPlayIndex);
        }

        /// <summary>
        /// Play a random Playback from the specified range on the Source's Playback List (whole List by default).
        /// </summary>
        /// <param name="firstPlaybackToRandomFromIndex">
        /// Index of first Playback to randomize from. Defaults to the first Playback on the list.
        /// </param>
        /// <param name="lastPlaybackToRandomFromIndex">
        /// Index of last Playback to randomize from. Defaults to the last Playback on the list.
        /// </param>
        public void PlayRandom(int firstPlaybackToRandomFromIndex = 0, int? lastPlaybackToRandomFromIndex = null)
        {
            m_playbackComponent.PlayRandom(Playbacks, firstPlaybackToRandomFromIndex, lastPlaybackToRandomFromIndex);
        }

        /// <summary>
        /// Stop a single Playback from the Source's Playback list.
        /// </summary>
        /// <param name="playbackToStopIndex"> 
        /// Index of Playback to stop. Defaults to the first Playback on the list.
        /// </param>
        public void StopSingle(int playbackToStopIndex = 0)
        {
            m_playbackComponent.StopSingle(Playbacks, playbackToStopIndex);
        }

        /// <summary>
        /// Stop a specified range of Playbacks from Source's list.
        /// </summary>
        public void StopRange(int firstPlaybackToStopIndex, int lastPlaybackToStopIndex)
        {
            m_playbackComponent.StopRange(Playbacks, firstPlaybackToStopIndex, lastPlaybackToStopIndex);
        }

        /// <summary>
        /// Pause a single Playback from the Source's Playback list.
        /// </summary>
        /// <param name="playbackToPauseIndex"> 
        /// Index of Playback to pause. Defaults to the first Playback on the list.
        /// </param>
        public void PauseSingle(int playbackToPauseIndex = 0)
        {
            m_playbackComponent.PauseSingle(Playbacks, playbackToPauseIndex);
        }

        /// <summary>
        /// Pause a specified range of Playbacks from Source's list. Pauses all by default.
        /// </summary>
        public void PauseRange(int firstPlaybackToPauseIndex, int lastPlaybackToPauseIndex)
        {
            m_playbackComponent.PauseRange(Playbacks, firstPlaybackToPauseIndex, lastPlaybackToPauseIndex);
        }

        #endregion

        #region unity methods

        protected override void Awake()
        {
            if (Playbacks == null)
            {
                Playbacks = new List<SoundToolKitPlayback>();
            }

            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        protected override void OnDestroy()
        {
            Playbacks.ForEach(x =>
            {
                x.Playback = null;
            });

            Playbacks.Clear();

            base.OnDestroy();
        }

        protected override void OnDisable()
        {
            Playbacks.ForEach(x =>
            {
                if (x.Playback != null)
                {
                    x.Playback.Pause();
                    x.IsPlayingPreviousState = true;
                }
                else
                {
                    x.IsPlayingPreviousState = false;
                }
            });
        }

        protected override void OnEnable()
        {
            Playbacks.ForEach(x =>
            {
                if (x.Playback != null && x.IsPlayingPreviousState)
                {
                    x.Playback.Play();
                }
            });
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            // <STK.LITE>
            // Warning: Modifying this code may result in plugin instability and frequent crashes to desktop.
            // This is the SoundToolKit Lite version.
            // Internally only one Playback per source is created anyway, so circumventing this check will not result in a possibility
            // of creating more playbacks - the previous Playback will be overwritten.
            if (Playbacks != null && Playbacks.Count > 1)
            {
                Playbacks.RemoveRange(1, Playbacks.Count - 1);
                SoundToolKitDebug.Log("Only one Playback per Sound Source is available in the SoundToolKit Lite version. " +
                    "Deleting illegal playbacks.");
                return;
            }
            // </STK.LITE>

            base.OnValidate();

            if (Playbacks != null)
            {
                Playbacks.ForEach(x =>
                {
                    x.OnValidate();
                    if (m_playbackComponent != null)
                    {
                        m_playbackComponent.SynchronizePlaybackSamples(x);
                    }
                });
            }
        }
#endif

        #endregion

        #region private members

        private PlaybackComponent m_playbackComponent;

        #endregion
    }
}