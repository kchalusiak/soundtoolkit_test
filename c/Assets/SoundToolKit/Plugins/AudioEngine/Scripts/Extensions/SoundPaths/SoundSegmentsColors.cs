/// \author Marcin Misiek
/// \date 15.06.2018

using SoundToolKit.Extensions.Diagnostics;
using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;

namespace SoundToolKit.Unity.Extensions
{
    [Serializable]
    public class SoundSegmentsColors
    {
        #region editor fields

        [SerializeField]
        private Color m_source = UnityEngine.Color.magenta;

        [SerializeField]
        private Color m_receiver = Color.red;

        [SerializeField]
        private Color m_reflection = Color.green;

        [SerializeField]
        private Color m_scattering = Color.cyan;

        [SerializeField]
        private Color m_transmission = Color.yellow;

        [SerializeField]
        private Color m_diffraction = Color.blue;

        #endregion

        #region public properties

        /// <summary>
        /// Source paths color - default Magenta.
        /// </summary>
        public Color Source
        {
            get { return m_source; }
            set
            {
                m_source = value;
            }
        }

        /// <summary>
        /// Receiver paths color - default Red.
        /// </summary>
        public Color Receiver
        {
            get { return m_receiver; }
            set
            {
                m_receiver = value;
            }
        }

        /// <summary>
        /// Reflection paths color - default Green.
        /// </summary>
        public Color Reflection
        {
            get { return m_reflection; }
            set
            {
                m_reflection = value;
            }
        }

        /// <summary>
        /// Scattering paths color - default Cyan.
        /// </summary>
        public Color Scattering
        {
            get { return m_scattering; }
            set
            {
                m_scattering = value;
            }
        }

        /// <summary>
        /// Transmission paths color - default Yellow.
        /// </summary>
        public Color Transmission
        {
            get { return m_transmission; }
            set
            {
                m_transmission = value;
            }
        }

        /// <summary>
        /// Diffraction paths color - default Blue.
        /// </summary>
        public Color Diffraction
        {
            get { return m_diffraction; }
            set
            {
                m_diffraction = value;
            }
        }

        #endregion

        #region public methods

        public Color Get(PathVertexType type)
        {
            switch (type)
            {
                case PathVertexType.Source: return Source;
                case PathVertexType.Receiver: return Receiver;
                case PathVertexType.Reflection: return Reflection;
                case PathVertexType.Scattering: return Scattering;
                case PathVertexType.Transmission: return Transmission;
                case PathVertexType.Diffraction: return Diffraction;
                default:
                    SoundToolKitDebug.Assert(false, "This enum case is not handled");
                    return Source;
            }
        }

        public void Update()
        {
            Source = m_source;
            Receiver = m_receiver;
            Reflection = m_reflection;
            Scattering = m_scattering;
            Transmission = m_transmission;
            Diffraction = m_diffraction;
        }

        #endregion
    }
}