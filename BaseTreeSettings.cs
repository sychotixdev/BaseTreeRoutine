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

        [Menu("Tick Rate", 3)]
        public RangeNode<int> TickRate { get; set; }

        [Menu("Strict Tick Rate", 4)]
        public ToggleNode StrictTickRate { get; set; }

        [Menu("Debug Mode", 100)]
        public ToggleNode Debug { get; set; }
    }
}
