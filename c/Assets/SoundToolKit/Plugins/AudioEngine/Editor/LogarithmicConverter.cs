/// \author Marcin Misiek
/// \date 08.10.2018

#if UNITY_EDITOR

using System;

namespace SoundToolKit.Unity.Editor
{
    public static class LogarithmicConverter
    {
        #region public constructors

        public static float ConvertToLinear(float decibels)
        {
            return Convert.ToSingle(Math.Pow(10.0f, decibels * 0.05f));
        }

        public static float ConvertToDecibels(float linear)
        {
            return Convert.ToSingle(20.0f * Math.Log10(linear));
        }

        #endregion
    }
}

#endif