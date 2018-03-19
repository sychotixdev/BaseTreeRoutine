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
        }

        [Menu("Debug")]
        public ToggleNode Debug { get; set; } = false;
    }
}
