using UnityEngine;
using ToolbarControl_NS;
using KSP.UI.Screens;
using KSP.Localization;

namespace ClearAllInputLocks
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(ClearInputLocks.MODID, ClearInputLocks.MODNAME);
        }
    }

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
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(ClearInputLocksToggle, ClearInputLocksToggle,
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
            }
        }

        void ClearInputLocksToggle()
        {
            //if (!HighLogic.LoadedSceneIsEditor)
            InputLockManager.ClearControlLocks();
            //else
            //    EditorLogic.fetch.Unlock(lockName);
        }

    }
}
