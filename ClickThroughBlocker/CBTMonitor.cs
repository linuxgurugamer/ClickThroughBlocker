using UnityEngine;
using System.Collections.Generic;
using KSP.UI.Screens;


#if !DUMMY
namespace ClickThroughFix
{
    internal class PartEAPS
    {

        internal uint persistentId;
        internal Part p;
        internal EditorActionPartSelector eaps;

        internal PartEAPS(Part p)
        {
            this.persistentId = p.persistentId;
            this.p = p;
            this.eaps = p.GetComponent<EditorActionPartSelector>();
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class CBTMonitor : MonoBehaviour
    {
        static internal Dictionary<uint, PartEAPS> partEAPSDict;

        void Start()
        {
            DontDestroyOnLoad(this);
            GameEvents.onGameSceneLoadRequested.Add(onGameSceneLoadRequested);
            GameEvents.onEditorShipModified.Add(_onEditorShipModified);
            GameEvents.onLevelWasLoadedGUIReady.Add(onLevelWasLoadedGUIReady);

            ClickThruBlocker.CTBWin.activeBlockerCnt = 0;
            partEAPSDict = new Dictionary<uint, PartEAPS>();
        }

        void onGameSceneLoadRequested(GameScenes gs)
        {
            ClickThruBlocker.CTBWin.activeBlockerCnt = 0;
            partEAPSDict.Clear();
        }

        void ScanParts(List<Part> parts)
        {
            for (int i = parts.Count - 1; i >= 0; i--)
            {
                if (!partEAPSDict.ContainsKey(parts[i].persistentId))
                {
                    partEAPSDict.Add(parts[i].persistentId, new PartEAPS(parts[i]));
                }
            }
        }
        void _onEditorShipModified(ShipConstruct sc)
        {
            if (sc != null && sc.parts != null)
            {
                ScanParts(sc.parts);
            }
        }

        void onLevelWasLoadedGUIReady(GameScenes gs)
        {
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (EditorLogic.fetch != null && EditorLogic.fetch.ship != null && EditorLogic.fetch.ship.Parts != null)
                    ScanParts(EditorLogic.fetch.ship.Parts);
            }
        }

        // this whole mess below is to work around a stock bug.
        // The bug is that the editor ignores the lock when the Action Group pane is shown.
        // So we have to each time, clear all locks and then reset those which were active when 
        // the mouse moved over a protected window
        void Update()
        {
           if (HighLogic.LoadedSceneIsEditor && ClearInputLocks.focusFollowsclick)
                return;
            if (ClickThruBlocker.CTBWin.activeBlockerCnt > 0)
            {
                //Log.Info("Setting Mouse.HoveredPart to null & deselecting all parts");
                Mouse.HoveredPart = null;

                if (EditorLogic.fetch == null)
                {
                    return;
                }
                for (int i = EditorLogic.fetch.ship.Parts.Count - 1; i >= 0; i--)
                {
                    EditorActionPartSelector selector = partEAPSDict[EditorLogic.fetch.ship.Parts[i].persistentId].eaps; // EditorLogic.fetch.ship.Parts[i].GetComponent<EditorActionPartSelector>();
                    if (selector != null)
                        selector.Deselect();
                }

                if (EditorActionGroups.Instance != null)
                {
                    if (EditorActionGroups.Instance.HasSelectedParts())
                        EditorActionGroups.Instance.ClearSelection(true);
                   for (int i = ClickThruBlocker.CTBWin.selectedParts.Count - 1; i >= 0; i--)
                    //for (int i = 0; i < ClickThruBlocker.CTBWin.selectedParts.Count; i++)
                    {
                        EditorActionPartSelector selector = partEAPSDict[ClickThruBlocker.CTBWin.selectedParts[i].persistentId].eaps; //                                 ClickThruBlocker.CTBWin.selectedParts[i].GetComponent<EditorActionPartSelector>();
                        if (selector != null)
                            EditorActionGroups.Instance.AddToSelection(selector);
                    }
                }
            }

        }

        //static internal long timeTics = 0;
        int d;
        void LateUpdate()
        {
            if (HighLogic.CurrentGame == null ||
                ClearInputLocks.focusFollowsclick) // ||
                //(!HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick && !HighLogic.LoadedSceneIsEditor))
                return;

            d = -1;
            ClickThruBlocker.CTBWin win = null;
            {
                foreach (var w in ClickThruBlocker.winList)
                {
                    //Log.Info("LateUpdate, w.Value.lastUpdated: " + w.Value.lastUpdated);
                    if (w.Value.lastUpdated + 4 < CBTGlobalMonitor.globalTimeTics) //+ 0.05 < Planetarium.GetUniversalTime())
                    {
                        d = w.Key;
                        win = w.Value;
                        break;
                    }
                }
                if (d != -1)
                {
                    win.OnDestroy();
                }
            }
        }
    }
}
#endif