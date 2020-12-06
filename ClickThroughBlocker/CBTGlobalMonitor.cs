using UnityEngine;


#if !DUMMY
namespace ClickThroughFix
{
    // This monitor deletes closed windows from the internal list
    // Needed for FocusFollowsWindow only

    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    class CBTGlobalMonitor : MonoBehaviour
    {
        void Start()
        {
            //DontDestroyOnLoad(this);
            // GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            FocusLock.focusLockDict.Clear();
        }

        void onGameSceneLoadRequested(GameScenes gs)
        {
            FocusLock.focusLockDict.Clear();
        }



        static internal long globalTimeTics = 0;
        void FixedUpdate()
        {
            globalTimeTics++;

            if (ClearInputLocks.focusFollowsclick)
            {
                foreach (var w in FocusLock.focusLockDict)
                {
                    if (w.Value.win.lastUpdated < globalTimeTics - 4)
                    {
                        FocusLock.FreeLock(w.Key, w.Value.win, 1);
                        w.Value.win.OnDestroy();
                        break;
                    }
                }
            }
        }
    }
}
#endif