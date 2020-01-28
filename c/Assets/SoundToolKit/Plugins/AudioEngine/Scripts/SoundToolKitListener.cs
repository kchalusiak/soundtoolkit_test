/// \author Marcin Misiek
/// \date 26.04.2018

using SoundToolKit.Unity.Utils;
using System;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// The Listener controls a virtual microphone. 
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    [AddComponentMenu("SoundToolKit/SoundToolKitListener")]
    public sealed class SoundToolKitListener : TransformableObject, ISoundToolKitObserver
    {
        #region public properties

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "AudioEngine is null");

                m_receiver = soundToolKitManager.StkAudioEngine.Scene.Receiver;
                SoundToolKitDebug.Assert(m_receiver != null, "Receiver is null");

                soundToolKitManager.ResourceContainer.AudioListener = this;

                SubscribeOnTransformChanged(m_receiver);

                Initialized = true;
            }
        }

        #endregion

        #region private unity methods

        private void Awake()
        {
            Initialized = false;
            SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
        }

        private void OnDestroy()
        {
            UnsubscribeFromTransformChanged(m_receiver);
            m_receiver.Dispose();
        }

        #endregion

        #region private members

        [NonSerialized]
        private Receiver m_receiver;

        #endregion
    }
}