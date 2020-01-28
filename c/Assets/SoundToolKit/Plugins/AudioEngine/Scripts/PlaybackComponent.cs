/// \author Michal Majewski
/// \date 08.03.2019

using System.Linq;
using System.Collections.Generic;
using SoundToolKit.Unity.Utils;
using SoundToolKit.Utils;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Class that contains methods controlling SoundToolKitPlaybacks, common for Spatial and Ambient Sound Sources.
    /// </summary>
    public class PlaybackComponent
    {
        #region internal constructor

        internal PlaybackComponent(Source stkSource)
        {
            m_source = stkSource;
        }

        #endregion

        #region internal methods

        internal bool IsPlaying(List<SoundToolKitPlayback> playbacks)
        {
            return playbacks.Exists(x => x.IsPlaying);
        }

        internal void Play(List<SoundToolKitPlayback> playbacks)
        {
            playbacks.ForEach(x =>
            {
                StartPlayback(x);
            });
        }

        internal void Stop(List<SoundToolKitPlayback> playbacks)
        {
            playbacks.ForEach(x =>
            {
                if (x.Playback != null)
                {
                    x.Playback.Stop();
                }
            });
        }

        internal void Pause(List<SoundToolKitPlayback> playbacks)
        {
            playbacks.ForEach(x =>
            {
                if (x.Playback != null)
                {
                    x.Playback.Pause();
                }
            });
        }

        internal void PlaySingle(List<SoundToolKitPlayback> playbacks, int playbackToPlayIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                StartPlayback(playbacks[ClampIndex(playbackToPlayIndex, playbacks.Count)]);
            }
        }

        internal void PlayRange(List<SoundToolKitPlayback> playbacks, int firstPlaybackToPlayIndex, int lastPlaybackToPlayIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                var firstToPlayIndex = ClampIndex(firstPlaybackToPlayIndex, playbacks.Count);
                var lastToPlayIndex = ClampIndex(lastPlaybackToPlayIndex, playbacks.Count);
                FixIndexOrder(ref firstToPlayIndex, ref lastToPlayIndex);

                for (int i = firstToPlayIndex; i <= lastToPlayIndex; i++)
                {
                    StartPlayback(playbacks[i]);
                }
            }
        }

        internal void PlayRandom(List<SoundToolKitPlayback> playbacks, int firstPlaybackToRandomFromIndex, int? lastPlaybackToRandomFromIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                var firstToRandIndex = ClampIndex(firstPlaybackToRandomFromIndex, playbacks.Count);
                var lastToRandIndex = ClampIndex(lastPlaybackToRandomFromIndex ?? playbacks.Count - 1, playbacks.Count);
                FixIndexOrder(ref firstToRandIndex, ref lastToRandIndex);

                var randomIndex = UnityEngine.Random.Range(firstToRandIndex, lastToRandIndex);
                StartPlayback(playbacks[randomIndex]);
            }
        }

        internal void StopSingle(List<SoundToolKitPlayback> playbacks, int playbackToStopIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                playbacks[ClampIndex(playbackToStopIndex, playbacks.Count)].Playback.Stop();
            }
        }

        internal void StopRange(List<SoundToolKitPlayback> playbacks, int firstPlaybackToStopIndex, int lastPlaybackToStopIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                var firstToStopIndex = ClampIndex(firstPlaybackToStopIndex, playbacks.Count);
                var lastToStopIndex = ClampIndex(lastPlaybackToStopIndex, playbacks.Count);
                FixIndexOrder(ref firstToStopIndex, ref lastToStopIndex);

                for (int i = firstToStopIndex; i <= lastToStopIndex; i++)
                {
                    playbacks[i].Playback.Stop();
                }
            }
        }

        internal void PauseSingle(List<SoundToolKitPlayback> playbacks, int playbackToPauseIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                playbacks[ClampIndex(playbackToPauseIndex, playbacks.Count)].Playback.Pause();
            }
        }

        internal void PauseRange(List<SoundToolKitPlayback> playbacks, int firstPlaybackToPauseIndex, int lastPlaybackToPauseIndex)
        {
            if (PlaybackListNotEmpty(playbacks))
            {
                var firstToPauseIndex = ClampIndex(firstPlaybackToPauseIndex, playbacks.Count);
                var lastToPauseIndex = ClampIndex(lastPlaybackToPauseIndex, playbacks.Count);
                FixIndexOrder(ref firstToPauseIndex, ref lastToPauseIndex);

                for (int i = firstToPauseIndex; i <= lastToPauseIndex; i++)
                {
                    playbacks[i].Playback.Pause();
                }
            }
        }

        internal void PlayOnAwake(List<SoundToolKitPlayback> playbacks, bool gameObjectActive, bool componentActive)
        {
            playbacks.ForEach(x =>
            {
                if (gameObjectActive && componentActive)
                {
                    if (x.AutoPlay && x.Playback != null)
                    {
                        x.Playback.Play();
                    }
                }
            });
        }

        internal void StartPlayback(SoundToolKitPlayback playback)
        {
            if (playback.Playback == null)
            {
                CreateInternalPlayback(playback);
            }

            if (!playback.IsPlaying)
            {
                playback.Playback.Play();
            }
        }

        internal void InitializeInternalPlaybacks(List<SoundToolKitPlayback> playbacks)
        {
            playbacks.ForEach(x =>
            {
                if (x.Playback == null)
                {
                    CreateInternalPlayback(x);
                }
            });
        }

        internal void CreateInternalPlayback(SoundToolKitPlayback playback)
        {
            if (playback.SoundToolKitSample == null)
            {
                return;
            }

            if (m_source != null)
            {
                var resourcesFactory = SoundToolKitManager.Instance.StkAudioEngine.ResourcesFactory;
                var samplesBuffer = playback.SoundToolKitSample.GetSamplesBuffer();

            SoundToolKitDebug.Assert(samplesBuffer != null, "SamplesBuffer is missing. Logical error.");
            SoundToolKitDebug.Assert(samplesBuffer.Length > 0, "SamplesBuffer doesn't have samples.");

                var playbackSample = resourcesFactory.CreateSampleStatic(samplesBuffer, playback.SoundToolKitSample.AudioClip.channels);

                if (playback.Looped)
                {
                    playback.Playback = resourcesFactory.CreatePlayback(playbackSample, m_source, new PlaybackParameters(0, 1, PlaybackParameters.Looped));
                }
                else
                {
                    playback.Playback = resourcesFactory.CreatePlayback(playbackSample, m_source);
                }

                playback.Playback.OnDispose += (sender, e) =>
                {
                    playbackSample.Dispose();
                };

                playback.Name = playback.SoundToolKitSample.name;
            }
        }

        internal void SynchronizePlaybackSamples(SoundToolKitPlayback playback)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying && SoundToolKitManager.Instance.Initialized)
            {
                if (playback.Playback != null && playback.SoundToolKitSample == null)
                {
                    playback.Playback = null;
                    playback.Name = "Not Initialized";
                }
                else if (playback.SoundToolKitSample != null && playback.Name != playback.SoundToolKitSample.name)
                {
                    CreateInternalPlayback(playback);

                    if (playback.Playback != null && playback.AutoPlay)
                    {
                        playback.Playback.Play();
                    }
                }
            }
