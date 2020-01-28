/// \author Marcin Misiek
/// \date 18.07.2018

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    internal class SoundToolKitAssetPostprocessor : AssetPostprocessor
    {
        [MenuItem("Tools/SoundToolKit/AudioClips/AutoConversion/Enable")]
        public static void EnableConversion()
        {
            m_conversionEnabled = true;
        }

        [MenuItem("Tools/SoundToolKit/AudioClips/AutoConversion/Enable", true)]
        public static bool ValidateEnableButton()
        {
            return !m_conversionEnabled;
        }

        [MenuItem("Tools/SoundToolKit/AudioClips/AutoConversion/Disable")]
        public static void DisableConversion()
        {
            m_conversionEnabled = false;
        }

        [MenuItem("Tools/SoundToolKit/AudioClips/AutoConversion/Disable", true)]
        public static bool ValidateDisableButton()
        {
            return m_conversionEnabled;
        }

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                AudioClip audioClip = LoadAsset<AudioClip>(path);
                if (audioClip != null)
                {
                    SoundToolKitSampleAssetLoader.CreateAudioSample(audioClip);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        private static T LoadAsset<T>(string path) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
        }

        private void OnPreprocessAudio()
        {
            OverrideAudioClipSampleRate();
        }

        private void OverrideAudioClipSampleRate()
        {
            if (m_conversionEnabled)
            {
                var audioImporter = assetImporter as AudioImporter;

                AudioImporterSampleSettings settings = audioImporter.defaultSampleSettings;
                settings.sampleRateOverride = STK_REQUIRED_SAMPLE_RATE;
                settings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;

                audioImporter.defaultSampleSettings = settings;

                Debug.Log("[SoundToolKit] Sample rate of AudioClips is automatically overriden by STK requested sample rate: " 
                   + STK_REQUIRED_SAMPLE_RATE + " Hz."); 
            }
            else
            {
                Debug.Log("[SoundToolKit] Sample rate of AudioClips not automatically overriden. STK requires" 
                   + STK_REQUIRED_SAMPLE_RATE + 
                   " Hz sample rate. Override manually or enable automatic overriding to use AudioClips in STK.");
            }
        }

        #region private members

        private const uint STK_REQUIRED_SAMPLE_RATE = 48000;
        private static bool m_conversionEnabled = true;

        #endregion
    }
}
#endif