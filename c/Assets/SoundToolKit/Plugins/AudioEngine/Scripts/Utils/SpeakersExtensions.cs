/// \author Marcin Misiek
/// \date 24.05.2018

using System;
using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    internal static class SpeakersExtensions
    {
        public static SpeakersConfiguration FromUnity(this AudioSpeakerMode audioSpeakerMode)
        {
            switch (audioSpeakerMode)
            {
                case AudioSpeakerMode.Mono:
                    return SpeakersConfiguration.Mono;
                case AudioSpeakerMode.Stereo:
                    return SpeakersConfiguration.Headphones;
                case AudioSpeakerMode.Mode5point1:
                    return SpeakersConfiguration.Speakers_5_1;
                case AudioSpeakerMode.Mode7point1:
                    return SpeakersConfiguration.Speakers_7_1;
                default:
                    throw new NotImplementedException("Unsupported speaker mode");
            }
        }
    }
}
