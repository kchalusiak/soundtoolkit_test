/// \author Jan Wilczek
/// \date 02.08.2018
/// \author Magdalena Malon <magdalena.malon@techmo.pl>
/// \date 22.11.2018
///

using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Base class for Stk SoundSources.
    /// </summary>
    public abstract class SourceComponent : TransformableObject, ISoundToolKitObserver
    {
        #region editor fields

        public bool playOnAwake = true;

        [SerializeField]
        protected bool m_muted = false;

        [SerializeField]
        protected float m_volume = 0.75f;

        #endregion

        #region public properties

        public bool Initialized { get; protected set; }

        public abstract bool IsPlaying { get; }

        public abstract bool Muted { get; set; }

        public abstract float Volume { get; set; }

        #endregion

        #region public methods

        public abstract void OnStkInitialized(SoundToolKitManager soundToolKitManager);

        public abstract void Play();

        public abstract void Stop();

        #endregion

        #region protected virtual methods

        #region unity methods

        protected abstract void Awake();

        protected abstract void OnDisable();

        protected abstract void OnEnable();

        protected abstract void OnDestroy();

#if UNITY_EDITOR
        protected abstract void OnValidate();
#endif

        #endregion

        #endregion

        #region protected members

        protected bool m_isPlayingPreviousState;

        #endregion
    }
}
