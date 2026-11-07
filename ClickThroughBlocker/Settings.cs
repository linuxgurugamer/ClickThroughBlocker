using System.IO;
using System.Reflection;

namespace ClickThroughFix
{
    public static class Settings
    {
        public static bool focusFollowsclick = false;
        public static bool universalClickBlocking = false;
        public static float cleanupDelay = 0.5f;

        private static string GlobalDefaultFile =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../Global.cfg";

        public static void Load()
        {
            if (!File.Exists(GlobalDefaultFile))
                return;

            ConfigNode globalSettings = ConfigNode.Load(GlobalDefaultFile);

            bool focusFollowsclick = false;
            if (globalSettings.TryGetValue("focusFollowsClick", ref focusFollowsclick))
                Settings.focusFollowsclick = focusFollowsclick;

            bool universalClickBlocking = false;
            if (globalSettings.TryGetValue("universalClickBlocking", ref universalClickBlocking))
                Settings.universalClickBlocking = universalClickBlocking;

            float cleanupDelay = 0;
            if (globalSettings.TryGetValue("cleanupDelay", ref cleanupDelay))
                Settings.cleanupDelay = cleanupDelay;
        }

        public static void Save()
        {
            ConfigNode globalSettings = new ConfigNode();
            globalSettings.AddValue("focusFollowsClick", focusFollowsclick);
            globalSettings.AddValue("universalClickBlocking", universalClickBlocking);
            globalSettings.AddValue("cleanupDelay", cleanupDelay);
            globalSettings.Save(GlobalDefaultFile);
        }
    }
}

