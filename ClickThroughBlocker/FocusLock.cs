using System.Collections.Generic;

namespace ClickThroughFix
{
    internal class FocusLock
    {
        internal static Dictionary<string, FocusLock> focusLockDict = new Dictionary<string, FocusLock>();

        internal string lockName;
        internal long cnt;
        internal ClickThruBlocker.CTBWin win;

        internal FocusLock(string lockName, ClickThruBlocker.CTBWin win)
        {
            this.lockName = lockName;
            cnt = CBTGlobalMonitor.globalTimeTics;
            this.win = win;
        }

        internal static void SetLock(string lockName, ClickThruBlocker.CTBWin win, int i)
        {
            FocusLock focusLock;
            if (HighLogic.LoadedSceneIsEditor)
                EditorLogic.fetch.Lock(true, true, true, lockName);
            else
                InputLockManager.SetControlLock(ControlTypes.ALLBUTCAMERAS, lockName);

            if (focusLockDict.TryGetValue(lockName, out focusLock))
            {
                focusLock.cnt = CBTGlobalMonitor.globalTimeTics;
            }
            else
            {
                focusLockDict.Add(lockName, new FocusLock(lockName, win));
            }
        }
        internal static void FreeLock(string lockName, int i)
        {
            focusLockDict.Remove(lockName);
            // flight
            if (!HighLogic.LoadedSceneIsEditor)
                InputLockManager.RemoveControlLock(lockName);
            else
                EditorLogic.fetch.Unlock(lockName);
        }
    }

}
