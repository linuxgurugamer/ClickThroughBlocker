using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    class CBTMonitor : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(this);
        }
        void LateUpdate()
        {
            int d = 0;
            ClickThruBlocker.CTBWin win = null;

            foreach (var w in  ClickThruBlocker.winList)
            {
                if (w.Value.lastUpdated + 0.05 < Planetarium.GetUniversalTime())
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
