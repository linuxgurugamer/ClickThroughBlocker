using UnityEngine;

namespace ClickThroughFix
{
#if !DUMMY
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    internal class OnGUILoopCount : MonoBehaviour
    {
        static long onguiCnt = 0;


        internal static long GetOnGUICnt()
        {
            return onguiCnt;
        }
        private void Start()
        {
            DontDestroyOnLoad(this);
            InvokeRepeating("DoGuiCounter", 5.0f, 0.25f);
        }

        long lastonGuiCnt;
        private void DoGuiCounter()
        {
            if (HighLogic.CurrentGame == null || ClearInputLocks.focusFollowsclick)
                return;

            lastonGuiCnt = (onguiCnt++) - 1;

            foreach (var win in ClickThruBlocker.winList.Values)
            {
                if (win.lastLockCycle < lastonGuiCnt)
                {
                    //Log.Info("lastonGuiCnt: " + lastonGuiCnt + "lastLockCycle: " + win.lastLockCycle);
                    {
                        if (EditorLogic.fetch != null && win.weLockedEditorInputs)
                        {
                            EditorLogic.fetch.Unlock(win.lockName);
                            win.weLockedEditorInputs = false;
                            ClickThruBlocker.CTBWin.activeBlockerCnt--;
                        }
                        if (win.weLockedFlightInputs && win.lockName != null)
                        {
                            InputLockManager.RemoveControlLock(win.lockName);
                            win.weLockedFlightInputs = false;
                        }
                    }
                }
            }

        }
    }
#endif

}
