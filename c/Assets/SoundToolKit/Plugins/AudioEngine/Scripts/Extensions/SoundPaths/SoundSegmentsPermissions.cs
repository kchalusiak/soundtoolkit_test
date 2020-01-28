/// \author Marcin Misiek
/// \date 15.06.2018

using SoundToolKit.Extensions.Diagnostics;
using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;

namespace SoundToolKit.Unity.Extensions
{
    [Serializable]
    public class SoundSegmentsPermissions
    {
        #region editor fields

        [SerializeField]
        private bool m_sourceRendered = true;

        [SerializeField]
        private bool m_receiverRendered = false;

        [SerializeField]
        private bool m_reflectionRendered = true;

        [SerializeField]
        private bool m_scatteringRendered = true;

        [SerializeField]
        private bool m_transmissionRendered = true;

        [SerializeField]
        private bool m_diffractionRendered = true;

        #endregion

        #region public properties

        /// <summary>
        /// Source paths bool - default Magenta.
        /// </summary>
        public bool SourceRendered
        {
            get { return m_sourceRendered; }
            set
            {
                m_sourceRendered = value;
            }
        }

        /// <summary>
        /// Receiver paths bool - default Red.
        /// </summary>
        public bool ReceiverRendered
        {
            get { return m_receiverRendered; }
            set
            {
                m_receiverRendered = value;
            }
        }

        /// <summary>
        /// Reflection paths bool - default Green.
        /// </summary>
        public bool ReflectionRendered
        {
            get { return m_reflectionRendered; }
            set
            {
                m_reflectionRendered = value;
            }
        }

        /// <summary>
        /// Scattering paths bool - default Cyan.
        /// </summary>
        public bool ScatteringRendered
        {
            get { return m_scatteringRendered; }
            set
            {
                m_scatteringRendered = value;
            }
        }

        /// <summary>
        /// Transmission paths bool - default Yellow.
        /// </summary>
        public bool TransmissionRendered
        {
            get { return m_transmissionRendered; }
            set
            {
                m_transmissionRendered = value;
            }
        }

        /// <summary>
        /// Diffraction paths bool - default Blue.
        /// </summary>
        public bool DiffractionRendered
        {
            get { return m_diffractionRendered; }
            set
            {
                m_diffractionRendered = value;
            }
        }

        #endregion

        #region public methods

        public bool Get(PathVertexType type)
        {
            switch (type)
            {
                case PathVertexType.Source: return SourceRendered;
                case PathVertexType.Receiver: return ReceiverRendered;
                case PathVertexType.Reflection: return ReflectionRendered;
                case PathVertexType.Scattering: return ScatteringRendered;
                case PathVertexType.Transmission: return TransmissionRendered;
                case PathVertexType.Diffraction: return DiffractionRendered;
                default:
                    SoundToolKitDebug.Assert(false, "This enum case is not handled");
                    return SourceRendered;
            }
        }

        public void Update()
        {
            SourceRendered = m_sourceRendered;
            ReceiverRendered = m_receiverRendered;
            ReflectionRendered = m_reflectionRendered;
            ScatteringRendered = m_scatteringRendered;
            TransmissionRendered = m_transmissionRendered;
            DiffractionRendered = m_diffractionRendered;
        }

        #endregion
    }
}
