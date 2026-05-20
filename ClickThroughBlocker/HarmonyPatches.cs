#if !DUMMY
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// CTB's existing InputLockManager / EditorLogic-based locks block IMGUI-on-IMGUI and
// IMGUI-on-KSP-input click-through, but they don't reliably block clicks from reaching
// Unity's uGUI EventSystem or the stock Part Action Window. These Harmony patches
// fill that gap by tracking every IMGUI window drawn this frame and short-circuiting
// the click-dispatch paths when the cursor is over any of them. Windows that haven't
// adopted CTB (MechJeb etc., which use plain GUILayout.Window) are protected too.

namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class CTBHarmonyLoader : MonoBehaviour
    {
        private const string HarmonyId = "ClickThroughFix.CTB";
        private const string UniversalLockId = "CTB_universal_IMGUI";
        private static bool _patched;
        private bool _flightLocked;
        private bool _editorLocked;

        internal void Awake()
        {
            DontDestroyOnLoad(this);
            if (_patched) return;
            _patched = true;
            try
            {
                new Harmony(HarmonyId).PatchAll(typeof(CTBHarmonyLoader).Assembly);
            }
            catch (System.Exception e)
            {
                Log.Error("CTB Harmony patching failed: " + e);
            }
        }

        // KSP's editor part-pick (single + double click to move) and various flight
        // controls poll Input.GetMouseButton directly and honor InputLockManager /
        // EditorLogic locks. uGUI raycaster suppression doesn't reach those paths.
        // Set a universal lock whenever the cursor is over any tracked IMGUI window
        // so a click meant for the mod window doesn't also grab a part in the editor.
        internal void Update()
        {
            // Sync the static enable flag from the per-save CTB settings each frame so
            // toggling the option in the stock settings UI takes effect immediately.
            if (HighLogic.CurrentGame != null)
                ClearInputLocks.universalClickBlocking =
                    HighLogic.CurrentGame.Parameters.CustomParams<CTB>().universalClickBlocking;

            bool over = ClearInputLocks.universalClickBlocking && IMGUIWindowTracker.MouseOverAnyWindow();
            bool wantEditor = over && HighLogic.LoadedSceneIsEditor && EditorLogic.fetch != null;
            bool wantFlight = over && (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneHasPlanetarium);

            if (wantEditor != _editorLocked)
            {
                if (wantEditor) EditorLogic.fetch.Lock(true, true, true, UniversalLockId);
                else if (EditorLogic.fetch != null) EditorLogic.fetch.Unlock(UniversalLockId);
                _editorLocked = wantEditor;
            }
            if (wantFlight != _flightLocked)
            {
                if (wantFlight) InputLockManager.SetControlLock(ControlTypes.ALLBUTCAMERAS, UniversalLockId);
                else InputLockManager.RemoveControlLock(UniversalLockId);
                _flightLocked = wantFlight;
            }
        }
    }

    // Records the screen-space rect of every IMGUI window drawn this frame.
    // Accumulates within a Unity frame across all OnGUI iterations (Layout, Repaint,
    // MouseDown, …) — Unity dispatches MouseDown only to the focused window's body,
    // so per-iter clearing would lose the rest of the visible windows. Swap to
    // _lastRects at each new Unity frame so EventSystem.RaycastAll (running in
    // Update before this frame's OnGUI) sees the previous frame's full set.
    internal static class IMGUIWindowTracker
    {
        private static List<Rect> _curRects = new List<Rect>();
        private static List<Rect> _lastRects = new List<Rect>();
        private static int _frame = -1;

        // Transform the returned window rect through the current GUI.matrix so we store
        // actual screen-pixel bounds. KSP applies a UI_SCALE matrix that some mods (KAC)
        // live inside; others (MechJeb) override GUI.matrix and draw in screen coords
        // directly. By transforming through the matrix in effect at return time, the
        // stored rect is always in screen-space and Contains(Input.mousePosition) works
        // for both kinds of mods.
        public static void RecordScreenSpace(Rect r)
        {
            if (!ClearInputLocks.universalClickBlocking) return;
            var m = GUI.matrix;
            Vector3 tl = m.MultiplyPoint3x4(new Vector3(r.xMin, r.yMin, 0f));
            Vector3 br = m.MultiplyPoint3x4(new Vector3(r.xMax, r.yMax, 0f));
            EnsureFrame();
            _curRects.Add(Rect.MinMaxRect(
                Mathf.Min(tl.x, br.x),
                Mathf.Min(tl.y, br.y),
                Mathf.Max(tl.x, br.x),
                Mathf.Max(tl.y, br.y)));
        }

        public static bool MouseOverAnyWindow()
        {
            if (!ClearInputLocks.universalClickBlocking) return false;
            EnsureFrame();
            Vector2 mp = Input.mousePosition;
            mp.y = Screen.height - mp.y;
            for (int i = 0; i < _curRects.Count; i++)
                if (_curRects[i].Contains(mp)) return true;
            for (int i = 0; i < _lastRects.Count; i++)
                if (_lastRects[i].Contains(mp)) return true;
            return false;
        }

        private static void EnsureFrame()
        {
            int frame = Time.frameCount;
            if (frame == _frame) return;
            var tmp = _lastRects;
            _lastRects = _curRects;
            _curRects = tmp;
            _curRects.Clear();
            _frame = frame;
        }
    }

    // Hook every GUI.Window / GUILayout.Window overload so we catch every IMGUI
    // window regardless of which entry point the mod used. Double-records for
    // GUILayout-routed windows (which internally call GUI.Window) are harmless —
    // MouseOverAnyWindow only checks Contains.
    [HarmonyPatch]
    internal class CTB_TrackIMGUIWindows
    {
        [HarmonyTargetMethods]
        internal static IEnumerable<MethodBase> Targets()
        {
            foreach (var t in new[] { typeof(GUI), typeof(GUILayout) })
                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    if (m.Name == "Window") yield return m;
        }

        [HarmonyPostfix]
        internal static void Postfix(Rect __result) => IMGUIWindowTracker.RecordScreenSpace(__result);
    }

    // Suppress a uGUI raycaster's hits when the cursor is over a tracked IMGUI mod
    // window. Stock KSP uGUI canvases overlap each other constantly (MainCanvas,
    // AppCanvas, Editor are all near-full-screen), so any further "is another canvas
    // covering us" heuristic just blocks legitimate stock UI clicks.
    [HarmonyPatch(typeof(GraphicRaycaster))]
    internal class CTB_GraphicRaycaster_Raycast
    {
        [HarmonyPrefix]
        [HarmonyPatch("Raycast")]
        [HarmonyPatch(new[] { typeof(PointerEventData), typeof(List<RaycastResult>) })]
        internal static bool Prefix()
        {
            return !ClickThruBlocker.MouseOverAnyWindow();
        }
    }

    // Right-click on a part runs UIPartActionController.MouseClickCoroutine which opens
    // or closes the PAW. Bail when the cursor is over a registered CTB window so a
    // right-click meant for the mod window doesn't also pop up the PAW behind it.
    [HarmonyPatch(typeof(UIPartActionController))]
    internal class CTB_UIPartActionController_MouseClickCoroutine
    {
        [HarmonyPrefix]
        [HarmonyPatch("MouseClickCoroutine")]
        internal static bool Prefix(ref IEnumerator __result)
        {
            if (ClickThruBlocker.MouseOverAnyWindow())
            {
                __result = NoOp();
                return false;
            }
            return true;
        }

        private static IEnumerator NoOp() { yield break; }
    }
}
#endif
