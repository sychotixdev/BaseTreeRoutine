using PoeHUD.Hud.Settings;
using PoeHUD.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiVector2 = System.Numerics.Vector2;
using ImGuiVector4 = System.Numerics.Vector4;

namespace TreeRoutine
{
    public class BaseTreeSettings : SettingsBase
    {

        public BaseTreeSettings()
        {
            Enable = false;
        }

       

        [Menu("Show ImGui Settings")]
        public ToggleNode ShowWindow { get; set; }
        public ImGuiVector2 LastSettingPos { get; set; }
        public ImGuiVector2 LastSettingSize { get; set; }

        public RangeNode<int> TickRate { get; set; } = new RangeNode<int>(50, 15, 1000);
        public ToggleNode StrictTickRate { get; set; } = false;
        public ToggleNode Debug { get; set; } = false;
    }
}
