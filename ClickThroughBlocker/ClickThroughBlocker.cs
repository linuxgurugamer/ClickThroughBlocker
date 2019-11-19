using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.Screens;
using UnityEngine.EventSystems;


namespace ClickThroughFix
{

    public static class ClickThruBlocker
    {
#if !DUMMY2
        internal static Dictionary<int, CTBWin> winList = new Dictionary<int, CTBWin>();
#endif
        // Most of this is from JanitorsCloset, ImportExportSelect.cs

        public class CTBWin
        {
#if !DUMMY2
            //public Rect rect;
            internal int id;

            internal bool weLockedEditorInputs = false;
            internal bool weLockedFlightInputs = false;
            internal string windowName;
            internal string lockName;
            internal long lastLockCycle;
            internal double lastUpdated = 0;


            public CTBWin(int id, Rect screenRect, string winName, string lockName)
            {
#if !DUMMY
                this.id = id;
                this.windowName = winName;
                this.lockName = lockName;
                lastUpdated = CBTMonitor.timeTics; // Planetarium.GetUniversalTime();
#endif
            }

            public void SetLockString(string s)
            {
                this.lockName = s;
            }
#if !DUMMY
            static Vector2 mousePos = new Vector2();
            public static bool MouseIsOverWindow(Rect rect)
            {
                mousePos.x = Input.mousePosition.x;
                mousePos.y = Screen.height - Input.mousePosition.y;
                return rect.Contains(mousePos);
            }
#endif
            internal static int activeBlockerCnt = 0;
            internal static List<Part> selectedParts = null;

            //Lifted this more or less directly from the Kerbal Engineer source. Thanks cybutek!
            internal void PreventEditorClickthrough(Rect r)
            {
#if DUMMY
                return;
#else
                if (lockName == null || EditorLogic.fetch == null)
                    return;
                //Log.Info("ClickThruBlocker: PreventEditorClickthrough");
                bool mouseOverWindow = MouseIsOverWindow(r);
                //Log.Info("PreventEditorClickthrough, mouseOverWindow: " + mouseOverWindow);
                if (mouseOverWindow)
                {
                    if (!weLockedEditorInputs)
                    {
                        //Log.Info("PreventEditorClickthrough, locking on window: " + windowName);
                        EditorLogic.fetch.Lock(true, true, true, lockName);
                        weLockedEditorInputs = true;
                        activeBlockerCnt++;
                        selectedParts = EditorActionGroups.Instance.GetSelectedParts();
                    }
                    lastLockCycle = OnGUILoopCount.GetOnGUICnt();
                    return;
                }
                if (!weLockedEditorInputs) return;
                //Log.Info("PreventEditorClickthrough, unlocking on window: " + windowName);
                EditorLogic.fetch.Unlock(lockName);
                weLockedEditorInputs = false;
                activeBlockerCnt--;
#endif
            }

            // Following lifted from MechJeb
            internal void PreventInFlightClickthrough(Rect r)
            {
#if !DUMMY
                //Log.Info("ClickThruBlocker: PreventInFlightClickthrough");
                bool mouseOverWindow = MouseIsOverWindow(r);
                if (mouseOverWindow)
                {
                    if (!weLockedFlightInputs && !Input.GetMouseButton(1) && lockName != null)
                    {
                        //Log.Info("PreventInFlightClickthrough, locking on window: " + windowName); ;

                        InputLockManager.SetControlLock(ControlTypes.ALLBUTCAMERAS, lockName);
                        weLockedFlightInputs = true;

                    }
                    if (weLockedFlightInputs)
                        lastLockCycle = OnGUILoopCount.GetOnGUICnt();
                }
                if (weLockedFlightInputs && !mouseOverWindow && lockName != null)
                {
                    //Log.Info("PreventInFlightClickthrough, unlocking on window: " + windowName);
                    InputLockManager.RemoveControlLock(lockName);
                    weLockedFlightInputs = false;
                }
#endif
            }

