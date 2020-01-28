/// \author Marcin Misiek
/// \date 18.07.2018

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    public class SoundToolKitSampleAssetLoader
    {
        #region public methods

        /// <summary>
        /// Creates the SoundToolKitSample assets for each AudioClip in the project and deletes StkSamples that have no corresponding AudioClip.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Samples/Synchronize")]
        public static void Synchronize()
        {
            var audioClips = AssetManager.FindAssets<AudioClip>();
            var soundToolKitSamples = AssetManager.FindAssets<SoundToolKitSample>();

            foreach (var audioClip in audioClips)
            {
                CreateAudioSample(audioClip);
            }

            var samplesToRemove = soundToolKitSamples.Where(x => x.AudioClip == null).ToList();
            samplesToRemove.ForEach(x => AssetManager.Delete(x));

            AssetManager.Update();
        }

        /// <summary>
        /// Samples Synchronization button should only be available in EditMode.
        /// </summary>
        [MenuItem("Tools/SoundToolKit/Samples/Synchronize", true)]
        public static bool ValidateSyncButton()
        {
            return !EditorApplication.isPlaying;
        }

        public static void CreateAudioSample(AudioClip audioClip)
        {
            SoundToolKitDebug.Assert(audioClip != null, "AudioClip is null");

            var soundToolKitSamples = AssetManager.FindAssets<SoundToolKitSample>();

            var audioSample = soundToolKitSamples.FirstOrDefault(x => x.name == audioClip.name);
            if (audioSample == null)
            {
                if (!AssetDatabase.IsValidFolder(AUDIO_SAMPLES_PATH))
                {
                    AssetDatabase.CreateFolder(AUDIO_SAMPLES_PARENT_PATH, AUDIO_SAMPLES_FOLDER_NAME);
                }

                audioSample = ScriptableObjectUtility.CreateAsset<SoundToolKitSample>(AUDIO_SAMPLES_PATH, audioClip.name);
            }

            audioSample.AudioClip = audioClip;

            EditorUtility.SetDirty(audioSample);

            Debug.Log("[SoundToolKit] AudioClip has been added; AudioSample: " + audioClip.name + " has been created in AudioSamples folder.");
        }

        #endregion

        #region private members

        private const string AUDIO_SAMPLES_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Assets\AudioSamples";
        private const string AUDIO_SAMPLES_PARENT_PATH = @"Assets\SoundToolKit\Plugins\AudioEngine\Assets";
        private const string AUDIO_SAMPLES_FOLDER_NAME = @"AudioSamples";

        #endregion
    }
}
#endif