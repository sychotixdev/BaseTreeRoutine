using System;
using System.IO;
using System.Threading;
using TreeRoutine.DefaultBehaviors.Helpers;
using TreeRoutine.FlaskComponents;
using Newtonsoft.Json;
using PoeHUD.Controllers;
using PoeHUD.Hud.Health;
using PoeHUD.Plugins;
using TreeRoutine.Menu;
using ImGuiNET;
using TreeRoutine.TreeSharp;

namespace TreeRoutine
{
    public abstract class BaseTreeRoutinePlugin<TSettings, TCache> : BaseSettingsPlugin<TSettings>
            where TSettings : BaseTreeSettings, new()
            where TCache : BaseTreeCache, new()
    {
        public int LogmsgTime { get; set; } = 3;
        public int ErrmsgTime { get; set; } = 10;

        protected Timer Timer { get; private set; } = null;

        public TCache Cache { get; protected set; }

        // These are useful for the helper methods
        public Action<object, float> Log => LogMessage;
        public Action<object, float> LogErr => LogError;

        public FlaskHelper<TSettings, TCache> FlaskHelper { get; set; } = new FlaskHelper<TSettings, TCache>();
        public PlayerHelper<TSettings, TCache> PlayerHelper { get; set; } = new PlayerHelper<TSettings, TCache>();
        public TreeHelper<TSettings, TCache> TreeHelper { get; set; } = new TreeHelper<TSettings, TCache>();

        public static TSettingType LoadSettingFile<TSettingType>(String fileName)
        {
            if (!File.Exists(fileName))
            {
                LogError("BaseTreeRoutinePlugin: Cannot find " + fileName + " file. This plugin will exit.", 10);
                return default;
            }

            return JsonConvert.DeserializeObject<TSettingType>(File.ReadAllText(fileName));
        }

        public static void SaveSettingFile<TSettingType>(String fileName, TSettingType setting)
        {
            string serialized = JsonConvert.SerializeObject(setting);

            File.WriteAllText(fileName, serialized);
        }

        public override void Initialise()
        {
            PluginName = "Base Tree Routine Plugin";
            
            Cache = new TCache();

            LoadSettingsFiles();
            InitializeHelpers();

            OnAreaChangeMethod(GameController.Area);
            OnPluginToggle();
            Settings.Enable.OnValueChanged += OnPluginToggle;
        }

        protected virtual void LoadSettingsFiles()
        {
            var flaskFilename = PluginDirectory + @"/config/flaskinfo.json";
            var debufFilename = "config/debuffPanel.json";
            var flaskBuffDetailsFileName = PluginDirectory + @"/config/FlaskBuffDetails.json";

            Cache.FlaskInfo = LoadSettingFile<FlaskInformation>(flaskFilename);
            Cache.DebuffPanelConfig = LoadSettingFile<DebuffPanelConfig>(debufFilename);
            Cache.MiscBuffInfo = LoadSettingFile<MiscBuffInfo>(flaskBuffDetailsFileName);
        }

        protected virtual void InitializeHelpers()
        {
            FlaskHelper.Core = this;
            PlayerHelper.Core = this;
            TreeHelper.Core = this;
        }

        protected virtual void OnPluginToggle()
        {
            try
            {
                if (Settings.Enable.Value)
                {
                    if (Settings.Debug.Value)
                        LogMessage(PluginName + ": Enabling " + PluginName + ".", LogmsgTime);

                    OnAreaChange += OnAreaChangeMethod;
                }
                else
                {
                    if (Settings.Debug.Value)
                        LogMessage(PluginName + ": Disabling " + PluginName + ".", LogmsgTime);

                    OnAreaChange += OnAreaChangeMethod;
                }
            }
            catch (Exception)
            {

                LogError(PluginName + ": Error Starting/Stopping " + PluginName + ".", ErrmsgTime);
            }
        }

        public void TickTree(Composite treeRoot)
        {
            try
            {
                if (!Settings.Enable)
                    return;

                if (Settings.Debug)
                    LogMessage(PluginName + ": Tick", LogmsgTime);

                if (treeRoot == null)
                {
                    if (Settings.Debug)
                        LogError("Plugin " + PluginName + " tree root function returned null. Plugin is either still initialising, or has an error.", ErrmsgTime);
                    return;
                }

                if (treeRoot.LastStatus != null)
                {
                    treeRoot.Tick(null);

                    // If the last status wasn't running, stop the tree, and restart it.
                    if (treeRoot.LastStatus != RunStatus.Running)
                    {
                        treeRoot.Stop(null);

                        UpdateCache();
                        treeRoot.Start(null);
                    }
                }
                else
                {
                    UpdateCache();
                    treeRoot.Start(null);
                    RunStatus status = treeRoot.Tick(null);
                }
            }
            catch (Exception e)
            {
                LogError(PluginName + ": Exception! Printscreen this and post it.\n" + e.Message + "\n" + e.StackTrace, 30);
                throw e;
            }
        }

        protected virtual void UpdateCache()
        {

        }

        

        protected void OnAreaChangeMethod(AreaController area)
        {
            if (Settings.Debug)
                LogMessage(PluginName + ": Area has been changed.", LogmsgTime);

            if (Settings.Enable.Value && area != null && area.CurrentArea != null)
            {
                Cache.InHideout = area.CurrentArea.IsHideout;
                Cache.InTown = area.CurrentArea.IsTown;
            }
        }
    }
}