            internal void OnDestroy()
            {
#if !DUMMY
                //Log.Info("OnDestroy, windowName: " + windowName + ", lockName: " + lockName + ", weLockedEditorInputs: " + weLockedEditorInputs.ToString() +
                //    ",  weLockedFlightInputs: " + weLockedFlightInputs.ToString());
                winList.Remove(id);
                if (lockName != null)
                {
                    if (HighLogic.LoadedSceneIsEditor)
                        EditorLogic.fetch.Unlock(lockName);
                    else
                        InputLockManager.RemoveControlLock(lockName);
                }
                if (EditorLogic.fetch != null && weLockedEditorInputs)
                {
                    //EditorLogic.fetch.Unlock(lockName);
                    weLockedEditorInputs = false;
                    activeBlockerCnt--;
                }
                if (weLockedFlightInputs)
                {
                    //InputLockManager.RemoveControlLock(lockName);
                    weLockedFlightInputs = false;
                }
#endif
            }
        }
#endif
        // This is outside the UpdateList method for runtime optimization 
        static CTBWin win = null;
        private static Rect UpdateList(int id, Rect rect, string text)
        {
#if !DUMMY
            win = null;
            if (!winList.TryGetValue(id, out win))
            {
                win = new CTBWin(id, rect, text, text);
                winList.Add(id, win);
            }

            if (HighLogic.LoadedSceneIsEditor)
                win.PreventEditorClickthrough(rect);
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneHasPlanetarium)
                win.PreventInFlightClickthrough(rect);
            win.lastUpdated = CBTMonitor.timeTics; // Planetarium.GetUniversalTime();
#endif
            return rect;
        }

        // This is outside all the GuiLayoutWindow methods for runtime optimization
        static Rect r;
        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            r = GUILayout.Window(id, screenRect, func, text, style, options);

