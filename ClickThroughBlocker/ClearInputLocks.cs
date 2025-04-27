using UnityEngine;

namespace ClickThroughFix
{
    public class ClearInputLocks
    {
        static internal bool focusFollowsclick
        {
            get
            {
                if (CTB.Instance == null)
                {
                    Debug.LogError("ClickThroughBlocker: Calling CTB.Instance outside of a loaded game.");
                    return false;
                }

                return CTB.Instance.focusFollowsclick;
            }
        }

        static internal bool universalClickBlocking
        {
            get { return CTB.Instance != null && CTB.Instance.universalClickBlocking; }
        }
    }
}
