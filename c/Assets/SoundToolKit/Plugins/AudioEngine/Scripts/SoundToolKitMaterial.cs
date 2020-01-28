/// \author Marcin Misiek
/// \date 14.06.2018
/// 
/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 18.02.2019

using SoundToolKit.Extensions.Scene;
using SoundToolKit.Unity.Utils;
using System.IO;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// StkMaterial holds path to the corresponding json which contains data about necessary models:
    /// Absorption, Scattering and Transmission. Those models can be toggled.
    /// </summary>
    public sealed class SoundToolKitMaterial : ScriptableObject
    {
        #region editor fields

        [SerializeField]
        private bool m_absorptionEnabled = true;

        [SerializeField]
        private bool m_scatteringEnabled = true;

        [SerializeField]
        private bool m_transmissionEnabled = true;

        [SerializeField]
        private UnityEngine.Material m_visualMaterial = null;

        [SerializeField]
        private EffectCoefficients m_absorptionCoefficients = null;

        [SerializeField]
        private EffectCoefficients m_scatteringCoefficients = null;

        [SerializeField]
        private EffectCoefficients m_transmissionCoefficients = null;

        [SerializeField, HideInInspector]
        private string m_filePath = null;

        #endregion

        #region public properties

        /// <summary>
        /// UnityMaterial which will be used to visualize the StkMaterial.
        /// </summary>
        public UnityEngine.Material VisualMaterial
        {
            get { return m_visualMaterial; }
            set
            {
                if (value != VisualMaterial)
                {
                    m_visualMaterial = value;
                }
            }
        }

        public EffectCoefficients AbsorptionCoefficients
        {
            get { return m_absorptionCoefficients; }
            private set { m_absorptionCoefficients = value; }
        }

        public EffectCoefficients ScatteringCoefficients
        {
            get { return m_scatteringCoefficients; }
            private set { m_scatteringCoefficients = value; }
        }

        public EffectCoefficients TransmissionCoefficients
        {
            get { return m_transmissionCoefficients; }
            private set { m_transmissionCoefficients = value; }
        }

        #endregion

        #region internal properties

        /// <summary>
        /// Path to json which contains all necessary data to create StkMaterial.
        /// </summary>
        internal string FilePath
        {
            get { return m_filePath; }
            set
            {
                m_filePath = value;
            }
        }

        #endregion

        #region internal methods

        internal Material GetStkMaterial()
        {
            if (m_acousticMaterial == null || !m_acousticMaterial.Valid)
            {
                var soundToolKitManager = SoundToolKitManager.Instance;
                SoundToolKitDebug.Assert(soundToolKitManager != null, "Manager is null");
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "AudioEngine is null");

                LoadMaterial(soundToolKitManager);

                UpdateStkProperties();
            }

            return m_acousticMaterial;
        }

        #endregion

        #region private methods   

        private void LoadMaterial(SoundToolKitManager soundToolKitManager)
        {
            var pathInStreamingAssets = Path.Combine(Application.streamingAssetsPath, m_filePath);
            var fullPath = Path.GetFullPath(pathInStreamingAssets);

            if (m_acousticMaterial == null)
            {
                soundToolKitManager.ResourceContainer.Add(acousticMaterial: this);
            }

            m_acousticMaterial = MaterialsLoader.LoadJsonFromMemory(soundToolKitManager.StkAudioEngine, File.ReadAllText(fullPath));

            LoadEffectCoefficients();
        }

        private void LoadEffectCoefficients()
        {
            if (m_acousticMaterial != null)
            {
                AbsorptionCoefficients = new EffectCoefficients(m_acousticMaterial.Absorption);
                AbsorptionCoefficients.OnCoefficientsChanged += UpdateAbsorption;

                ScatteringCoefficients = new EffectCoefficients(m_acousticMaterial.Scattering);
                ScatteringCoefficients.OnCoefficientsChanged += UpdateScattering;

                TransmissionCoefficients = new EffectCoefficients(m_acousticMaterial.Transmission);
                TransmissionCoefficients.OnCoefficientsChanged += UpdateTransmission;
            }
        }

        private void UpdateAbsorption(EffectCoefficients coeffs)
        {
            m_acousticMaterial.Absorption.SetCoefficients(coeffs.GetCoefficientArray());
        }

        private void UpdateScattering(EffectCoefficients coeffs)
        {
            m_acousticMaterial.Scattering.SetCoefficients(coeffs.GetCoefficientArray());
        }

        private void UpdateTransmission(EffectCoefficients coeffs)
        {
            m_acousticMaterial.Transmission.SetCoefficients(coeffs.GetCoefficientArray());
        }

        #region unity methods

        private void Awake()
        {
#if !UNITY_EDITOR
            UpdateComponentProperties();
# endif
        }

        private void OnDestroy()
        {
            var soundToolKitManager = SoundToolKitManager.Instance;

            if (m_acousticMaterial != null)
            {
                AbsorptionCoefficients.OnCoefficientsChanged -= UpdateAbsorption;
                ScatteringCoefficients.OnCoefficientsChanged -= UpdateScattering;
                TransmissionCoefficients.OnCoefficientsChanged -= UpdateTransmission;
                m_acousticMaterial.Dispose();
            }

            if (soundToolKitManager != null)
            {
                soundToolKitManager.ResourceContainer.Remove(acousticMaterial: this);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateComponentProperties();

            if (m_acousticMaterial != null)
            {
                UpdateStkProperties();
            }
        }
#endif

        #region editor fields update

        private void UpdateComponentProperties()
        {
            FilePath = m_filePath;
            VisualMaterial = m_visualMaterial;
        }

        private void UpdateStkProperties()
        {
            if (m_absorptionEnabled)
            {
                m_acousticMaterial.Absorption.Enable();
            }
            else
            {
                m_acousticMaterial.Absorption.Disable();
            }

            if (m_scatteringEnabled)
            {
                m_acousticMaterial.Scattering.Enable();
            }
            else
            {
                m_acousticMaterial.Scattering.Disable();
            }

            if (m_transmissionEnabled)
            {
                m_acousticMaterial.Transmission.Enable();
            }
            else
            {
                m_acousticMaterial.Transmission.Disable();
            }
        }

        #endregion

        #endregion

        #endregion

        #region private members

        private Material m_acousticMaterial;

        #endregion
    }
}