            return UpdateList(id, r, text);
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options)
        {
            r = GUILayout.Window(id, screenRect, func, text, options);

            return UpdateList(id, r, text);
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options)
        {
            r = GUILayout.Window(id, screenRect, func, content, options);

            return UpdateList(id, r, id.ToString());
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options)
        {
            r = GUILayout.Window(id, screenRect, func, image, options);

            return UpdateList(id, r, id.ToString());
        }

        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
        {
            r = GUI.Window(id, clientRect, func, image, style);

            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
        {
            r = GUI.Window(id, clientRect, func, text, style);

            return UpdateList(id, r, text);
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content)
        {
            r = GUI.Window(id, clientRect, func, content);

            return UpdateList(id, r, id.ToString());

        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image)
        {
            r = GUI.Window(id, clientRect, func, image);

            return UpdateList(id, r, id.ToString());

        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, string text)
        {
            r = GUI.Window(id, clientRect, func, text);

            return UpdateList(id, r, text);
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style)
        {
            r = GUI.Window(id, clientRect, func, title, style);

            return UpdateList(id, r, title.ToString());
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, UnityEngine.GUI.WindowFunction func, Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            r = GUILayout.Window(id, screenRect, func, image, style, options);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUILayoutWindow(int id, Rect screenRect, UnityEngine.GUI.WindowFunction func, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            r = GUILayout.Window( id,  screenRect,  func,  content,  style,  options);
            return UpdateList(id, r, id.ToString());
        }

        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, string text)
        {
            r = GUI.ModalWindow(id, clientRect, func, text);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, Texture image)
        {
            r = GUI.ModalWindow(id, clientRect, func, image);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, GUIContent content)
        {
            r = GUI.ModalWindow(id, clientRect, func, content);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, string text, GUIStyle style)
        {
            r = GUI.ModalWindow(id, clientRect, func, text, style);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, Texture image, GUIStyle style)
        {
            r = GUI.ModalWindow(id, clientRect, func, image, style);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIModalWindow(int id, Rect clientRect, UnityEngine.GUI.WindowFunction func, GUIContent content, GUIStyle style)
        {
            r = GUI.ModalWindow( id,  clientRect,  func,  content,  style);
            return UpdateList(id, r, id.ToString());
        }



#if false

        static bool fieldHasFocus()
        {
            GameObject obj;
            bool inputFieldIsFocused;
            // First check for a text field ???
            // Ignore keystrokes when a text field has focus (e.g. part search, craft title box)
            obj = EventSystem.current.currentSelectedGameObject;
            inputFieldIsFocused = (obj != null && obj.GetComponent<InputField>() != null && obj.GetComponent<InputField>().isFocused);
            //if (inputFieldIsFocused)
                //return false;

            //inputFieldIsFocused = (inputObj != null && inputObj.GetComponent<InputField>() != null && inputObj.GetComponent<InputField>().isFocused);

            return inputFieldIsFocused;
        }

        // GUI.TextArea
        public static string GUITextArea(Rect position, string text)
        {
            string t = text;
            text = GUI.TextArea(position, text);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextArea(Rect position, string text, int maxLength)
        {
            string t = text;
            text = GUI.TextArea(position, text, maxLength);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextArea(Rect position, string text, GUIStyle style)
        {
            string t = text;
            text = GUI.TextArea(position, text, style);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextArea(Rect position, string text, int maxLength, GUIStyle style)
        {
            string t = text;
            text = GUI.TextArea(position, text, maxLength, style);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        // GUI.TextField
        public static string GUITextField(Rect position, string text, GUIStyle style)
        {
            string t = text;
            text = GUI.TextField(position, text, style);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextField(Rect position, string text, int maxLength)
        {
            string t = text;
            text = GUI.TextField(position, text, maxLength);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextField(Rect position, string text)
        {
            string t = text;
            text = GUI.TextField(position, text);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUITextField(Rect position, string text, int maxLength, GUIStyle style)
        {
            string t = text;
            text = GUI.TextField(position, text, maxLength, style);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        // GUILayout.TextArea

        public static string GUILayoutTextArea(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextArea(text, maxLength, style, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextArea(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextArea(text, style, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextArea(string text, int maxLength, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextArea(text, maxLength, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextArea(string text, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextArea(text, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        // GUILayout.TextField

        public static string GUILayoutTextField(string text, int maxLength, GUIStyle style, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextField(text, maxLength, style, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextField(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextField(text, style, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextField(string text, int maxLength, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextField(text, maxLength, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }

        public static string GUILayoutTextField(string text, params GUILayoutOption[] options)
        {
            string t = text;
            text = GUILayout.TextField(text, options);
            if (!fieldHasFocus())
                return t;
            return text;
        }
#endif
    }
#if !DUMMY
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    internal class OnGUILoopCount : MonoBehaviour
    {
        static long onguiCnt = 0;


        internal static long GetOnGUICnt()
        {
            return onguiCnt;
        }
        private void Start()
        {
            DontDestroyOnLoad(this);
            InvokeRepeating("DoGuiCounter", 5.0f, 0.25f);
        }

        long lastonGuiCnt;
        private void DoGuiCounter()
        {
            lastonGuiCnt = (onguiCnt++) - 1;

            foreach (var win in ClickThruBlocker.winList.Values)
            {
                if (win.lastLockCycle < lastonGuiCnt)
                {
                    //Log.Info("lastonGuiCnt: " + lastonGuiCnt + "lastLockCycle: " + win.lastLockCycle);
                    {
                        if (EditorLogic.fetch != null && win.weLockedEditorInputs)
                        {
                            EditorLogic.fetch.Unlock(win.lockName);
                            win.weLockedEditorInputs = false;
                            ClickThruBlocker.CTBWin.activeBlockerCnt--;
                        }
                        if (win.weLockedFlightInputs && win.lockName != null)
                        {
                            InputLockManager.RemoveControlLock(win.lockName);
                            win.weLockedFlightInputs = false;
                        }
                    }
                }
            }

        }
    }
#endif
}
