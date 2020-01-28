/// \author Michal Majewski
/// \date 09.05.2019
/// 
using SoundToolKit.Unity.Utils;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Allows controlling the per-scene global settings of SoundToolKit, such as volume levels and effects that occur on the scene.
    /// </summary>
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/SoundToolKitSceneConfiguration")]
    public class SoundToolKitSceneConfiguration : MonoBehaviour, ISoundToolKitObserver
    {
        #region editor fields

        [SerializeField]
        private float m_spatialVolume = 1.0f;

        [SerializeField]
        private float m_ambientVolume = 1.0f;

        [SerializeField]
        private float m_reverbVolume = 1.0f;

        [SerializeField]
        private bool m_attenuationEnabled = true;

        [SerializeField]
        private bool m_reflectionEnabled = true;

        [SerializeField]
        private bool m_scatteringEnabled = false;

        [SerializeField]
        private bool m_transmissionEnabled = true;

        [SerializeField]
        private bool m_diffractionEnabled = true;

        [SerializeField]
        private bool m_reverbEnabled = true;

        [SerializeField]
        private float m_speedOfSound = 343.0f;

        [SerializeField]
        private bool m_dampingEnabled = false;

        [SerializeField]
        private EffectCoefficients m_dampingCoefficients = null;

        #endregion

        #region public properties

        /// <summary>
        /// The output volume of SoundToolKit's spatial processing of Sound Paths.
        /// </summary>
        public float SpatialVolume
        {
            get { return m_spatialVolume; }
            set
            {
                m_spatialVolume = value;

                if (Initialized)
                {
                    SoundToolKitManager.Instance.StkAudioEngine.Scene.SpatialVolume = m_spatialVolume;
                }
            }
        }

        /// <summary>
        /// The output volume of SoundToolKit's Ambient Sound Sources.
        /// </summary>
        public float AmbientVolume
        {
            get { return m_ambientVolume; }
            set
            {
                m_ambientVolume = value;

                if (Initialized)
                {
                    SoundToolKitManager.Instance.StkAudioEngine.Scene.AmbientVolume = m_ambientVolume;
                }
            }
        }

        /// <summary>
        /// The output volume of SoundToolKit's late reverberation processor.
        /// </summary>
        public float ReverbVolume
        {
            get { return m_reverbVolume; }
            set
            {
                m_reverbVolume = value;

                if (Initialized)
                {
                    SoundToolKitManager.Instance.StkAudioEngine.Scene.ReverbVolume = m_reverbVolume;
                }
            }
        }

        /// <summary>
        /// Enables/Disables global Attenuation with distance effect.
        /// </summary>
        public bool AttenuationEnabled
        {
            get { return m_attenuationEnabled; }
            set
            {
                m_attenuationEnabled = value;

                if (Initialized)
                {
                    if (value)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.EnableSpatialEffect(SpatialEffectType.Attenuation);
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.DisableSpatialEffect(SpatialEffectType.Attenuation);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables global Reflection effect.
        /// </summary>
        public bool ReflectionEnabled
        {
            get { return m_reflectionEnabled; }
            set
            {
                m_reflectionEnabled = value;

                if (Initialized)
                {
                    if (value)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.EnableSpatialEffect(SpatialEffectType.Reflection);
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.DisableSpatialEffect(SpatialEffectType.Reflection);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables global Scattering effect.
        /// </summary>
        public bool ScatteringEnabled
        {
            get { return m_scatteringEnabled; }
            set
            {
                m_scatteringEnabled = value;

                if (Initialized)
                {
                    if (value)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.EnableSpatialEffect(SpatialEffectType.Scattering);
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.DisableSpatialEffect(SpatialEffectType.Scattering);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables global Transmission effect.
        /// </summary>
        public bool TransmissionEnabled
        {
            get { return m_transmissionEnabled; }
            set
            {
                m_transmissionEnabled = value;

                if (Initialized)
                {
                    if (value)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.EnableSpatialEffect(SpatialEffectType.Transmission);
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.DisableSpatialEffect(SpatialEffectType.Transmission);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables global Diffraction effect.
        /// </summary>
        public bool DiffractionEnabled
        {
            get { return m_diffractionEnabled; }
            set
            {
                m_diffractionEnabled = value;

                if (Initialized)
                {
                    if (value)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.EnableSpatialEffect(SpatialEffectType.Diffraction);
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.DisableSpatialEffect(SpatialEffectType.Diffraction);
                    }
                }
            }
        }

        /// <summary>
        /// Enables/Disables global late reverb processing.
        /// </summary>
        public bool ReverbEnabled
        {
            get { return m_reverbEnabled; }
            set
            {
                m_reverbEnabled = value;

                if (Initialized)
                {
                    SoundToolKitManager.Instance.StkAudioEngine.Scene.ReverberationEnabled = m_reverbEnabled;
                }
            }
        }

        /// <summary>
        /// Speed of sound in an Acoustic Medium. Defaults to 343 m/s, which is the speed of sound in 25 degree Celsius air.
        /// </summary>
        public float SpeedOfSound
        {
            get { return m_speedOfSound; }
            set
            {
                m_speedOfSound = Mathf.Clamp(value, 100.0f, 1000.0f);

                if (Initialized)
                {
                    SoundToolKitManager.Instance.StkAudioEngine.Scene.AcousticMedium.SetSpeedOfSound(m_speedOfSound);
                }
            }
        }

        public bool DampingEnabled
        {
            get { return m_dampingEnabled; }
            set
            {
                m_dampingEnabled = value;

                if (Initialized)
                {
                    if (m_dampingEnabled)
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.AcousticMedium.DampingWithDistance.Enable();
                    }
                    else
                    {
                        SoundToolKitManager.Instance.StkAudioEngine.Scene.AcousticMedium.DampingWithDistance.Disable();
                    }
                }
            }
        }

        /// <summary>
        /// Controls how sound is attenuated due to the Acoustic Medium's viscosity.
        /// The default value corresponds to air, temperature: 15 degrees Celsius, pressure: 1013 hPa, humidity: 30%
        /// </summary>
        /// <note>Calculated by the formula: freqResponse = exp2(-coefficient * distance) [in octave bands]</note>
        public EffectCoefficients DampingCoefficients
        {
            get { return m_dampingCoefficients; }
            private set { m_dampingCoefficients = value; }
        }

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "Audio engine is not initialized. Logical error.");

                UpdateDamping(DampingCoefficients);
                DampingCoefficients.OnCoefficientsChanged += UpdateDamping;

                SoundToolKitManager.Instance.ResourceContainer.SceneConfiguration = this;
                Initialized = true;
                UpdateProperties();
            }
        }

        #endregion

        #region private methods

        private void Reset()
        {
            var airDamping = new float[] { 4.99651760e-05f, 0.000116024588f, 0.000379357982f, 0.00142597477f, 0.00550973462f, 0.0203645360f, 0.0633036569f, 0.137860075f };
            DampingCoefficients = new EffectCoefficients(airDamping);
        }

        private void Awake()
        {
            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateProperties();
        }
#endif

        private void OnDestroy()
        {
            if (Initialized)
            {
                DampingCoefficients.OnCoefficientsChanged -= UpdateDamping;
                SoundToolKitManager.Instance.ResourceContainer.SceneConfiguration = null;
            }
        }

        private void UpdateProperties()
        {
            if (Initialized)
            {
                SpatialVolume = m_spatialVolume;
                AmbientVolume = m_ambientVolume;
                ReverbVolume = m_reverbVolume;

                AttenuationEnabled = m_attenuationEnabled;
                ReflectionEnabled = m_reflectionEnabled;
                ScatteringEnabled = m_scatteringEnabled;
                TransmissionEnabled = m_transmissionEnabled;
                DiffractionEnabled = m_diffractionEnabled;
                ReverbEnabled = m_reverbEnabled;

                SpeedOfSound = m_speedOfSound;
                DampingEnabled = m_dampingEnabled;
            }
        }

        private void UpdateDamping(EffectCoefficients coeffs)
        {
            SoundToolKitManager.Instance.StkAudioEngine.Scene.AcousticMedium.DampingWithDistance.SetCoefficients(coeffs.GetCoefficientArray());
        }

        #endregion

        #region private members

        #endregion
    }
}
