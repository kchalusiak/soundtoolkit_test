using SoundToolKit.Extensions.AI;
using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SoundToolKit.Unity
{

    /// <summary>
    /// This component performs scene analysis using AI methods. It combines methods for analyzing sound paths energy.
    /// </summary>
    [Serializable]
    [AddComponentMenu("SoundToolKit/SoundToolKitAI")]
    public sealed class SoundToolKitAI : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private bool m_analysisEnabled = false;

        [SerializeField]
        private bool m_analyseAllSources = false;

        [SerializeField]
        private float m_energyThreshold;

        [SerializeField]
        private List<SourceComponent> m_sources = null;

        #endregion

        #region public properties

        public bool Initialized { get; private set; }

        /// <summary>
        /// Checks if analysis in SoundToolKitAI is currently enabled.
        /// </summary>
        public bool AnalysisEnabled
        {
            get { return m_analysisEnabled; }
            set
            {
                m_analysisEnabled = value;
                if (value)
                {
                    m_toolkit.StartAnalysis();
                }
                else
                {
                    m_toolkit.StopAnalysis();
                }
            }
        }

        /// <summary>
        /// True if all sound sources in the scene should be analysed.
        /// </summary>
        public bool AnalyseAllSources
        {
            get { return m_analyseAllSources; }
            set
            {
                m_analyseAllSources = value;
                if (Initialized)
                {
                    if (m_analyseAllSources)
                    {
                        PrevStateAnalysisOff();
                    }
                    else
                    {
                        PrevStateAnalysisOn();
                    }
                }
            }
        }

        /// <summary>
        /// Energy threshold that is currently applied to energy analyzers.
        /// </summary>
        public float EnergyThreshold
        {
            get { return m_energyThreshold; }
            set
            {
                m_energyThreshold = value;
                m_toolkit.EnergyThreshold = m_energyThreshold;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Connect to event that fires, when energy above threshold is detected in some position
        /// </summary>
        /// <param name="onEnergyDetected">
        /// Callback with position as parameter, that firest when event occurs
        ///
        public void ConnectOnHighEnergyDetection(Action<Vector3> onEnergyDetected)
        {
            var @delegate = new EnergyDetectionHandler((posHandle) =>
            {
                onEnergyDetected(Vector3Extensions.ToUnityEngine(posHandle));
            });

            Action<Numerics.Vector3> action = new Action<Numerics.Vector3>(@delegate);
            m_toolkit.ConnectOnHighEnergyDetection(action);
        }

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "AudioEngine is not initialized.");
                var audioEngine = soundToolKitManager.StkAudioEngine;
                m_toolkit = new AiToolKit(audioEngine);

                SoundToolKitDebug.Assert(m_toolkit != null, "SoundToolKitAI is not initialized.");
                if (m_toolkit != null)
                {
                    SoundToolKitDebug.Log("SoundToolKitAI intialized.");
                    m_toolkit.StopAnalysis();
                    m_analysisEnabled = m_toolkit.AnalysisEnabled();
                    m_energyThreshold = m_toolkit.EnergyThreshold;
                    m_analyseAllSources = false;
                    m_wasAnalysisOnBeforeChange = false;
                }
                Initialized = true;
            }
        }

        #endregion

        #region private methods

        private void PrevStateAnalysisOn()
        {
            if (m_wasAnalysisOnBeforeChange)
            {
                if (m_sources.Count != m_sourcesAlreadyAdded.Count)
                {
                    m_toolkit.RemoveSoundSources();
                    m_sources.Clear();
                    m_sources = new List<SourceComponent>(m_sourcesAlreadyAdded);
                }

                m_sourcesAlreadyAdded.Clear();
            }

            foreach (var source in m_sources)
            {
                if (source != null && source is SpatialComponent)
                {
                    m_toolkit.AddSoundSource(((SpatialComponent)source).SourceSpatial);
                }
            }

            m_wasAnalysisOnBeforeChange = false;
        }

        private void PrevStateAnalysisOff()
        {
            if (m_sources.Count != 0)
            {
                m_sourcesAlreadyAdded = new List<SourceComponent>(m_sources);
            }

            foreach (var source in SoundToolKitManager.Instance.ResourceContainer.AudioSources)
            {
                if (source is SpatialComponent && !m_sources.Contains(source))
                {
                    m_sources.Add(source);
                    m_toolkit.AddSoundSource(((SpatialComponent)source).SourceSpatial);
                }
            }

            m_wasAnalysisOnBeforeChange = true;
        }

        private void UpdateProperties()
        {
            if (m_toolkit != null)
            {
                AnalysisEnabled = m_analysisEnabled;
                EnergyThreshold = m_energyThreshold;
                AnalyseAllSources = m_analyseAllSources;
            }

            if (Initialized)
            {
                // Unity is allowing to have duplicates in the container
                // Following lines make sure there aren't any
                m_sources.RemoveAll(source => source == null);
                m_sources = new List<SourceComponent>(m_sources.Distinct().ToList());
            }
        }

        #endregion

        #region unity methods

        private void Awake()
        {
            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);

            foreach (var source in m_sources)
            {
                if (source != null && source is SpatialComponent)
                {
                    m_toolkit.AddSoundSource(((SpatialComponent)source).SourceSpatial);
                }
            }
        }

        private void OnDestroy()
        {
            m_toolkit.Dispose();
            m_toolkit.audio().Finish();
        }

#if UNITY_EDITOR
        internal void OnValidate()
        {
            UpdateProperties();
        }
#endif

        private void Update()
        {
            if (m_analysisEnabled)
            {
                m_toolkit.AnalyzeEnergy();
            }
        }

        #endregion

        #region private delegates

        private delegate void EnergyDetectionHandler(Numerics.Vector3 position);

        #endregion

        #region private members

        [NonSerialized]
        private AiToolKit m_toolkit;

        [NonSerialized]
        private List<SourceComponent> m_sourcesAlreadyAdded;

        [NonSerialized]
        private bool m_wasAnalysisOnBeforeChange;

        #endregion
    }
}
