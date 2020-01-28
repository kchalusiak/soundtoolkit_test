/// \author Marcin Misiek
/// \date 10.10.2018

namespace SoundToolKit.Unity
{
    public interface ISoundToolKitObserver
    {
        bool Initialized { get; }

        void OnStkInitialized(SoundToolKitManager soundToolKitManager);
    }
}
