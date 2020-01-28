/// \author Marcin Misiek
/// \date 20.06.2018

using System.Threading;

namespace SoundToolKit.Unity.Utils
{
    /// <summary>
    /// Thread safe enter once into a code block:
    /// the first call to CheckAndSetFirstCall returns always true,
    /// all subsequent call return false.
    /// </summary>
    public class ThreadSafeSingleShotGuard
    {
        #region public properties

        /// <summary>Explicit call to check and set if this is the first call</summary>
        public bool CheckAndSetFirstCall
        {
            get { return Interlocked.Exchange(ref m_state, (int)State.Called) == (int)State.NotCalled; }
        }

        #endregion

        private enum State
        {
            NotCalled = 0,
            Called = 1
        }

        #region private members

        private int m_state = (int)State.NotCalled;

        #endregion
    }
}
