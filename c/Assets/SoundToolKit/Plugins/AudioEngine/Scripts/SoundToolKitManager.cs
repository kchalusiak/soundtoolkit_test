/// \author Marcin Misiek
/// \date 26.04.2018

#pragma warning disable 0414

using SoundToolKit.Extensions.Unity;
using SoundToolKit.Unity.Utils;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoundToolKit.Unity
{
    // [Reasoning for usage of singleton]:
    // This class is a singleton because most scripts of our plugin use functionality of SoundToolKitManager.
    // Additionally there should be one SoundToolKitManager in a game.
    // Instead of using Find each time we want use SoundToolKitManager we are using SoundToolKitManager.Instance.
    // We don't have to worry about two AudioEngines in the scene and this object will also prevail on the scene change.

    /// <summary>
    /// Singleton class which governs the resources and lifetime of SoundToolKit engine.
    /// Allows controlling several output volume levels of STK engine and synchronizing the commands issued to it.
    /// </summary>
    [AddComponentMenu("SoundToolKit/DefaultPrefabComponents/SoundToolKitManager")]
    public sealed class SoundToolKitManager : Singleton<SoundToolKitManager>
    {
        #region editor fields

        [SerializeField]
        private SoundToolKitSettings m_settingsInspector = null;

        [SerializeField]
        private bool m_hrtfEnabled = true;

        [SerializeField]
        private float m_masterVolume = 1.0f;

        #endregion

        #region public events

        /// <summary>
        /// This event will be fired on Initialization.
        /// </summary>
        public event Action OnInitialized;

        public event Action<SeverityLevel, string> OnLogAdded;

        #endregion

        #region public properties

        public bool Initialized
        {
            get { return StkAudioEngine != null; }
        }

        /// <summary>
        /// A container that holds reference to resurces used by SoundToolKit.
        /// </summary>
        public ResourcesContainer ResourceContainer { get; private set; }

        /// <summary>
        /// SoundToolKitSettings that determine the low-level processing details of SoundToolKit.
        /// Can be swapped at runtime.
        /// </summary>
        public SoundToolKitSettings Settings
        {
            get { return m_settings; }
            set
            {
                if (value != Settings && value != null)
                {
                    m_settings = value;
                    m_settingsInspector = m_settings;

                    StkAudioEngine.Control.OnSettingAdded += (setting, currentValue) =>
                    {
                        m_settings.StkSettings.Add(setting);
                        m_settings.UpdateSetting(setting);
                    };
                }
            }
        }

        /// <summary>
        /// Controls global HRTF support.
        /// </summary>
        public bool HrtfEnabled
        {
            get { return m_hrtfEnabled; }
            set
            {
                m_hrtfEnabled = value;
                StkAudioEngine.AudioOutput.HrtfEnabled = m_hrtfEnabled;
            }
        }

        /// <summary>
        /// The output volume of SoundToolKit.
        /// </summary>
        public float MasterVolume
        {
            get { return m_masterVolume; }
            set
            {
                m_masterVolume = value;
                StkAudioEngine.AudioOutput.Volume = m_masterVolume;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// This will start SoundToolKit processing.
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            if (StkAudioEngine != null)
            {
                m_audioBridge.AudioOutput = StkAudioEngine.AudioOutput;
                m_paused = false;
            }
        }

        /// <summary>
        /// This will disable SoundToolKit processing.
        /// </summary>
        [ContextMenu("Pause")]
        public void Pause()
        {
            if (StkAudioEngine != null)
            {
                m_paused = true;
                m_audioBridge.InvalidateAudioOutput();
            }
        }

        /// <summary>
        /// This method will synchronize each command send to SoundToolKit.
        /// </summary>
        public void UpdateAudio()
        {
            if (StkAudioEngine.Good)
            {
                StkAudioEngine.AdvanceTime((ulong)(Time.deltaTime * 1000) + 1);
                StkAudioEngine.Flush();
            }
            else if (!StkAudioEngine.Good && StkAudioEngine.Valid)
            {
                if (m_reinitializeCounter <= MAX_REINITIALIZE_COUNT)
                {
                    ReInitialize();
                }
                else
                {
                    SoundToolKitDebug.LogError("Manager crashed. Internal issue.");
                }
            }
        }

        /// <summary>
        /// This method will force execution of each command send to SoundToolKit prior to it's call.
        /// Note: Calling this while in game may cause freezes as it enforces waiting for all commands to execute.
        /// </summary>
        public void Finish()
        {
            StkAudioEngine.Finish();
        }

        #endregion

        #region internal properties

        internal Audio StkAudioEngine { get; private set; }

        #endregion

        #region internal methods

        internal List<string> UncacheLogs()
        {
            var cachedLogs = m_cachedLogs ?? new List<string>();
            m_cachedLogs.Clear();

            return cachedLogs;
        }

        #endregion

        #region private constructor

        private SoundToolKitManager() { }

        #endregion

        #region private methods

        private void Initialize()
        {
            m_stkTaskScheduler = new SoundToolKitTaskScheduler();

            if (StkAudioEngine == null)
            {
                InitializeAudioEngine();
            }

            InitializeAudioBridge();

            ResourceContainer = new ResourcesContainer();

            if (m_settingsInspector == null)
            {
                Settings = ScriptableObject.CreateInstance<SoundToolKitSettings>();
            }

            UpdateProperties();

            SoundToolKitDebug.SetLogStackTrace();

#if UNITY_EDITOR
#if !UNITY_5 && !UNITY_2017_1
            EditorApplication.pauseStateChanged += EditorApplication_pauseStateChanged;
#else
            EditorApplication.playmodeStateChanged += EditorApplication_playmodeStateChanged;
#endif
#endif
            SoundToolKitDebug.Log("Sound enabled.");

            // NOTE: On audioEngine start it will automatically play sound.
            Play();
        }

        private void ReInitialize()
        {
            SoundToolKitDebug.LogWarning("Manager became unusable. Trying to re-initialize.");

            //TODO: [Not Tested]: This should be tested when AudioEngine is not Good.
            m_audioBridge.Dispose();
            StkAudioEngine.Dispose();

            //Re-Initialize
            Initialize();

            m_reinitializeCounter++;
        }

        private void InitializeAudioEngine()
        {
            try
            {
                m_stkThreadPoolHandler = (taskData, task) =>
                {
                    return (ulong)m_stkTaskScheduler.Schedule(() => task(taskData));
                };
                m_stkThreadPool = SoundToolKitBuilder.CreateCustomThreadPool(m_stkThreadPoolHandler);

                StkAudioEngine = SoundToolKitBuilder.CreateSoundToolKit(
                    new SoundToolKitBuilder.ThreadPoolRequest
                    {
                        CustomThreadPool = m_stkThreadPool
                    }, null, null);

                var resourcesDirectory = Path.Combine(Application.streamingAssetsPath, @"SoundToolKit\Resources\");

                var diffractionData = File.ReadAllBytes(resourcesDirectory + "diffraction_table_100_20.bin");
                var hrtfData = File.ReadAllBytes(resourcesDirectory + "bqc_hrtf.bin");

                StkAudioEngine.SetDiffractionData(diffractionData);
                StkAudioEngine.AudioOutput.SetHrtfData(hrtfData);

                StkAudioEngine.AudioOutput.HrtfEnabled = true;
            }
            catch (Exception e)
            {
                SoundToolKitDebug.LogError("Message: " + e.Message + "\nStack Trace: " + e.InnerException);
            }
        }

        private void InitializeAudioBridge()
        {
            try
            {
                m_audioBridge = new AudioBridge(StkAudioEngine) { AudioOutput = StkAudioEngine.AudioOutput };
                AudioSettings.OnAudioConfigurationChanged += AudioSettings_OnAudioConfigurationChanged;

                AudioSettings_OnAudioConfigurationChanged(false);

                m_soundToolKitMixer = (AudioMixer)Resources.Load(MIXER_PATH);
            }
            catch (Exception e)
            {
                SoundToolKitDebug.LogError("Message: " + e.Message + "\nStack Trace: " + e.InnerException);
            }
        }

        private Dictionary<string, string> ExtractSettingsInitializer(SoundToolKitSettings settings)
        {
            var initializer = new Dictionary<string, string>();

            settings.SerializedSettings.ForEach(x =>
            {
                initializer.Add(x.Name, x.CurrentValue);
            });

            return initializer;
        }

        private void AudioSettings_OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            StkAudioEngine.AudioOutput.SampleRate = AudioSettings.outputSampleRate;
            StkAudioEngine.AudioOutput.OutputSpeakersType = SpeakersExtensions.FromUnity(AudioSettings.speakerMode);
        }

        private void SavePreviousState()
        {
            m_isPlayingPrevious = !m_paused;
        }

        private void Resume()
        {
            if (m_isPlayingPrevious)
            {
                Play();
            }
        }

        #region unity methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // If manager already exists, as in after scene change or scene reload
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);

                Initialize();

                if (OnInitialized != null)
                {
                    OnInitialized();
                }
            }
        }

        private void Start()
        {
            if (ResourceContainer.Geometry == null)
            {
                SoundToolKitDebug.LogWarning("No SoundToolKitGeometry component on the scene. Acoustic Meshes will not be loaded. " +
                    "Default component prefab available in Assets/SoundToolKit/Plugins/AudioEngine/Prefabs. " +
                    "You can also use menu Tools/SoundToolKit/Geometry/SynchronizeDefaultGeometry.");
            }

            if (ResourceContainer.AudioListener == null)
            {
                SoundToolKitDebug.LogWarning("Missing SoundToolKitListener on the scene.");
            }

            if (!m_audioBridge.ProcessCallbackActive)
            {
                SoundToolKitDebug.LogWarning("Audio bridge is not processing. Missing audio mixer on scene.");
            }
        }

        private void OnDestroy()
        {
            if (Initialized)
            {
                m_paused = true;

                m_audioBridge.Dispose();
                StkAudioEngine.Dispose();
                m_stkTaskScheduler.Stop();

                if (ResourceContainer.Geometry != null)
                {
                    ResourceContainer.Geometry.ClearGeometry();
                }
            }
        }

        // Note: This will be called each Time.fixedDeltaTime
        private void FixedUpdate()
        {
            UpdateAudio();
        }

        private void OnDisable()
        {
            if (Initialized)
            {
                SavePreviousState();

                Pause();
                UpdateAudio();
            }
        }

        private void OnEnable()
        {
            Resume();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateProperties();
        }
