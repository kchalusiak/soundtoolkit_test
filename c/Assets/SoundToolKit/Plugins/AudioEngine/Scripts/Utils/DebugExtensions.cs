using UnityEngine;

namespace SoundToolKit.Unity.Utils
{
    internal static class SoundToolKitDebug
    {
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, ASSERT_SOUNDTOOLKIT_PREFIX + " " + message);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition, ASSERT_SOUNDTOOLKIT_PREFIX);
        }

        public static void Log(string message)
        {
            Debug.Log(LOG_SOUNDTOOLKIT_PREFIX + " " + message);
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning(LOG_SOUNDTOOLKIT_PREFIX + " " + message);
        }

        public static void LogError(string message)
        {
            Debug.LogError(LOG_SOUNDTOOLKIT_PREFIX + " " + message);
        }

        public static void SetLogStackTrace()
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        }

        private const string ASSERT_SOUNDTOOLKIT_PREFIX = "[SoundToolKit.Assert]";

        private const string LOG_SOUNDTOOLKIT_PREFIX = "[SoundToolKit]";
    }
}