#endif
        }

        #endregion

        #region private methods

        private int ClampIndex(int playbackIndex, int playbacksCount)
        {
            if (playbackIndex >= playbacksCount)
            {
                playbackIndex = playbacksCount - 1;
                SoundToolKitDebug.LogWarning("Index of the chosen Playback is too large. Clamping to the last element on the list.");
            }
            else if (playbackIndex < 0)
            {
                playbackIndex = 0;
                SoundToolKitDebug.LogWarning("Index of the chosen Playback is too small. Clamping to the first element on the list.");
            }

            return playbackIndex;
        }

        private void FixIndexOrder(ref int firstObjectIndex, ref int lastObjectIndex)
        {
            if (firstObjectIndex > lastObjectIndex)
            {
                var tempIndex = firstObjectIndex;
                firstObjectIndex = lastObjectIndex;
                lastObjectIndex = tempIndex;
            }
        }

        private bool PlaybackListNotEmpty(List<SoundToolKitPlayback> playbacks)
        {
            if (playbacks.Any())
            {
                return true;
            }
            else
            {
                SoundToolKitDebug.LogWarning("Playback List for SoundToolKitspatialSource is empty. No Playback to Play.");
                return false;
            }
        }

        #endregion

        #region private members

        private Source m_source;

        #endregion

    }
}
