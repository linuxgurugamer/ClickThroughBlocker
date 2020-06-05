#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System;

namespace ClickThroughBlocker
{
    class GlobalFlagStorage
    {
        const string NODE = "CLICKTHROUGHBLOCKER";
        string PATH = KSPUtil.ApplicationRootPath + "GameData/000_ClickThroughBlocker/PluginData/CBT.cfg";

        ConfigNode configFile;
        ConfigNode configFileNode;

        public void Create()
        {
            if (configFileNode == null)
            {
                configFile = new ConfigNode();
            }
            if (!configFile.HasNode(NODE))
            {
                configFileNode = new ConfigNode(NODE);
                configFile.SetNode(NODE, configFileNode, true);
            }
            else
            {
                if (configFileNode == null)
                {
                    configFileNode = configFile.GetNode(NODE);

                }
            }
        }

        public bool Load()
        {
            configFile = ConfigNode.Load(PATH);
            if (configFile != null)
                configFileNode = configFile.GetNode(NODE);
            return configFile != null;
        }

        public void Save()
        {
            configFile.Save(PATH);
        }

        public void SetValue(string name, string value)
        {
            configFileNode.SetValue(name, value, true);
        }

#if false
        public void SetValue(string name, Rect rect)
        {
            SetValue(name + ".x", (int)rect.x);
            SetValue(name + ".y", (int)rect.y);
            SetValue(name + ".width", (int)rect.width);
            SetValue(name + ".height", (int)rect.height);
        }
        public void SetValue(string name, int i)
        {
            SetValue(name, i.ToString());
        }
#endif
        public void SetValue(string name, bool b)
        {
            SetValue(name, b.ToString());
        }

        public string GetValue(string name, string d = "")
        {
            string s = d;
            if (name == null)
                return "";

            if (configFileNode.HasValue(name))
                s = configFileNode.GetValue(name);

            return s;
        }

  #if false
      public Rect GetValue(string name, Rect rect)
        {
            try
            {
                rect.x = Convert.ToSingle(GetValue(name + ".x", rect.x.ToString()));
                rect.y = Convert.ToSingle(GetValue(name + ".y", rect.y.ToString()));
                rect.width = Convert.ToSingle(GetValue(name + ".width", rect.width.ToString()));
                rect.height = Convert.ToSingle(GetValue(name + ".height", rect.height.ToString()));
            }
            catch (Exception e)
            {
                Debug.LogError("Exception converting: " + name + " -   " + e.Message);
            }
            return rect;
        }
        public int GetValue(string name, int i = 0)
        {
            int r;
            try
            {
                r = Convert.ToInt32(GetValue(name, i.ToString()));
            }
            catch (Exception e)
            {
                r = i;
                Debug.LogError("Exception converting: " + name + " -   " + e.Message);
            }
            return r;
        }
#endif
        public bool GetValue(string name, bool b = false)
        {
            bool r;
            try
            {

                r = Convert.ToBoolean(GetValue(name, b.ToString()));
            }
            catch (Exception e)
            {
                r = b;
                Debug.LogError("Exception converting: " + name + " -   " + e.Message);
            }
            return r;
        }
    }
}
#endif