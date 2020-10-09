using ToolbarControl_NS;
using KSP.UI.Screens;
using KSP.Localization;
using System;
using System.IO;

using KSP.IO;
using UnityEngine;

namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class ClearInputLocks : MonoBehaviour
    {
        internal const string MODID = "CTB_ClearInputLocks_ns";
        internal const string MODID2 = "CTB_Toggle_ns";
        internal const string MODNAME = "ClickThroughBlocker: Clear Input Locks";
        internal const string MODNAME2 = "ClickThroughBlocker: Toggle Mode";
        static internal ToolbarControl toolbarControl = null;
        static internal ToolbarControl clickThroughToggleControl = null;
        static internal bool focusFollowsclick = false;

        const string FFC_38 = "000_ClickThroughBlocker/PluginData/FFC-38";
        const string FFM_38 = "000_ClickThroughBlocker/PluginData/FFM-38";
        const string FFC_24 = "000_ClickThroughBlocker/PluginData/FFC-24";
        const string FFM_24 = "000_ClickThroughBlocker/PluginData/FFM-24";


        void Start()
        {
            AddToolbarButton();
        }

        void AddToolbarButton()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(null, null,
                    ApplicationLauncher.AppScenes.SPACECENTER |
                    ApplicationLauncher.AppScenes.FLIGHT |
                    ApplicationLauncher.AppScenes.MAPVIEW |
                    ApplicationLauncher.AppScenes.VAB |
                    ApplicationLauncher.AppScenes.SPH |
                    ApplicationLauncher.AppScenes.TRACKSTATION,
                    MODID,
                    "ClearInputLocks",
                    "000_ClickThroughBlocker/PluginData/lock-38",
                    "000_ClickThroughBlocker/PluginData/lock-24",
                    MODNAME
                );
                toolbarControl.AddLeftRightClickCallbacks(ClearInputLocksToggle, CallModeWindow);



                clickThroughToggleControl = gameObject.AddComponent<ToolbarControl>();
                clickThroughToggleControl.AddToAllToolbars(ToggleFocusSetting, ToggleFocusSetting,
                    ApplicationLauncher.AppScenes.SPACECENTER |
                    ApplicationLauncher.AppScenes.FLIGHT |
                    ApplicationLauncher.AppScenes.MAPVIEW |
                    ApplicationLauncher.AppScenes.VAB |
                    ApplicationLauncher.AppScenes.SPH |
                    ApplicationLauncher.AppScenes.TRACKSTATION,
                    MODID2,
                    "CTBToggle",
                    FFC_38,
                    FFM_38,
                    FFC_24,
                    FFM_24,
                    MODNAME2
                );
            }
        }

        void OnDestroy()
        {
            if (toolbarControl != null)
            {
                toolbarControl.OnDestroy();
                Destroy(toolbarControl);
            }
        }

        static internal void ToggleFocusSetting()
        {
            focusFollowsclick = !focusFollowsclick;
            if (focusFollowsclick)
                clickThroughToggleControl.SetTexture(FFC_38, FFC_24);
            else
                clickThroughToggleControl.SetTexture(FFM_38, FFM_24);
        }
        static internal void ClearInputLocksToggle()
        {
            InputLockManager.ClearControlLocks();
            ScreenMessages.PostScreenMessage("All Input Locks Cleared", 5);
        }

        internal static MonoBehaviour modeWindow = null;
        void CallModeWindow()
        {
            //ClickThroughFix.Log.Info("CallModeWindow, modeWindow: " + (modeWindow != null));
            if (modeWindow == null)
            {
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().showPopup = true;
                modeWindow = gameObject.AddComponent<OneTimePopup>();
            }
            else
            {
                Destroy(modeWindow);
                InputLockManager.ClearControlLocks();
            }
        }
    }
}
