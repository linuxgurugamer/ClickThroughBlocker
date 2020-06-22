using System;
using System.Collections;
using UnityEngine;

namespace ClearAllInputLocks
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
        class SceneChangeCleanup : MonoBehaviour
    {
        void Start()
        {
            GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            GameEvents.onGUIApplicationLauncherReady.Add(onGUIApplicationLauncherReady);
            GameEvents.onLevelWasLoadedGUIReady.Add(onLevelWasLoadedGUIReady);
        }

        void onGameSceneLoadRequested(GameScenes gs)
        {
            ClickThroughFix.Log.Info("SceneChangeCleanup.onGameSceneLoadRequested");
            InputLockManager.ClearControlLocks();
            StartCoroutine(CleanupInputLocks());
        }

        void onGUIApplicationLauncherReady()
        {
            ClickThroughFix.Log.Info("SceneChangeCleanup.onGUIApplicationLauncherReady");
            StartCoroutine(CleanupInputLocks());
        }

        void onLevelWasLoadedGUIReady(GameScenes gs)
        {
            ClickThroughFix.Log.Info("SceneChangeCleanup.onLevelWasLoadedGUIReady");
            StartCoroutine(CleanupInputLocks());
        }



        IEnumerator  CleanupInputLocks()
        {
            yield return new WaitForSeconds(HighLogic.CurrentGame.Parameters.CustomParams<ClickThroughFix.CTB>().cleanupDelay);
            InputLockManager.ClearControlLocks();
            yield return null;
        }
    }
}
