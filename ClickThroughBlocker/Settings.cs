using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



namespace ClickThroughFix
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick



    public class CTB : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Click-Through-Blocker"; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Click-Through-Blocker"; } }
        public override string DisplaySection { get { return "Click-Through-Blocker"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Focus follows mouse click",
            toolTip = "Click on a window to move the  focus to it")]
        public bool focusFollowsclick = false;

        [GameParameters.CustomParameterUI("Focus change is global",
         toolTip = "This will make it a global setting for all games")]
        public bool global = true;

        [GameParameters.CustomParameterUI("Show Popup at next start",
            toolTip = "Clearing this will allow the pop-up window to be displayed at the next game start.\nSetting it after clearing will allow the popup-window to be shown at the next start of a different save")]
        public bool showPopup = true;

        [GameParameters.CustomFloatParameterUI("Cleanup delay", minValue = 0.1f, maxValue = 5f, displayFormat = "F2",
            toolTip = "Time to wait after scene change before clearing all the input locks")]
        public float cleanupDelay = 0.5f;

        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }

        public override bool Interactible(MemberInfo member, GameParameters parameters) 
        {
            if (showPopup)
                ClearAllInputLocks.OneTimePopup.RemovePopUpFlagFile();
            return true; 
        }

        public override IList ValidValues(MemberInfo member) { return null; }

    }
}

