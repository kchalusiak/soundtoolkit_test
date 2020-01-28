/// \author Marcin Misiek
/// \date 26.04.2018

using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    internal static class Vector3Extensions
    {
        public static Vector3 ToUnityEngine(this SoundToolKit.Numerics.Vector3 v)
        {
            return new Vector3 { x = v.X, y = v.Y, z = v.Z };
        }

        public static SoundToolKit.Numerics.Vector3 ToNumerics(this Vector3 v)
        {
            return new SoundToolKit.Numerics.Vector3(v.x, v.y, v.z);
        }
    }
}
