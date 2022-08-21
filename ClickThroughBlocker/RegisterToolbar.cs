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
            DontDestroyOnLoad(this);
        }

        void OnGameNewStart()
        {
            bool b = false ;
            if (OneTimePopup.GetGlobalDefault(ref b))
            {
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick = b;
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().showPopup = false;
                OneTimePopup.CreatePopUpFlagFile();
            }
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
        }
    }
}