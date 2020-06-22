using ToolbarControl_NS;
using KSP.UI.Screens;
using KSP.Localization;
using System;
using System.IO;

using KSP.IO;
using UnityEngine;


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
}