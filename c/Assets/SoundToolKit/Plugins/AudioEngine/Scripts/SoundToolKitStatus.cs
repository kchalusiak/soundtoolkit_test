/// \author Marcin Misiek
/// \date 26.04.2018

using SoundToolKit.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    //TODO: [Merge_discussion] - this might be easily changed to the ScriptableObject which would allow 
    //  to have something like log dump when engine would stop working. This could make debugging easier for us.

    /// <summary>
    /// This allows to view current status of SoundToolKit Audio Engine. 
    /// </summary>
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/SoundToolKitStatus")]
    public sealed class SoundToolKitStatus : MonoBehaviour, ISoundToolKitObserver
    {
        #region public properties

        public List<SerializedStatusIndicator> SerializedStatusIndicators
        {
            get { return m_serializedStatusIndicators; }
            private set
            {
                if (value != SerializedStatusIndicators)
                {
                    m_serializedStatusIndicators = value;
                }
            }
        }

        public List<string> SerializedLogs
        {
            get { return m_serializedLogs; }
            private set
            {
                if (value != SerializedLogs)
                {
                    m_serializedLogs = value;
                }
            }
        }

        public bool Initialized { get; private set; }

        #endregion

        #region public methods

        public void OnStkInitialized(SoundToolKitManager soundToolKitManager)
        {
            if (!Initialized)
            {
                SoundToolKitDebug.Assert(soundToolKitManager.StkAudioEngine != null, "Audio engine is not initialized. Logical error.");

                m_stkStatus = soundToolKitManager.StkAudioEngine.Control;

                SerializedLogs = SoundToolKitManager.Instance.UncacheLogs();
                SoundToolKitManager.Instance.OnLogAdded += OnLogEntryAdded;

                m_stkStatus.OnIndicatorAdded += (indicator, indicatorValue) =>
                {
                    SerializedStatusIndicators.Add(new SerializedStatusIndicator(indicator, indicatorValue));
                };

                m_stkStatus.OnIndicatorRemoved += (indicator) =>
                {
                    SerializedStatusIndicators.Remove(SerializedStatusIndicators.FirstOrDefault(
                        x => x.Name == indicator.Name));
                };

                SoundToolKitManager.Instance.ResourceContainer.Status = this;
                Initialized = true;
            }
        }

        #endregion

        #region private methods

        private void OnLogEntryAdded(SeverityLevel level, string message)
        {
            SerializedLogs.Add(message);
        }

        #region unity methods

        private void Awake()
        {
            if (SoundToolKitManager.Instance != null &&
                SoundToolKitManager.Instance.ResourceContainer != null &&
                SoundToolKitManager.Instance.ResourceContainer.Status != null)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Initialized = false;
                SoundToolKitSubscriber.SubscribeOnIntialized(soundToolKitObserver: this);
            }
        }

        private void OnDestroy()
        {
            foreach (var indicator in m_serializedStatusIndicators)
            {
                indicator.Native.Dispose();
            }

            if (m_stkStatus != null)
            {
                m_stkStatus.Dispose();
            }
        }

        #endregion

        #endregion

        #region private members

        private List<SerializedStatusIndicator> m_serializedStatusIndicators = new List<SerializedStatusIndicator>();

        private List<string> m_serializedLogs = new List<string>();

        private Control m_stkStatus;

        #endregion
    }
}