#endif

        #endregion

        private void UpdateProperties()
        {
            if (Initialized)
            {
                Settings = m_settingsInspector;
                HrtfEnabled = m_hrtfEnabled;
                MasterVolume = m_masterVolume;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SavePreviousState();
                Pause();
                UpdateAudio();
            }
            else
            {
                Resume();
            }
        }

#if UNITY_EDITOR
#if !UNITY_5 && !UNITY_2017_1
        private void EditorApplication_pauseStateChanged(PauseState obj)
        {
            switch (obj)
            {
                case PauseState.Paused:
                    SavePreviousState();

                    Pause();
                    UpdateAudio();
                    break;

                case PauseState.Unpaused:
                    Resume();
                    break;
            }
        }
#else
        private void EditorApplication_playmodeStateChanged()
        {
            if (EditorApplication.isPaused)
            {
                SavePreviousState();
                Pause();
                UpdateAudio();
            }
            else if (EditorApplication.isPlaying && !EditorApplication.isPaused)
            {
                Resume();
            }
        }
#endif
#endif
        #endregion

        #region private members

        private const string MIXER_PATH = "STKMixer/SoundToolKit";

        private const int MAX_REINITIALIZE_COUNT = 5;

        private int m_reinitializeCounter = 0;

        private SoundToolKitSettings m_settings;

        private AudioMixer m_soundToolKitMixer;

        private AudioBridge m_audioBridge;

        private bool m_isPlayingPrevious;

        private List<string> m_cachedLogs = new List<string>();

        private bool m_paused = true;

        private ThreadPool m_stkThreadPool;

        private Func<IntPtr, Action<IntPtr>, ulong> m_stkThreadPoolHandler;

        private SoundToolKitTaskScheduler m_stkTaskScheduler;

        #endregion
    }
}
