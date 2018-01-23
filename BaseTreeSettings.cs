using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeRoutine
{
    public class BaseTreeSettings : SettingsBase
    {

        public BaseTreeSettings()
        {
            Enable = false;
            Debug = false;
            TickRate = new RangeNode<int>(50, 15, 1000);
            StrictTickRate = false;
        }

        [Menu("Tick Rate", 3, Tooltip = "Milliseconds between every tick of plugin.")]
        public RangeNode<int> TickRate { get; set; }

        [Menu("Strict Tick Rate", 4, Tooltip = "Enable to force a strict tick rate. This will ensure the ticks are at a constant timing, but may cause ticks to overlap as the previous tick may not have finished. Enable only if you have a reason to.")]
        public ToggleNode StrictTickRate { get; set; }

        [Menu("Debug Mode", 100, Tooltip = "Enables debug logging to help debug flask issues.")]
        public ToggleNode Debug { get; set; }
    }
}
