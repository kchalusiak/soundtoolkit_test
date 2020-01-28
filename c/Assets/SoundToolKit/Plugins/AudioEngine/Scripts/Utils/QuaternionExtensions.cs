/// \author Marcin Misiek
/// \date 26.04.2018

using UnityEngine;


namespace SoundToolKit.Unity.Utils
{
    internal static class QuaternionExtensions
    {
        public static Quaternion ToUnityEngine(this SoundToolKit.Numerics.Quaternion rotation)
        {
            return new Quaternion { x = rotation.X, y = rotation.Y, z = rotation.Z, w = rotation.W };
        }

        public static SoundToolKit.Numerics.Quaternion ToNumerics(this Quaternion rotation)
        {
            return new SoundToolKit.Numerics.Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        }
    }
}
