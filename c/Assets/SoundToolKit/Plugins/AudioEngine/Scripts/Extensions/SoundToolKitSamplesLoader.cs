/// \author Marcin Misiek
/// \date 10.10.2018

using System.Collections.Generic;
using UnityEngine;

namespace SoundToolKit.Unity.Extensions
{
    public class SoundToolKitSamplesLoader : MonoBehaviour, ISoundToolKitObserver
    {
        #region inspector members

        [SerializeField]
        private List<SoundToolKitSample> m_soundToolKitSamplesReadOnly = new List<SoundToolKitSample>();

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        public void Load(SoundToolKitSample soundToolKitSample)
        {
            if (Initialized)
            {
                LoadSample(soundToolKitSample);
            }
            else
            {
                m_soundToolKitSamples.Add(soundToolKitSample);
            }
        }

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                // Samples from inspector
                LoadSamples(m_soundToolKitSamplesReadOnly);

                // Samples from method Load
                LoadSamples(m_soundToolKitSamples);

                // No further need for storing those samples here
                m_soundToolKitSamples.Clear();

                Initialized = true;
            }
        }

        #endregion

        #region private methods

        private void Awake()
        {
            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void LoadSamples(List<SoundToolKitSample> soundToolKitSamples)
        {
            foreach (var stkSample in soundToolKitSamples)
            {
                LoadSample(stkSample);
            }
        }

        private void LoadSample(SoundToolKitSample soundToolKitSample)
        {
            if (soundToolKitSample != null)
            {
                soundToolKitSample.GetSamplesBuffer();
            }
        }

        #endregion

        #region private members

        private List<SoundToolKitSample> m_soundToolKitSamples = new List<SoundToolKitSample>();

        #endregion
    }
}
