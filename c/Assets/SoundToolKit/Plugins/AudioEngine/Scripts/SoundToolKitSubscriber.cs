/// \author Marcin Misiek
/// \date 10.10.2018

using SoundToolKit.Unity.Utils;

namespace SoundToolKit.Unity
{
    public static class SoundToolKitSubscriber
    {
        public static void SubscribeOnIntialized(ISoundToolKitObserver soundToolKitObserver)
        {
            var soundToolKitManager = SoundToolKitManager.Instance;
            SoundToolKitDebug.Assert(soundToolKitManager != null, "No instance of Manager exists.");

            if (soundToolKitManager.Initialized)
            {
                soundToolKitObserver.OnStkInitialized(soundToolKitManager);
            }
            else
            {
                soundToolKitManager.OnInitialized += () => soundToolKitObserver.OnStkInitialized(soundToolKitManager);
            }
        }
    }
}
