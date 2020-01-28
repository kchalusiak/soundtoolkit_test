///
/// \author Magdalena Malon <magdalena.malon@techmo.pl>
/// \date 22.11.2018
///

using SoundToolKit.Unity.Utils;
using System.Linq;
using UnityEngine;

using AttenuationCurve = UnityEngine.AnimationCurve;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Base class for StkSpatialSource and StkStreamingSource. It contains common methods of SpatialSoundSource.
    /// </summary>
    public abstract class SpatialComponent : SourceComponent
    {
        #region editor fields

        [SerializeField]
        protected float m_simulationQuality = 1f;

        [SerializeField]
        protected bool m_hrtfSpatializationEnabled = true;

        [SerializeField]
        protected bool m_attenuationEnabled = true;

        [SerializeField]
        protected bool m_reflectionEnabled = true;

        [SerializeField]
        protected bool m_scatteringEnabled = true;

        [SerializeField]
        protected bool m_transmissionEnabled = true;

        [SerializeField]
        protected bool m_diffractionEnabled = true;

        [SerializeField]
        protected bool m_reverbEnabled = true;

        [SerializeField]
        protected float m_maxDistance = 300f;

        [SerializeField]
        protected float m_minDistance = 0.25f;

        [SerializeField]
        protected SoundAttenuation m_attenuation = SoundAttenuation.PointSource;

        [SerializeField]
        protected AttenuationCurve m_attenuationCurve = AttenuationCurve.Linear(0, 1, 300, 0); //just a default value

        #endregion

        #region public properties

        public override bool Muted
        {
            get { return m_spatialSoundSource != null ? m_spatialSoundSource.Muted : m_muted; }
            set
            {
                m_muted = value;

                if (m_spatialSoundSource != null)
                {
                    m_spatialSoundSource.Muted = m_muted;
                }
            }
        }

        public override float Volume
        {
            get { return m_spatialSoundSource != null ? m_spatialSoundSource.Volume : m_volume; }
            set
            {
                m_volume = value;

                if (m_spatialSoundSource != null)
                {
                    m_spatialSoundSource.Volume = m_volume;
                }
            }
        }

        #region 3d sound

        /// <summary>
        /// This currently has no effect.
        /// Will be added in the future releases.
        /// Determines the quality of source processing. The value affects how many engine resources are delegated for processing sound emitted by this source. 
        ///	The simulation quality also determines the priority of given source, meaning, that sources with higher simulation quality can have lower latency. 
        ///	Quality in range 0 to 1.0, where:
        ///		1.0 - the best possible quality
        ///		0.0 - source not processed at all
        /// </summary>
        public float SimulationQuality
        {
            get { return m_spatialSoundSource != null ? m_spatialSoundSource.SimulationQuality : m_simulationQuality; }
            set
            {
                m_simulationQuality = value;

                if (m_spatialSoundSource != null)
                {
                    m_spatialSoundSource.SimulationQuality = m_simulationQuality;
                }
            }
        }

        #region Spatial effect control

        /// <summary>
        /// Enables/Disables HRTF processing of this Sound Source.
        /// </summary>
        public bool HrtfSpatializationEnabled
        {
            get { return m_spatialSoundSource != null ? m_spatialSoundSource.HrtfSpatializationEnabled : m_hrtfSpatializationEnabled; }
            set
            {
                m_hrtfSpatializationEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    m_spatialSoundSource.HrtfSpatializationEnabled = m_hrtfSpatializationEnabled;
                }
            }
        }

        /// <summary>
        /// Enables/Disables Attenuation with distance effect on this Sound Source.
        /// </summary>
        public bool AttenuationEnabled
        {
            get { return m_attenuationEnabled; }
            set
            {
                m_attenuationEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    if (value)
                    {
                        m_spatialSoundSource.EnableSpatialEffect(SpatialEffectType.Attenuation);
                    }
                    else
                    {
                        m_spatialSoundSource.DisableSpatialEffect(SpatialEffectType.Attenuation);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables geometric Reflection effect on this Sound Source.
        /// </summary>
        public bool ReflectionEnabled
        {
            get { return m_reflectionEnabled; }
            set
            {
                m_reflectionEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    if (value)
                    {
                        m_spatialSoundSource.EnableSpatialEffect(SpatialEffectType.Reflection);
                    }
                    else
                    {
                        m_spatialSoundSource.DisableSpatialEffect(SpatialEffectType.Reflection);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables geometric Scattering effect on this Sound Source.
        /// </summary>
        public bool ScatteringEnabled
        {
            get { return m_scatteringEnabled; }
            set
            {
                m_scatteringEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    if (value)
                    {
                        m_spatialSoundSource.EnableSpatialEffect(SpatialEffectType.Scattering);
                    }
                    else
                    {
                        m_spatialSoundSource.DisableSpatialEffect(SpatialEffectType.Scattering);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables geometric Transmission effect on this Sound Source.
        /// </summary>
        public bool TransmissionEnabled
        {
            get { return m_transmissionEnabled; }
            set
            {
                m_transmissionEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    if (value)
                    {
                        m_spatialSoundSource.EnableSpatialEffect(SpatialEffectType.Transmission);
                    }
                    else
                    {
                        m_spatialSoundSource.DisableSpatialEffect(SpatialEffectType.Transmission);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables geometric Diffraction effect on this Sound Source.
        /// </summary>
        public bool DiffractionEnabled
        {
            get { return m_diffractionEnabled; }
            set
            {
                m_diffractionEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    if (value)
                    {
                        m_spatialSoundSource.EnableSpatialEffect(SpatialEffectType.Diffraction);
                    }
                    else
                    {
                        m_spatialSoundSource.DisableSpatialEffect(SpatialEffectType.Diffraction);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables late reverb processing of this Sound Source.
        /// </summary>
        public bool ReverbEnabled
        {
            get { return m_spatialSoundSource != null ? m_spatialSoundSource.ReverberationEnabled : m_reverbEnabled; }
            set
            {
                m_reverbEnabled = value;

                if (m_spatialSoundSource != null)
                {
                    m_spatialSoundSource.ReverberationEnabled = m_reverbEnabled;
                }
            }
        }

        #endregion

        /// <summary>
        /// The custom Attenuation curve interpreted from Unity's AnimationCurve. It will be used if Attenuation 
        /// is set to Custom. Axis x represents distance while axis y represents attenuation in range 0 to 1.0, where:
        ///     1.0 - no attenuation
        ///     0.0 - full attenuation
        /// </summary>
        public AttenuationCurve AttenuationCurve
        {
            get { return m_attenuationCurve; }
            private set
            {
                m_attenuationCurve = value;
                SetAttenuationFunction();
            }
        }

        /// <summary>
        /// Max distance of virtual sphere that simulates boundary of source volume
        /// This is only valid in Linear and ReverseLog Attenuation curve.
        /// </summary>
        public float MaxDistance
        {
            get { return m_maxDistance; }
            set { m_maxDistance = value; }
        }

        /// <summary>
        ///	Radius of virtual sphere that simulates boundary of source volume.
        ///	This means that below this distance from source attenuation will not occur.
        ///	Above that distance attenuation will be calculated according to chosen attenuation curve (see setAttenuation)
        /// </summary>
        public float MinDistance
        {
            get { return m_minDistance; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                    SoundToolKitDebug.LogWarning("Minimial distance cannot be a negative number.");
                }

                m_minDistance = System.Math.Max(value, 0.001f);
            }
        }

        /// <summary>
        ///	The attenuation model which should be used by this source
        /// </summary>
        public SoundAttenuation Attenuation
        {
            get { return m_attenuation; }
            set
            {
                m_attenuation = value;
                SetAttenuationFunction();
            }
        }

        #endregion

        #endregion

        #region public methods

        public override void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                // <STK.LITE>
                // Warning: Modifying this code may result in plugin instability and frequent crashes to desktop.
                // This is the SoundToolKit Lite version.
                // Internally only three SoundSources can be created simultaneously, so circumventing this check will not result
                // in a possibility of creating more sound sources - only more references to the same source.
                if (soundToolKitManager.ResourceContainer.AudioSources.Count >= StkLite.AvailableSources())
                {
                    SoundToolKitDebug.Log("Only three SoundSources are available simultaneously in the SoundToolKit " +
                        "Lite version. Deleting the illegal SoundSource components at GameObject " + gameObject.name);
                    Destroy(this);
                    return;
                }
                // </STK.LITE>

                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "AudioEngine is not initialized.");

                var audioEngine = soundToolKitManager.StkAudioEngine;
                m_spatialSoundSource = audioEngine.ResourcesFactory.CreateSpatialSoundSource();

                UpdateSourceProperties();

                SubscribeOnTransformChanged(m_spatialSoundSource);

                soundToolKitManager.ResourceContainer.Add(audioSource: this);
            }
        }

        #endregion

        #region internal properties

        internal SourceSpatial SourceSpatial
        {
            get { return m_spatialSoundSource; }
        }

        #endregion

        #region protected virtual methods

        #region unity methods

        protected override void OnDestroy()
        {
            if (m_spatialSoundSource != null)
            {
                UnsubscribeFromTransformChanged(m_spatialSoundSource);
                m_spatialSoundSource.Dispose();

                //Unregister
                var soundToolKitManager = SoundToolKitManager.Instance;

                if (soundToolKitManager != null)
                {
                    soundToolKitManager.ResourceContainer.Remove(audioSource: this);
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            UpdateSourceProperties();
        }
#endif

        #endregion

        #endregion

        #region private methods

        private void UpdateSourceProperties()
        {
            Muted = m_muted;
            Volume = m_volume;
            SimulationQuality = m_simulationQuality;

            HrtfSpatializationEnabled = m_hrtfSpatializationEnabled;
            AttenuationEnabled = m_attenuationEnabled;
            ReflectionEnabled = m_reflectionEnabled;
            ScatteringEnabled = m_scatteringEnabled;
            TransmissionEnabled = m_transmissionEnabled;
            DiffractionEnabled = m_diffractionEnabled;

            MaxDistance = m_maxDistance;
            MinDistance = m_minDistance;
            Attenuation = m_attenuation;
        }

        private void SetAttenuationFunction()
        {
            if (m_spatialSoundSource != null)
            {
                if (m_attenuation != SoundAttenuation.Custom)
                {
                    m_spatialSoundSource.Attenuation = GetPredefinedAttenuation();
                }
                else
                {
                    m_spatialSoundSource.Attenuation = m_attenuationCurve.Evaluate;
                }
            }
        }

        private System.Func<float, float> GetPredefinedAttenuation()
        {
            switch (Attenuation)
            {
                case SoundAttenuation.Nothing:
                    return (distance) => 1.0f;
                case SoundAttenuation.PointSource:
                    return (distance) => SoundToolKit.Extensions.Attenuation.PointSource(distance, MinDistance);
                case SoundAttenuation.LineSource:
                    return (distance) => SoundToolKit.Extensions.Attenuation.LineSource(distance, MinDistance);
                case SoundAttenuation.Linear:
                    return (distance) => SoundToolKit.Extensions.Attenuation.Linear(distance, MinDistance, MaxDistance);
                case SoundAttenuation.Logarithmic:
                    return (distance) => SoundToolKit.Extensions.Attenuation.Logarithmic(distance, MinDistance, MaxDistance);
                case SoundAttenuation.Inverse:
                    return (distance) => SoundToolKit.Extensions.Attenuation.Inverse(distance, MinDistance, MaxDistance);
                case SoundAttenuation.ReverseLog:
                    return (distance) => SoundToolKit.Extensions.Attenuation.ReverseLog(distance, MinDistance, MaxDistance);
                default:
                    SoundToolKitDebug.LogError("Invalid attenuation type.");
                    return (distance) => 0.0f;
            }
        }

        #endregion

        #region protected members

        protected SourceSpatial m_spatialSoundSource;

        #endregion
    }
}
