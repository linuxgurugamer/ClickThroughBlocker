using System.IO;
using System.Reflection;

namespace ClickThroughFix
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    // HighLogic.CurrentGame.Parameters.CustomParams<CTB>().focusFollowsclick

    public class CTB : GameParameters.CustomParameterNode
    {
        // Instance.
        private static CTB instance;

        public static CTB Instance
        {
            get
            {
                    if (instance == null)
                        if (HighLogic.CurrentGame != null)
                            instance = HighLogic.CurrentGame.Parameters.CustomParams<CTB>();

                    return instance;
            }
        }

        // Load global settings ahead of save game creation.
        public CTB()
        {
            if (HighLogic.CurrentGame == null)
                OnLoad(null);
        }

        // Boilerplate.
        public override string Title { get { return "Click-Through-Blocker"; } } // Column header
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Click-Through-Blocker"; } }
        public override string DisplaySection { get { return "Click-Through-Blocker"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }


        // Settings.
        [GameParameters.CustomParameterUI("Focus follows mouse click",
            toolTip = "Click on a window to move the  focus to it")]
        public bool focusFollowsclick = false;

        [GameParameters.CustomParameterUI("Universal IMGUI click-through blocking",
            toolTip = "Block clicks / mouse wheel / editor part-pick from passing through\nANY IMGUI mod window onto PAW, the uGUI EventSystem, or KSP's\neditor — even mods that haven't opted in via ClickThruBlocker.GUILayoutWindow.")]
        public bool universalClickBlocking = false;

        [GameParameters.CustomFloatParameterUI("Cleanup delay", minValue = 0.1f, maxValue = 5f, displayFormat = "F2",
            toolTip = "Time to wait after scene change before clearing all the input locks")]
        public float cleanupDelay = 0.5f;

        public override bool Enabled(MemberInfo member, GameParameters parameters) 
        {
            if (Versioning.version_major == 1 && Versioning.version_minor >= 11)
                return member.Name != "cleanupDelay";

            return true; 
        }

        // Global Serialisation.
        // This may seem redundant, but the stock difficulty settings provide a nice enough GUI for global settings.
        // However the user may be confused if they don't know that this is happening and intended behaviour.
        // A better solution would be integration with the stock pause settings menu via a harmony patch.

        static string GlobalDefaultFile
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../Global.cfg";
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            if (!File.Exists(GlobalDefaultFile))
                return;

            // Overwrite the just loaded current settings with the global settings.

            ConfigNode globalSettings = ConfigNode.Load(GlobalDefaultFile);

            bool focusFollowsclick = false;
            if (globalSettings.TryGetValue("focusFollowsClick", ref focusFollowsclick))
                this.focusFollowsclick = focusFollowsclick;

            float cleanupDelay = 0;
            if (globalSettings.TryGetValue("cleanupDelay", ref focusFollowsclick))
                this.cleanupDelay = cleanupDelay;
        }

        public override void OnSave(ConfigNode node)
        {
            // Current settings have just been saved to the save file.
            // Also save them to the global settings file.

            ConfigNode globalSettings = new ConfigNode();
            globalSettings.AddValue("focusFollowsClick", focusFollowsclick);
            globalSettings.AddValue("cleanupDelay", cleanupDelay);
            globalSettings.Save(GlobalDefaultFile);
        }
    }
}

