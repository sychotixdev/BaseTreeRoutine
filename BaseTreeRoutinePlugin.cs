using System;
using System.IO;
using System.Threading;
using TreeRoutine.DefaultBehaviors.Helpers;
using TreeRoutine.FlaskComponents;
using Newtonsoft.Json;
using PoeHUD.Controllers;
using PoeHUD.Hud.Health;
using PoeHUD.Plugins;
using TreeSharp;
using TreeRoutine.Menu;
using ImGuiNET;

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
                LogError("Cannot find " + fileName + " file. This plugin will exit.", 10);
                return default(TSettingType);
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
                        LogMessage("Enabling " + PluginName + ".", LogmsgTime);

                    GameController.Area.OnAreaChange += OnAreaChange;
                }
                else
                {
                    if (Settings.Debug.Value)
                        LogMessage("Disabling " + PluginName + ".", LogmsgTime);

                    GameController.Area.OnAreaChange -= OnAreaChange;
                }
            }
            catch (Exception)
            {

                LogError("Error Starting/Stopping " + PluginName + ".", ErrmsgTime);
            }
        }

        public void TickTree(Composite treeRoot)
        {
            try
            {
                if (Settings.Debug)
                    LogMessage("Tick", LogmsgTime);

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
                LogError("Exception! Printscreen this and post it.\n" + e.StackTrace, 30);
                throw e;
            }
        }

        protected virtual void UpdateCache()
        {

        }

        

        protected virtual void OnAreaChange(AreaController area)
        {
            if (Settings.Enable.Value)
            {
                if (Settings.Debug)
                    LogMessage("Area has been changed.", LogmsgTime);

                Cache.InHideout = area.CurrentArea.IsHideout;
                Cache.InTown = area.CurrentArea.IsTown;
            }
        }

        public override void Render()
        {
            base.Render();
            RunWindow();
        }

        protected virtual void RunWindow()
        {
            if (!Settings.ShowSettings) return;
            ImGuiExtension.BeginWindow($"{PluginName} Settings", Settings.LastSettingPos.X, Settings.LastSettingPos.Y, Settings.LastSettingSize.X, Settings.LastSettingSize.Y);

            Settings.Debug.Value = ImGuiExtension.Checkbox("Debug Mode", Settings.Debug);

            // Storing window Position and Size changed by the user
            if (ImGui.GetWindowHeight() > 21)
            {
                Settings.LastSettingPos = ImGui.GetWindowPosition();
                Settings.LastSettingSize = ImGui.GetWindowSize();
            }

            ImGui.EndWindow();
        }
    }
}

