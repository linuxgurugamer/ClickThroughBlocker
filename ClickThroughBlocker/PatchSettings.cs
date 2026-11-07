using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using HarmonyLib;

namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    class Patcher : MonoBehaviour
    {
        public void Start()
        {
            // Use Patcher to load settings only because it's convenient and saves on a GameObject/MonoBehaviour.
            // If the patcher is removed, the settings will need to be loaded elsewhere.
            Settings.Load();

            // Apply patches.
            var harmony = new Harmony("ClickThroughBlocker");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(GameplaySettingsScreen))]
    [HarmonyPatch(nameof(GameplaySettingsScreen.DrawMiniSettings))]
    class PatchDrawMiniSettings
    {
        static void Postfix(ref DialogGUIBase[] __result)
        {
            // Add some elements to the result of GameplaySettingsScreen.DrawMiniSettings, which returns an array of DialogGUI elements for the mini settings window.
            // Modified from KSPCommunityFixes (MIT License)
            // https://github.com/KSPModdingLibs/KSPCommunityFixes/blob/441f2d7e0ab844cbc3bd86ef2c5410c245f08cb9/KSPCommunityFixes/Internal/PatchSettings.cs#L54

            List<DialogGUIBase> modifiedList = __result.ToList();
            List<DialogGUIBase> additions = new List<DialogGUIBase>(3);

            // Focus Follows Click Toggle.
            var toggleLabel = new DialogGUILabel("Focus Follows Click:", 150f);
            var toggle = new DialogGUIToggle(() => Settings.focusFollowsclick, "", b => Settings.focusFollowsclick = b, 150f);

            additions.Add(new DialogGUIHorizontalLayout(0f, 18f, 0f, new RectOffset(), TextAnchor.MiddleLeft, toggleLabel, toggle, new DialogGUIFlexibleSpace()));

            // Universal click blocking toggle.
            var universalLabel = new DialogGUILabel("Universal Click Blocking:", 150f);
            var universalToggle = new DialogGUIToggle(() => Settings.universalClickBlocking, "", b => Settings.universalClickBlocking = b, 150f);

            additions.Add(new DialogGUIHorizontalLayout(0f, 18f, 0f, new RectOffset(), TextAnchor.MiddleLeft, universalLabel, universalToggle, new DialogGUIFlexibleSpace()));

            // Clean up delay slider.
            if (!(Versioning.version_major == 1 && Versioning.version_minor >= 11))
            {
                DialogGUISlider slider = new DialogGUISlider(() => Settings.cleanupDelay, 0.1f, 5f, false, 150f, -1f, f => Settings.cleanupDelay = f);
                DialogGUILabel label = new DialogGUILabel(() => Settings.cleanupDelay.ToString(), 80);

                DialogGUIHorizontalLayout entry = new DialogGUIHorizontalLayout(0f, 18f, 0f, new RectOffset(), TextAnchor.MiddleLeft,
                    new DialogGUILabel("Cleanup Delay:", 150f), slider, new DialogGUISpace(30f), label);

                additions.Add(entry);
            }

            // Add the new entries to the list after UI but before the Flight UI section (or the end of the list if not in Flight).
            int index = modifiedList.Count;
            bool foundUI = false;

            for (int i = 1; i < index; i++)
            {
                if (modifiedList[i] is DialogGUIBox)
                {
                    if (!foundUI)
                        foundUI = true;
                    else
                    {
                        index = i;
                        break;
                    }
                }
            }

            modifiedList.InsertRange(index, additions);
            __result = modifiedList.ToArray();
        }
    }

    [HarmonyPatch(typeof(GameplaySettingsScreen))]
    [HarmonyPatch(nameof(GameplaySettingsScreen.ApplySettings))]
    class PatchApplySettings
    {
        static void Postfix()
        {
            // When the user clicks accept or apply on the mini-settings window.

            Settings.Save();
        }
    }
}
