using ClickThroughFix;
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
        void OnDestroy()
        {
            GameEvents.onGameSceneLoadRequested.Remove(onGameSceneLoadRequested);
            GameEvents.onGUIApplicationLauncherReady.Remove(onGUIApplicationLauncherReady);
            GameEvents.onLevelWasLoadedGUIReady.Remove(onLevelWasLoadedGUIReady);
        }

        bool ongameSceneLoadRequestedCalled = false;
        void onGameSceneLoadRequested(GameScenes gs)
        {
            if (!ongameSceneLoadRequestedCalled)
            {
                ongameSceneLoadRequestedCalled = true;
                InputLockManager.ClearControlLocks();
            }
        }

        bool onGUIApplicationLauncherReadyCalled = false;
        void onGUIApplicationLauncherReady()
        {
            if (!onGUIApplicationLauncherReadyCalled)
            {
                onGUIApplicationLauncherReadyCalled = true;
                if (!isRunning)
                    StartCoroutine("CleanupInputLocks");
            }
        }

        bool onLevelWasLoadedGUIReadycalled = false;
        void onLevelWasLoadedGUIReady(GameScenes gs)
        {
            if (!onLevelWasLoadedGUIReadycalled)
            {
                onLevelWasLoadedGUIReadycalled = true;
                if (!isRunning)
                    StartCoroutine("CleanupInputLocks");
            }
        }

        void StopStartCoroutine()
        {
            StopCoroutine("CleanupInputLocks");
            StartCoroutine("CleanupInputLocks");
        }

        bool isRunning = false;
        IEnumerator  CleanupInputLocks()
        {
            //Log.Info("CleanUpInputLocks entry");
            isRunning = true;
            yield return new WaitForSeconds(HighLogic.CurrentGame.Parameters.CustomParams<ClickThroughFix.CTB>().cleanupDelay);
            InputLockManager.ClearControlLocks();
            yield return null;
            isRunning = false;
            //Log.Info("CleanUpInputLocks exit");
        }
    }
}
