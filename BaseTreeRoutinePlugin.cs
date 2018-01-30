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

        protected Composite Tree { get; set; } = new TreeSharp.Action();

        public Action<object, float> Log => LogMessage;
        public Action<object, float> LogErr => LogError;

        public FlaskHelper<TSettings, TCache> FlaskHelper { get; set; } = new FlaskHelper<TSettings, TCache>();
        public PlayerHelper<TSettings, TCache> PlayerHelper { get; set; } = new PlayerHelper<TSettings, TCache>();
        public TreeHelper<TSettings, TCache> TreeHelper { get; set; } = new TreeHelper<TSettings, TCache>();



        public override void Initialise()
        {
            PluginName = "Base Tree Routine Plugin";
            var flaskFilename = PluginDirectory + @"/config/flaskinfo.json";
            var debufFilename = "config/debuffPanel.json";
            var flaskBuffDetailsFileName = PluginDirectory + @"/config/FlaskBuffDetails.json";

            Cache = new TCache();

            if (!File.Exists(debufFilename))
            {
                LogError("Cannot find " + debufFilename + " file. This plugin will exit.", ErrmsgTime);
                return;
            }

            if (!File.Exists(flaskFilename))
            {
                LogError("Cannot find " + flaskFilename + " file. This plugin will exit.", ErrmsgTime);
                return;
            }

            if (!File.Exists(flaskBuffDetailsFileName))
            {
                LogError("Cannot find " + flaskBuffDetailsFileName + " file. This plugin will exit.", ErrmsgTime);
                return;
            }

            Cache.FlaskInfo = JsonConvert.DeserializeObject<FlaskInformation>(File.ReadAllText(flaskFilename));
            Cache.DebuffPanelConfig = JsonConvert.DeserializeObject<DebuffPanelConfig>(File.ReadAllText(debufFilename));
            Cache.MiscBuffInfo = JsonConvert.DeserializeObject<MiscBuffInfo>(File.ReadAllText(flaskBuffDetailsFileName));

            InitializeHelpers();

            OnPluginToggle();
            Settings.Enable.OnValueChanged += OnPluginToggle;
            Settings.TickRate.OnValueChanged += RestartTimer;
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

                    StartTimer();
                }
                else
                {
                    if (Settings.Debug.Value)
                        LogMessage("Disabling " + PluginName + ".", LogmsgTime);
                    StopTimer();

                    GameController.Area.OnAreaChange -= OnAreaChange;
                }
            }
            catch (Exception)
            {

                LogError("Error Starting/Stopping " + PluginName + ".", ErrmsgTime);
            }
        }
        
        protected void StartTimer()
        {
            if (Timer == null)
            {
                Timer = new Timer(Tick);
            }


            Cache.TickRate = Settings.TickRate;
            // If we are on a strict tickrate, we will use the period on the timer. Otherwise, we will start the timer back up after every tick
            Timer.Change(Settings.TickRate, (Settings.StrictTickRate ? Settings.TickRate : Timeout.Infinite));
        }

        protected void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            if (Tree != null)
                Tree.Stop(null);
        }

        private void RestartTimer()
        {
            StopTimer();
            StartTimer();
        }

        public void Tick(Object stateInfo)
        {
            if (Settings.Enable)
            {
                if (Settings.Debug)
                    LogMessage("Tick", LogmsgTime);

                if (Tree == null)
                {
                    if (Settings.Debug)
                        LogError("Plugin " + PluginName + " tree root was null. Plugin is either still initialising, or has an error.", ErrmsgTime);
                    return;
                }

                if (Tree.LastStatus != null)
                {
                    Tree.Tick(null);

                    // If the last status wasn't running, stop the tree, and restart it.
                    if (Tree.LastStatus != RunStatus.Running)
                    {
                        Tree.Stop(null);

                        UpdateCache();
                        Tree.Start(null);
                    }
                }
                else
                {
                    UpdateCache();
                    Tree.Start(stateInfo);
                    RunStatus status = Tree.Tick(stateInfo);
                }
            }

            // If we are not on a strict tickrate, then we need to start the timer again
            if (!Settings.StrictTickRate)
            {
                StartTimer();
            }
        }

        protected virtual void UpdateCache()
        {
            Cache.SavedIngameState = GameController.Game.IngameState;
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
            if (!Settings.ShowWindow) return;
            ImGuiExtension.BeginWindow($"{PluginName} Settings", Settings.LastSettingPos.X, Settings.LastSettingPos.Y, Settings.LastSettingSize.X, Settings.LastSettingSize.Y);

            Settings.TickRate.Value = ImGuiExtension.IntSlider("Tick Rate", Settings.TickRate);
            Settings.StrictTickRate.Value = ImGuiExtension.Checkbox("Strict Tick Rate", Settings.StrictTickRate);
            ImGui.Separator();
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

