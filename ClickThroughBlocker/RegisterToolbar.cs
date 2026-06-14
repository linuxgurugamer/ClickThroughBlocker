using ToolbarControl_NS;
using UnityEngine;


namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(ClearInputLocks.MODID, ClearInputLocks.MODNAME);
            ToolbarControl.RegisterMod(ClearInputLocks.MODID2, ClearInputLocks.MODNAME2);
            GameEvents.onGameNewStart.Add(OnGameNewStart);
            GameEvents.onGameStateCreated.Add(OnGameStateCreated);
            GameEvents.OnGameSettingsWritten.Add(OnGameSettingsWritten);
            DontDestroyOnLoad(this);
        }
        void OnGameSettingsWritten()
        {
            if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Parameters.CustomParams<CTB>().global)
                OneTimePopup.SaveGlobalDefault(
                    HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick,
                    HighLogic.CurrentGame.Parameters.CustomParams<CTB>().universalClickBlocking);
        }

        void OnGameNewStart()
        {
            bool b = false;
            if (OneTimePopup.GetGlobalDefault(ref b))
            {
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick = b;
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().showPopup = false;
                OneTimePopup.CreatePopUpFlagFile();
            }
            bool u = false;
            if (OneTimePopup.GetGlobalDefaultUniversal(ref u))
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().universalClickBlocking = u;
        }
        void OnGameStateCreated(Game g)
        {
            bool b = false;
            if (OneTimePopup.GetGlobalDefault(ref b))
            {
                g.Parameters.CustomParams<CTB>().focusFollowsclick = b;
                g.Parameters.CustomParams<CTB>().showPopup = false;
                OneTimePopup.CreatePopUpFlagFile();
            }
            bool u = false;
            if (OneTimePopup.GetGlobalDefaultUniversal(ref u))
                g.Parameters.CustomParams<CTB>().universalClickBlocking = u;
        }
    }
}