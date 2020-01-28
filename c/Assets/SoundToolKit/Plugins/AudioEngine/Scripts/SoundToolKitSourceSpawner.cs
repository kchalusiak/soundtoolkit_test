/// \author Michal Majewski
/// \date 21.11.2018

using SoundToolKit.Unity.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundToolKit.Unity
{
    /// <summary>
    /// SourceSpawner creates fire-and-forget SpatialSources that play a given SoundToolKitSample 
    /// at a given location and then dispose of themselves.
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Transform))]
    [AddComponentMenu("SoundToolKit/SoundToolKitSourceSpawner")]
    public class SoundToolKitSourceSpawner : MonoBehaviour
    {
        #region editor fields

        [SerializeField]
        private List<VolumeControlledSample> m_samples;

        [SerializeField]
        private float m_volume = 0.75f;

        [SerializeField]
        private float m_simulationQuality = 1f;

        [SerializeField]
        private bool m_hrtfSpatializationEnabled = true;

        [SerializeField]
        protected bool m_attenuationEnabled = true;

        [SerializeField]
        protected bool m_reflectionEnabled = true;

        [SerializeField]
        protected bool m_scatteringEnabled = true;

        [SerializeField]
        protected bool m_transmissionEnabled = true;

        [SerializeField]
        protected bool m_diffractionEnabled = true;

        [SerializeField]
        protected bool m_reverbEnabled = true;

        [SerializeField]
        private SoundAttenuation m_attenuation = SoundAttenuation.LineSource;

        [SerializeField]
        private float m_minDistance = 0.25f;

        [SerializeField]
        private float m_maxDistance = 300;

        #endregion

        #region public properties

        /// <summary>
        /// List of VolumeControlledSamples that can be played by the spawned SoundToolKitSpatialSource
        /// </summary>
        public List<VolumeControlledSample> Samples
        {
            get { return m_samples; }
            set { m_samples = value; }
        }

        /// <summary>
        /// Volume of the sound that will be played
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set { m_volume = value; }
        }

        /// <summary>
        /// This currently has no effect.
        /// Will be added in the future releases.
        /// Determines the quality of source processing. The value affects how many engine resources are delegated for processing sound emitted by this source. 
        ///	The simulation quality also determines the priority of given source, meaning that sources with higher simulation quality can have lower latency. 
        ///	Quality in range 0 to 1.0, where:
        ///		1.0 - the best possible quality
        ///		0.0 - source not processed at all
        /// </summary>
        public float SimulationQuality
        {
            get { return m_simulationQuality; }
            set { m_simulationQuality = value; }
        }

        /// <summary>
        /// Enables/Disables HRTF processing of this Sound Source.
        /// </summary>
        public bool HrtfSpatializationEnabled
        {
            get { return m_hrtfSpatializationEnabled; }
            set { m_hrtfSpatializationEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables Attenuation with distance effect on this Sound Source.
        /// </summary>
        public bool AttenuationEnabled
        {
            get { return m_attenuationEnabled; }
            set { m_attenuationEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables geometric Reflection effect on this Sound Source.
        /// </summary>
        public bool ReflectionEnabled
        {
            get { return m_reflectionEnabled; }
            set { m_reflectionEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables geometric Scattering effect on this Sound Source.
        /// </summary>
        public bool ScatteringEnabled
        {
            get { return m_scatteringEnabled; }
            set { m_scatteringEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables geometric Transmission effect on this Sound Source.
        /// </summary>
        public bool TransmissionEnabled
        {
            get { return m_transmissionEnabled; }
            set { m_transmissionEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables geometric Diffraction effect on this Sound Source.
        /// </summary>
        public bool DiffractionEnabled
        {
            get { return m_diffractionEnabled; }
            set { m_diffractionEnabled = value; }
        }

        /// <summary>
        /// Enables/Disables late reverb processing of this Sound Source.
        /// </summary>
        public bool ReverbEnabled
        {
            get { return m_reverbEnabled; }
            set { m_reverbEnabled = value; }
        }

        /// <summary>
        ///	The attenuation model which should be used by the spawned sources
        /// </summary>
        public SoundAttenuation Attenuation
        {
            get { return m_attenuation; }
            set { m_attenuation = value; }
        }

        /// <summary>
        ///	Radius of virtual sphere that simulates boundary of source volume.
        ///	This means that below this distance from source attenuation will not occur.
        ///	Above that distance attenuation will be calculated according to chosen attenuation curve (see setAttenuation)
        /// </summary>
        public float MinDistance
        {
            get { return m_minDistance; }
            set { m_minDistance = Math.Max(value, 0.001f); }
        }

        /// <summary>
        /// Max distance of virtual sphere that simulates boundary of source volume
        /// This is only valid in Linear and ReverseLog Attenuation curve.
        /// </summary>
        public float MaxDistance
        {
            get { return m_maxDistance; }
            set { m_maxDistance = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Spawn a SoundToolKitSpatialSource playing a single given SoundToolKitSample at a given location.
        /// The default position is that of component's parent GameObject.
        /// </summary>
        /// <param name="sampleToPlay"> 
        /// The VolumeControlledSample to be played by the spawned source. Pass an element of the Samples List or a VolumeControlledSample object.
        /// </param>
        /// <param name="positionOfSpawn"> 
        /// Position at which the Source is spawned. Defaults to the parent GameObject's position.
        /// </param>
        public void SpawnSound(VolumeControlledSample sampleToPlay, Vector3? positionOfSpawn = null)
        {
            if (sampleToPlay != null && sampleToPlay.Sample != null)
            {
                var spawnPosition = DeterminePosition(positionOfSpawn);

                var sourceObject = new GameObject("SpawnedSoundSource");
                sourceObject.transform.position = spawnPosition;
                SourceObjects.Add(sourceObject);

                var source = sourceObject.AddComponent<SoundToolKitSpatialSource>();
                SetupSource(source);

                PlayOneShot(source, sampleToPlay);

                StartCoroutine(TimedDestroy(sourceObject, sampleToPlay.Sample.AudioClip.length));
            }
            else
            {
                WarnSampleInvalid();
            }
        }

        /// <summary>
        /// Spawn a SoundToolKitSpatialSource playing a random SoundToolKitSample from the Samples list at a given position.
        /// The default position is that of component's parent GameObject.
        /// </summary>
        /// <param name="positionOfSpawn"> 
        /// Position at which the Source is spawned. Defaults to the parent GameObject's position.
        /// </param>
        public void SpawnRandomSound(Vector3? positionOfSpawn = null)
        {
            if (m_samples.Any() && !m_samples.Exists(x => x.Sample == null))
            {
                var spawnPosition = DeterminePosition(positionOfSpawn);

                var sourceObject = new GameObject("SpawnedSoundSource");
                sourceObject.transform.position = spawnPosition;
                SourceObjects.Add(sourceObject);

                var source = sourceObject.AddComponent<SoundToolKitSpatialSource>();
                SetupSource(source);

                var randomSample = Samples[UnityEngine.Random.Range(0, Samples.Count - 1)];
                PlayOneShot(source, randomSample);

                StartCoroutine(TimedDestroy(sourceObject, randomSample.Sample.AudioClip.length));
            }
            else
            {
                WarnSampleInvalid();
            }
        }

        /// <summary>
        /// Destroys all of the SpatialSources spawned by this Spawner immediately.
        /// </summary>
        public void KillAllSpawnedSources()
        {
            SourceObjects.ForEach(x =>
            {
                var soundSource = x.GetComponent<SoundToolKitSpatialSource>();
                var playback = soundSource.Playbacks.Last();

                playback.OnEnded -= OnPlaybackEnded;
                OnPlaybackEnded(playback);

                SourceObjects.Remove(x);
                Destroy(x);
            });
        }

        #endregion

        #region internal properties

        internal List<GameObject> SourceObjects
        {
            get { return m_sourceObjects; }
            private set { m_sourceObjects = value; }
        }

        #endregion

        #region private methods

        private Vector3 DeterminePosition(Vector3? optionalPosition)
        {
            return optionalPosition ?? gameObject.transform.position;
        }

        private void SetupSource(SoundToolKitSpatialSource source)
        {
            source.Volume = Volume;
            source.SimulationQuality = SimulationQuality;
            source.HrtfSpatializationEnabled = HrtfSpatializationEnabled;
            source.AttenuationEnabled = AttenuationEnabled;
            source.ReflectionEnabled = ReflectionEnabled;
            source.ScatteringEnabled = ScatteringEnabled;
            source.TransmissionEnabled = TransmissionEnabled;
            source.DiffractionEnabled = DiffractionEnabled;
            source.ReverbEnabled = ReverbEnabled;
            source.Attenuation = Attenuation;
            source.MaxDistance = MaxDistance;
            source.MinDistance = MinDistance;
        }

        private void WarnSampleInvalid()
        {
            SoundToolKitDebug.LogWarning("Attempted to play invalid VolumeControlledSample in " +
                gameObject.name + " GameObject's SourceSpawner. Check it's Samples List.");
        }

        private void PlayOneShot(SoundToolKitSpatialSource source, VolumeControlledSample sample)
        {
            source.Playbacks.Add(new SoundToolKitPlayback(sample.Sample, sample.Volume));
            source.Play();
            source.Playbacks.Last().OnEnded += OnPlaybackEnded;
        }

        private void OnPlaybackEnded(SoundToolKitPlayback playback)
        {
            playback.Playback = null;
        }

        private IEnumerator TimedDestroy(GameObject sourceObject, float timeToDestroy)
        {
            yield return new WaitForSeconds(timeToDestroy);

            SourceObjects.Remove(sourceObject);
            Destroy(sourceObject);
        }

        #region unity methods

        private void Awake()
        {
            // <STK.LITE>
            // Warning: Modifying this code will result in an undefined behaviour.
            // This is the SoundToolKit Lite version.
            // SourceSpawner component is not allowed in that version.
            // To obtain a full version, visit https://assetstore.unity.com/packages/tools/audio/soundtoolkit-136305
            SoundToolKitDebug.Log("SourceSpawner is not available in the SoundToolKit Lite version. " +
                "Deleting the illegal SourceSpawner component.");
            Destroy(this);
            // </STK.LITE>

            if (m_samples == null)
            {
                m_samples = new List<VolumeControlledSample>();
            }

            SourceObjects = new List<GameObject>();
        }

        private void OnDestroy()
        {
            KillAllSpawnedSources();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Samples = m_samples;

            if (Samples != null)
            {
                Samples.ForEach(x => x.OnValidate());
            }
        }
#endif

        #endregion

        #endregion

        #region private members

        [NonSerialized]
        private List<GameObject> m_sourceObjects;

        #endregion
    }
}
