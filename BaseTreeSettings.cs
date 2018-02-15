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

            var centerPos = BasePlugin.API.GameController.Window.GetWindowRectangle().Center;
            LastSettingSize = new ImGuiVector2(620, 376);
            LastSettingPos = new ImGuiVector2(centerPos.X - LastSettingSize.X / 2, centerPos.Y - LastSettingSize.Y / 2);
        }



        [Menu("Show Settings")]
        public ToggleNode ShowSettings { get; set; } = false;

        [Menu("Show Profile Menu")]
        public ToggleNode ShowProfileMenu { get; set; } = false;

        public ImGuiVector2 LastSettingPos { get; set; }
        public ImGuiVector2 LastSettingSize { get; set; }

        public ToggleNode Debug { get; set; } = false;
    }
}
