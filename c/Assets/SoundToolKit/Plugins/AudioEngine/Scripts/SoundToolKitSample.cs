using SoundToolKit.Unity.Utils;
using UnityEngine;
using System.Runtime.CompilerServices;

// For testing purposes
[assembly: InternalsVisibleTo("Soundtoolkit.Tests.PlayMode")]

namespace SoundToolKit.Unity
{
    /// <summary>
    /// This is wrapper for AudioClip designed to prevent long loading of AudioClips when user is already in game.
    /// This object ensures that Samples created as assets will be loaded only once.
    /// </summary>
    [CreateAssetMenu(menuName = "SoundToolKit/SoundToolKitSample")]
    public sealed class SoundToolKitSample : ScriptableObject
    {
        #region editor fields

        [SerializeField]
        private AudioClip m_audioClip = null;

        #endregion

        #region public properties

        /// <summary>
        /// Audio Clip which will be used by Stk.
        /// This clip sample rate should be 48000 Hz.
        /// </summary>
        public AudioClip AudioClip
        {
            get { return m_audioClip; }
            set
            {
                m_audioClip = value;
            }
        }

        #endregion

        #region internal methods

        internal float[] GetSamplesBuffer()
        {
            SoundToolKitDebug.Assert(m_audioClip != null, "AudioClip is missing.");

            if (m_samplesBuffer == null || m_samplesBuffer.Length != m_audioClip.samples * m_audioClip.channels)
            {
                m_samplesBuffer = ConvertToArrayOfSamples(m_audioClip);
            }

            return m_samplesBuffer;
        }

        #endregion

        #region private methods

        private float[] ConvertToArrayOfSamples(AudioClip audioClip)
        {
            var audioEngine = SoundToolKitManager.Instance.StkAudioEngine;
            var samples = new float[audioClip.samples * audioClip.channels];

            SoundToolKitDebug.Assert(samples.Length > 0, "Precondition not met. AudioClip doesn't have any samples.");

            audioClip.GetData(samples, 0);

            //Note: Current release 1.0 of AudioEngine doesn't support more channels than 1. 
            //      If we pass multi channeled sample only left channel will be used.

            if (audioClip.frequency != STK_REQUIRED_SAMPLE_RATE)
            {
                SoundToolKitDebug.LogError("Audio clip: " + audioClip.name + " has sample rate: " + audioClip.frequency + ". SoundToolKit requires: " + STK_REQUIRED_SAMPLE_RATE);
            }

            return samples;
        }

        #region unity methods

        private void Awake()
        {
            UpdateProperties();
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
            AudioClip = m_audioClip;
        }

        #endregion

        #region private enums

        private enum SampleType
        {
            Mono = 1,
            Stereo = 2
        }

        #endregion

        #region private members

        private const int STK_SUPPORTED_SAMPLES_CHANNELS = 1;

        private const int STK_REQUIRED_SAMPLE_RATE = 48000;

        private float[] m_samplesBuffer = null;

        #endregion
    }
}
