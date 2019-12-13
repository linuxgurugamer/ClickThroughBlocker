
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

#if !DUMMY
namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class CBTMonitor : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this);
            GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
        }

        void onGameSceneLoadRequested(GameScenes gs)
        {
            ClickThruBlocker.CTBWin.activeBlockerCnt = 0;
        }

        // this whole mess below is to work around a stock bug.
        // The bug is that the editor ignores the lock when the Action Group pane is show.
        // So we have to each time, clear all locks and then reset those which were active when 
        // the mouse moved over a protected window
        void Update()
        {
            if (HighLogic.LoadedSceneIsEditor )
            {
                if (ClickThruBlocker.CTBWin.activeBlockerCnt > 0)
                {
                    //Log.Info("Setting Mouse.HoveredPart to null & deselecting all parts");
                    Mouse.HoveredPart = null;
                    Log.Info("CBTMonitor.Update 2.1");
                    if (EditorLogic.fetch == null)
                    {
                        Log.Info("EditorLogic.fetch == null");
                        return;
                    }
 
                    for (int i = EditorLogic.fetch.ship.Parts.Count - 1; i >= 0; i--)
                    //for (int i = 0; i < EditorLogic.fetch.ship.Parts.Count; i++)
                    {
                        EditorActionPartSelector selector = EditorLogic.fetch.ship.Parts[i].GetComponent<EditorActionPartSelector>();
                        if (selector != null)
                            selector.Deselect();
                    }

                    if (EditorActionGroups.Instance != null)
                    {
                        EditorActionGroups.Instance.ClearSelection(true);
                        for (int i = ClickThruBlocker.CTBWin.selectedParts.Count - 1; i>= 0; i--)
                        //for (int i = 0; i < ClickThruBlocker.CTBWin.selectedParts.Count; i++)
                        {
                            EditorActionPartSelector selector = ClickThruBlocker.CTBWin.selectedParts[i].GetComponent<EditorActionPartSelector>();
                            if (selector != null)
                                EditorActionGroups.Instance.AddToSelection(selector);
                        }
                    }
                }

            }
        }

        static internal long timeTics = 0;
        int d;
        void LateUpdate()
        {
            d = 0;
            ClickThruBlocker.CTBWin win = null;

            timeTics++;

            foreach (var w in  ClickThruBlocker.winList)
            {
                if (w.Value.lastUpdated+4  < timeTics ) //+ 0.05 < Planetarium.GetUniversalTime())
                {
                    d = w.Key;
                    win = w.Value;
                    break;
                }
            }
            if (d  != 0)
            {
                win.OnDestroy();
            }
        }
    }
}
#endif