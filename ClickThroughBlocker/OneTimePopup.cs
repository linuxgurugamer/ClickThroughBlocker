using System;
using System.IO;

using KSP.IO;
using UnityEngine;
using ClickThroughFix;
using System.Reflection;

namespace ClickThroughFix
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class OneTimePopup : MonoBehaviour
    {
        internal static OneTimePopup Instance = null;
        const int WIDTH = 600;
        const int HEIGHT = 350;
        Rect popupRect = new Rect(300, 50, WIDTH, HEIGHT);
        bool visible = false;
        static string popUpShownCfgPath { get { 
                return Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../PluginData/PopUpShown.cfg"); ; 
            } }

        string cancelStr = "Cancel (window will open next startup)";
        Game curGame;
        public void Awake()
        {
            //popUpShownCfgPath = Path.Combine(
            //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../PluginData/PopUpShown.cfg");
            if (HighLogic.CurrentGame != curGame)
            {
                ClearInputLocks.focusFollowsclick = HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick;
                curGame = HighLogic.CurrentGame;
            }
            if (!HighLogic.CurrentGame.Parameters.CustomParams<CTB>().showPopup)
                Destroy(this);
            Instance = this;
        }

        public void Start()
        {

            visible = true;
            if (ClearInputLocks.modeWindow != null)
            {
                visible = true;
                focusFollowsClick = oldFocusFollowsClick = HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick;
                focusFollowsMouse = oldFocusFollowsMouse = !focusFollowsClick;
                cancelStr = "Cancel";
            }
            else
            {
                if (visible)
                    DontDestroyOnLoad(this);
            }
            popupRect.x = (Screen.width - WIDTH) / 2;
            popupRect.y = (Screen.height - HEIGHT) / 2;

        }

        void OnDestroy()
        {
            Instance = null;
        }


        public void OnGUI()
        {
            if (visible)
            {
                GUI.skin = HighLogic.Skin;
                //popupRect = GUILayout.Window(847733455, popupRect, PopUpWindow, "Click Through Blocker Focus Setting");
                popupRect = ClickThruBlocker.GUILayoutWindow(84733455, popupRect, PopUpWindow, "Click Through Blocker Focus Setting");
            }
        }

        bool focusFollowsMouse = false;
        bool focusFollowsClick = false;
        bool oldFocusFollowsMouse = false;
        bool oldFocusFollowsClick = false;
        void PopUpWindow(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label("Up until this release, the Click Through Blocker (CTB) has used a model of");
            GUILayout.Label("Focus-Follows-Mouse, meaning that the focus was on whatever window the mouse was");
            GUILayout.Label("over.  This release provides a new model of Focus-Follows-Click, which means that");
            GUILayout.Label("you will need to click on a window for that window to have the focus, and the focus");
            GUILayout.Label("won't leave the window without clicking outside the window");
            GUILayout.Space(10);
            GUILayout.Label("This window will only appear once to offer the choice of focus model.  It can always");
            GUILayout.Label("be changed in the stock settings window, under the Click-Through-Blocker tab");
            GUILayout.Space(20);
            focusFollowsMouse = GUILayout.Toggle(focusFollowsMouse, "Focus-Follows-Mouse");
            focusFollowsClick = GUILayout.Toggle(focusFollowsClick, "Focus-Follows-Click");
            if (focusFollowsMouse && !oldFocusFollowsMouse)
            {
                oldFocusFollowsMouse = true;
                focusFollowsClick = oldFocusFollowsClick = false;
            }
            if (focusFollowsClick && !oldFocusFollowsClick)
            {
                oldFocusFollowsClick = true;
                focusFollowsMouse = oldFocusFollowsMouse = false;
            }
            if (!focusFollowsClick && !focusFollowsMouse)
                GUI.enabled = false;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save as global default for all new saves"))
            {
                SaveGlobalDefault(focusFollowsClick);
            }
            if (GUILayout.Button("Accept"))
            {
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick = focusFollowsClick;
                HighLogic.CurrentGame.Parameters.CustomParams<CTB>().showPopup = false;
                CreatePopUpFlagFile();
                ClearInputLocks.ClearInputLocksToggle();
                visible = false;
                Destroy(this);
            }
            GUI.enabled = true;

            if (GUILayout.Button(cancelStr))
            {
                visible = false;
                ClearInputLocks.ClearInputLocksToggle();
                Destroy(this);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.enabled = true;
            GUI.DragWindow();
        }

        static string GlobalDefaultFile
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../Global.cfg";
            }
        }
        static internal void SaveGlobalDefault(bool focusFollowsClick)
        {
            ConfigNode node = new ConfigNode();
            node.AddValue("focusFollowsClick", focusFollowsClick);
            node.Save(GlobalDefaultFile);
        }

        static internal bool GetGlobalDefault(ref bool b)
        {
            if (System.IO.File.Exists(GlobalDefaultFile))
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<CTB>().global)
                {
                    ConfigNode node = ConfigNode.Load(GlobalDefaultFile);
                    if (node.TryGetValue("focusFollowsClick", ref b))
                    {
                        return true;
                    }
                }
                else
                    return HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick;
            }
            return false;
        }
        static internal void CreatePopUpFlagFile()
        {
            RemovePopUpFlagFile(); // remove first to avoid any overwriting
            System.IO.File.WriteAllText(popUpShownCfgPath, "popupshown = true");
        }

        static public void RemovePopUpFlagFile()
        {
            System.IO.File.Delete(popUpShownCfgPath);
        }
    }
}
