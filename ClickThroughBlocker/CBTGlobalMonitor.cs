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

            if (HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick)
            {
                if (FocusLock.focusLockDict.Count > 0)
                //Log.Info("CBTGlobalMonitor.FixedUpate, focusFollowsClick, count: " + ClickThruBlocker.focusLockDict.Count + ", timeTics: " + timeTics);
                foreach (var w in FocusLock.focusLockDict)
                {
                   // Log.Info("CBTGlobalMonitor.FixedUpate,  lockName: " + w.Key + ", lastUpdated: " + w.Value.win.lastUpdated);
                    if (w.Value.win.lastUpdated < globalTimeTics - 4)
                    {
                            Log.Info("CBTGlobalMonitor.FixedUpate, FreeLock  lockName: " + w.Key + ", lastUpdated: " + w.Value.win.lastUpdated);
                            FocusLock.FreeLock(w.Key,1);
                            w.Value.win.OnDestroy();
                            break;
                    }
                }
            }
        }
    }
}
#endif