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

        internal static Dictionary<int, CTBWin> winList = new Dictionary<int, CTBWin>();

        // Most of this is from JanitorsCloset, ImportExportSelect.cs

        public class CTBWin
        {
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
                this.id = id;
                this.windowName = winName;
                this.lockName = lockName;
                lastUpdated = CBTMonitor.timeTics; // Planetarium.GetUniversalTime();
            }

            public void SetLockString(string s)
            {
                this.lockName = s;
            }

            public static bool MouseIsOverWindow(Rect rect)
            {
                return   rect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
            }

            internal static int activeBlockerCnt = 0;
            internal static List<Part> selectedParts = null;

            //Lifted this more or less directly from the Kerbal Engineer source. Thanks cybutek!
            internal void PreventEditorClickthrough(Rect r)
            {
                //Log.Info("ClickThruBlocker: PreventEditorClickthrough");
                bool mouseOverWindow = MouseIsOverWindow(r);
                Log.Info("PreventEditorClickthrough, mouseOverWindow: " + mouseOverWindow);
                if (mouseOverWindow)
                {
                    if (!weLockedEditorInputs)
                    {
                        Log.Info("PreventEditorClickthrough, locking on window: " + windowName);
                        EditorLogic.fetch.Lock(true, true, true, lockName);
                        weLockedEditorInputs = true;
                        activeBlockerCnt++;
                        selectedParts = EditorActionGroups.Instance.GetSelectedParts();
                    }
                    lastLockCycle = OnGUILoopCount.GetOnGUICnt();
                    return;
                }
                if (!weLockedEditorInputs) return;
                Log.Info("PreventEditorClickthrough, unlocking on window: " + windowName);
                EditorLogic.fetch.Unlock(lockName);
                weLockedEditorInputs = false;
                activeBlockerCnt--;
            }

            // Following lifted from MechJeb
            internal void PreventInFlightClickthrough(Rect r)
            {
                //Log.Info("ClickThruBlocker: PreventInFlightClickthrough");
                bool mouseOverWindow = MouseIsOverWindow(r);
                if (mouseOverWindow)
                {
                    if (!weLockedFlightInputs && !Input.GetMouseButton(1))
                    {
                        Log.Info("ClickThruBlocker: PreventInFlightClickthrough, locking on window: " + windowName); ;

                        InputLockManager.SetControlLock(ControlTypes.ALLBUTCAMERAS, lockName);
                        weLockedFlightInputs = true;
                       
                    }
                    if (weLockedFlightInputs)
                        lastLockCycle = OnGUILoopCount.GetOnGUICnt();
                }
                if (weLockedFlightInputs && !mouseOverWindow)
                {
                    Log.Info("ClickThruBlocker: PreventInFlightClickthrough, unlocking on window: " + windowName);
                    InputLockManager.RemoveControlLock(lockName);
                    weLockedFlightInputs = false;
                }
            }

            internal void OnDestroy()
            {
                Log.Info("ClickThruBlocker: OnDestroy, windowName: " + windowName + ", lockName: " + lockName);
                winList.Remove(id);
                if (weLockedEditorInputs)
                {
                    EditorLogic.fetch.Unlock(lockName);
                    weLockedEditorInputs = false;
                    activeBlockerCnt--;
                }
                if (weLockedFlightInputs)
                {
                    InputLockManager.RemoveControlLock(lockName);
                    weLockedFlightInputs = false;
                }
            }
        }


        private static Rect UpdateList(int id, Rect rect, string text)
        {
            CTBWin win = null;
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
            return rect;
        }     
    
        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect r = GUILayout.Window(id, screenRect, func, text, style, options);

            return UpdateList(id, r, text); 
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, string text, params GUILayoutOption[] options)
        {
            Rect r = GUILayout.Window(id, screenRect, func, text, options);

            return UpdateList(id, r, text);
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, GUIContent content, params GUILayoutOption[] options)
        {
            Rect r = GUILayout.Window(id, screenRect, func, content, options);

            return UpdateList(id, r, id.ToString());
        }

        public static Rect GUILayoutWindow(int id, Rect screenRect, GUI.WindowFunction func, Texture image, params GUILayoutOption[] options)
        {
            Rect r = GUILayout.Window(id, screenRect, func, image, options);

            return UpdateList(id, r, id.ToString());
        }

        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image, GUIStyle style)
        {
            Rect r = GUI.Window(id, clientRect, func, image, style);
            return UpdateList(id, r, id.ToString());

        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, string text, GUIStyle style)
        {
            Rect r = GUI.Window(id, clientRect, func, text, style);
            return UpdateList(id, r, text);
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent content)
        {
            Rect r = GUI.Window(id, clientRect, func, content);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, Texture image)
        {
            Rect r = GUI.Window(id, clientRect, func, image);
            return UpdateList(id, r, id.ToString());
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, string text)
        {
            Rect r = GUI.Window(id, clientRect, func, text);
            return UpdateList(id, r, text);
        }
        public static Rect GUIWindow(int id, Rect clientRect, GUI.WindowFunction func, GUIContent title, GUIStyle style)
        {
            Rect r = GUI.Window(id, clientRect, func, title, style);
            return UpdateList(id, r, title.ToString());
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
            InvokeRepeating("DoGuiCounter",5.0f, 0.25f);
        }
        private void OnDestroy()
        {

        }
        private void DoGuiCounter()
        {
            long lastonGuiCnt = (onguiCnt++) - 1;

            //for (int i = 0; i < ClickThruBlocker.winList.Count; i++)
            foreach (var win in ClickThruBlocker.winList.Values)
            {
                if (win.lastLockCycle < lastonGuiCnt)
                {
                    Log.Info("lastonGuiCnt: " + lastonGuiCnt + "lastLockCycle: " + win.lastLockCycle);
                    {
                        if (win.weLockedEditorInputs)
                        {
                            EditorLogic.fetch.Unlock(win.lockName);
                            win.weLockedEditorInputs = false;
                            ClickThruBlocker.CTBWin.activeBlockerCnt--;
                        }
                        if (win.weLockedFlightInputs)
                        {
                            InputLockManager.RemoveControlLock(win.lockName);
                            win.weLockedFlightInputs = false;
                        }
                    }
                }
            }

        }
    }

}