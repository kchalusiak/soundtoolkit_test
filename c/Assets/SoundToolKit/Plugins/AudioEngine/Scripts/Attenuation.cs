/// \author Michal Majewski
/// \date 16.04.2019

namespace SoundToolKit.Unity
{
    /// <summary>
    /// Describes an attenuation model for the sound emitted by a sound source
    /// </summary>
    public enum SoundAttenuation
    {
        Nothing = 0,
        PointSource = 1,          // Inverse square of distance law (-6dB/doubling the distance)
        LineSource = 2,           // Inverse distance law (-3dB/doubling distance)
        Linear = 3,               // Linearly from source radius to maxDistance
        Logarithmic = 4,          // Logarithmic reduction of volume in distance
        Inverse = 5,              // A very steep curve in the shape of a hyperbole
        ReverseLog = 6,           // A reverse logarithmic reduction
        Custom = 7                // Defined by a graphical curve
    };
}
