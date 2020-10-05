using ToolbarControl_NS;
using KSP.UI.Screens;
using KSP.Localization;
using System;
using System.IO;

using KSP.IO;
using UnityEngine;

namespace ClearAllInputLocks
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class ClearInputLocks : MonoBehaviour
    {
        internal const string MODID = "ClearInputLocks_ns";
        internal const string MODNAME = "Clear Input Locks";
        static internal ToolbarControl toolbarControl = null;


        void Start()
        {
            AddToolbarButton();
        }

        void AddToolbarButton()
        {
            if (toolbarControl == null)
            {
#if false
        public void AddToAllToolbars(TC_ClickHandler onTrue, TC_ClickHandler onFalse, TC_ClickHandler onHover, TC_ClickHandler onHoverOut, TC_ClickHandler onEnable, TC_ClickHandler onDisable, ApplicationLauncher.AppScenes visibleInScenes, string nameSpace, string toolbarId, string largeToolbarIcon, string smallToolbarIcon, string toolTip = "");

#endif
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
                    "Clear all input locks"
                );
                toolbarControl.AddLeftRightClickCallbacks(ClearInputLocksToggle, CallModeWindow);
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
                modeWindow = gameObject.AddComponent<OneTimePopup>();
            else
            {
                Destroy(modeWindow);
                InputLockManager.ClearControlLocks();
            }
        }
    }
}
