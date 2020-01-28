/// \author Michal Majewski
/// \date 07.06.2019

namespace SoundToolKit.Unity.Utils
{
    /// <summary>
    /// Used only to ensure that the Lite version of the plugin works well with the Lite version of the engine.
    /// </summary>
    public static class StkLite
    {
        // <STK.LITE>
        // That method returns the number of sound sources available in the SoundToolKit Lite Unity Plugin.
        // Modifying this value may result in frequent crashes and instability of the plugin.
        // It will not allow the creation of more sound sources.
        //
        // To get the full version of SoundToolKit, visit the Asset Store:
        // https://assetstore.unity.com/packages/tools/audio/soundtoolkit-136305
        public static int AvailableSources()
        {
            return 3;
        }
        // </STK.LITE>
    }
}
