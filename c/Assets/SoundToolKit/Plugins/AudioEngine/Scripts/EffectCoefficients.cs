/// \author Michal Majewski <michal.majewski@techmo.pl>
/// \date 18.02.2019

using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// EffectCoefficients is a representation of a SoundToolKitMaterial's Effect - absorption, scattering, 
    /// transmission in the 8 octave bands - 125, 250, 500, 1000, 2000, 4000, 8000, 16000 [Hz]. 
    /// Effect in each band is a value ranging from 0 to 1.
    /// </summary>
    [Serializable]
    public class EffectCoefficients : ISerializationCallbackReceiver
    {
        #region editor fields

        [SerializeField]
        private float m_band125Hz;
        [SerializeField]
        private float m_band250Hz;
        [SerializeField]
        private float m_band500Hz;
        [SerializeField]
        private float m_band1000Hz;
        [SerializeField]
        private float m_band2000Hz;
        [SerializeField]
        private float m_band4000Hz;
        [SerializeField]
        private float m_band8000Hz;
        [SerializeField]
        private float m_band16000Hz;

        #endregion

        #region public events

        public event Action<EffectCoefficients> OnCoefficientsChanged;

        #endregion

        #region public properties

        public float Band125Hz
        {
            get { return m_band125Hz; }
            set
            {
                if (value != m_coefficientsArray[0])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[0] = value;
                    m_band125Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band250Hz
        {
            get { return m_band250Hz; }
            set
            {
                if (value != m_coefficientsArray[1])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[1] = value;
                    m_band250Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band500Hz
        {
            get { return m_band500Hz; }
            set
            {
                if (value != m_coefficientsArray[2])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[2] = value;
                    m_band500Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band1000Hz
        {
            get { return m_band1000Hz; }
            set
            {
                if (value != m_coefficientsArray[3])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[3] = value;
                    m_band1000Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band2000Hz
        {
            get { return m_band2000Hz; }
            set
            {
                if (value != m_coefficientsArray[4])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[4] = value;
                    m_band2000Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band4000Hz
        {
            get { return m_band4000Hz; }
            set
            {
                if (value != m_coefficientsArray[5])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[5] = value;
                    m_band4000Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band8000Hz
        {
            get { return m_band8000Hz; }
            set
            {
                if (value != m_coefficientsArray[6])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[6] = value;
                    m_band8000Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        public float Band16000Hz
        {
            get { return m_band16000Hz; }
            set
            {
                if (value != m_coefficientsArray[7])
                {
                    value = ClampToCorrectValue(value);
                    m_coefficientsArray[7] = value;
                    m_band16000Hz = value;

                    if (OnCoefficientsChanged != null)
                    {
                        OnCoefficientsChanged(this);
                    }
                }
            }
        }

        #endregion

        #region internal constructors

        internal EffectCoefficients(Effect effect)
        {
            m_coefficientsArray = new float[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            effect.GetCoefficients(coefficients =>
            {
                Band125Hz = coefficients[0];
                Band250Hz = coefficients[1];
                Band500Hz = coefficients[2];
                Band1000Hz = coefficients[3];
                Band2000Hz = coefficients[4];
                Band4000Hz = coefficients[5];
                Band8000Hz = coefficients[6];
                Band16000Hz = coefficients[7];
            });
        }

        internal EffectCoefficients(float[] coefficients)
        {
            SoundToolKitDebug.Assert(coefficients.Length == 8, "Improper number of coefficients used when trying" +
                " to initialize EffectCoefficients!");

            m_coefficientsArray = new float[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            Band125Hz = coefficients[0];
            Band250Hz = coefficients[1];
            Band500Hz = coefficients[2];
            Band1000Hz = coefficients[3];
            Band2000Hz = coefficients[4];
            Band4000Hz = coefficients[5];
            Band8000Hz = coefficients[6];
            Band16000Hz = coefficients[7];
        }

        #endregion

        #region internal methods

        internal float[] GetCoefficientArray()
        {
            if (m_coefficientsArray == null)
            {
                m_coefficientsArray = new float[] { Band125Hz, Band250Hz, Band500Hz, Band1000Hz, Band2000Hz, Band4000Hz, Band8000Hz, Band16000Hz };
            }
            return m_coefficientsArray;
        }

#if UNITY_EDITOR
        internal void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UpdateProperties();
            }
        }
#endif

        #endregion

        #region ISerializationCallbackReceiver implementation

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            OnValidate();
#endif
        }

        public void OnAfterDeserialize()
        {
        }

        #endregion

        #region private methods

        private float ClampToCorrectValue(float value)
        {
            return value < 0 ? 0 : value > 1 ? 1 : value;
        }

        private void UpdateProperties()
        {
            if (m_coefficientsArray != null)
            {
                Band125Hz = m_band125Hz;
                Band250Hz = m_band250Hz;
                Band500Hz = m_band500Hz;
                Band1000Hz = m_band1000Hz;
                Band2000Hz = m_band2000Hz;
                Band4000Hz = m_band4000Hz;
                Band8000Hz = m_band8000Hz;
                Band16000Hz = m_band16000Hz;
            }
        }

        #endregion

        #region private members

        [NonSerialized]
        private float[] m_coefficientsArray;

        #endregion
    }
}
