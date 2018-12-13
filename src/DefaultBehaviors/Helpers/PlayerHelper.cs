using PoeHUD.Controllers;
using PoeHUD.Models.Enums;
using PoeHUD.Poe.Components;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeRoutine.DefaultBehaviors.Helpers
{
    public class PlayerHelper<TSettings, TCache>
        where TSettings : BaseTreeSettings, new()
        where TCache : BaseTreeCache, new()
    {
        public BaseTreeRoutinePlugin<TSettings, TCache> Core { get; set; }

        public Boolean isHealthBelowPercentage(int healthPercentage)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.HPPercentage * 100 < healthPercentage;
        }

        public Boolean isHealthBelowValue(int healthValue)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurHP < healthValue;
        }

        public Boolean isManaBelowPercentage(int manaPercentage)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MPPercentage * 100 < manaPercentage;
        }

        public Boolean isManaBelowValue(int manaValue)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurMana < manaValue;
        }

        public Boolean isEnergyShieldBelowPercentage(int energyShieldPercentage)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MaxES > 0 && playerLife.ESPercentage * 100 < energyShieldPercentage;
        }

        public Boolean isEnergyShieldBelowValue(int energyShieldValue)
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MaxES > 0 && playerLife.CurMana < energyShieldValue;
        }

        public Boolean playerHasBuffs(List<String> buffs)
        {
            if (buffs == null || buffs.Count == 0)
                return false;

            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            var playerBuffs = playerLife.Buffs;

            if (playerBuffs == null)
                return false;

            foreach (var buff in buffs)
            {
                if (!String.IsNullOrEmpty(buff) && !playerBuffs.Any(x => x.Name == buff))
                {
                    return false;
                }
            }
            return true;
        }


        public Boolean playerDoesNotHaveAnyOfBuffs(List<String> buffs)
        {
            if (buffs == null || buffs.Count == 0)
                return true;

            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            var playerBuffs = playerLife.Buffs;

            if (playerBuffs == null)
                return true;

            foreach (var buff in buffs)
            {
                if (!String.IsNullOrEmpty(buff) && playerBuffs.Any(x => x.Name == buff))
                {
                    return false;
                }
            }
            return true;
        }

        public Boolean isPlayerDead()
        {
            var playerLife = Core.GameController.Game.IngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurHP <= 0;
        }

        public int? getPlayerStat(string playerStat)
        {
            int statValue = 0;
            if (!Core.GameController.EntityListWrapper.PlayerStats.TryGetValue(GameController.Instance.Files.Stats.records[playerStat].ID, out statValue))
                return null;

            return statValue;
        }

        public void DrawSquareToWorld(Vector3 vector3Pos, float radius)
        {
            var rightMove = new Vector3(radius, 0, 0);
            var downMove = new Vector3(0, radius, 0);

            var PlayerCoords = Core.GameController.Game.IngameState.Camera.WorldToScreen(vector3Pos, Core.GameController.Player);

            Vector3.Add(ref vector3Pos, ref rightMove, out Vector3 rightPos);
            Vector2 rightScreenPos = Core.GameController.Game.IngameState.Camera.WorldToScreen(rightPos, Core.GameController.Player);

            Vector3.Add(ref vector3Pos, ref downMove, out Vector3 downPos);
            Vector2 downScreenPos = Core.GameController.Game.IngameState.Camera.WorldToScreen(downPos, Core.GameController.Player);

            var sizeX = (PlayerCoords.X - rightScreenPos.X);
            var sizeY = (PlayerCoords.Y - downScreenPos.Y);
            Core.Graphics.DrawFrame(new RectangleF(PlayerCoords.X - sizeX, PlayerCoords.Y - sizeY, sizeX * 2, sizeY * 2), 2, Color.Red);
        }


        public void DrawEllipseToWorld(Vector3 vector3Pos, int radius, int points, int lineWidth, Color color)
        {
            var camera = Core.GameController.Game.IngameState.Camera;

            var plottedCirclePoints = new List<Vector3>();
            var slice = 2 * Math.PI / points;
            for (var i = 0; i < points; i++)
            {
                var angle = slice * i;
                var x = (decimal)vector3Pos.X + decimal.Multiply((decimal)radius, (decimal)Math.Cos(angle));
                var y = (decimal)vector3Pos.Y + decimal.Multiply((decimal)radius, (decimal)Math.Sin(angle));
                plottedCirclePoints.Add(new Vector3((float)x, (float)y, vector3Pos.Z));
            }

            for (var i = 0; i < plottedCirclePoints.Count; i++)
            {
                if (i >= plottedCirclePoints.Count - 1)
                {
                    var pointEnd1 = camera.WorldToScreen(plottedCirclePoints.Last(), Core.GameController.Player);
                    var pointEnd2 = camera.WorldToScreen(plottedCirclePoints[0], Core.GameController.Player);
                    Core.Graphics.DrawLine(pointEnd1, pointEnd2, lineWidth, color);
                    return;
                }

                var point1 = camera.WorldToScreen(plottedCirclePoints[i], Core.GameController.Player);
                var point2 = camera.WorldToScreen(plottedCirclePoints[i + 1], Core.GameController.Player);
                Core.Graphics.DrawLine(point1, point2, lineWidth, color);
            }
        }
    }
}
