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
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.HPPercentage * 100 < healthPercentage;
        }

        public Boolean isHealthBelowValue(int healthValue)
        {
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurHP < healthValue;
        }

        public Boolean isManaBelowPercentage(int manaPercentage)
        {
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MPPercentage * 100 < manaPercentage;
        }

        public Boolean isManaBelowValue(int manaValue)
        {
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurMana < manaValue;
        }

        public Boolean isEnergyShieldBelowPercentage(int energyShieldPercentage)
        {
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MaxES > 0 && playerLife.ESPercentage * 100 < energyShieldPercentage;
        }

        public Boolean isEnergyShieldBelowValue(int energyShieldValue)
        {
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.MaxES > 0 && playerLife.CurMana < energyShieldValue;
        }

        public Boolean playerHasBuffs(List<String> buffs)
        {
            if (buffs == null || buffs.Count == 0)
                return false;

            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
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

            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
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
            var playerLife = Core.Cache.SavedIngameState.Data.LocalPlayer.GetComponent<Life>();
            return playerLife.CurHP <= 0;
        }

        public int? getPlayerStat(PlayerStats playerStat)
        {
            int statValue = 0;
            if (!Core.GameController.EntityListWrapper.PlayerStats.TryGetValue(playerStat, out statValue))
                return null;

            return statValue;
        }

        public void RenderRangeFromPlayer(float range)
        {
            var playerPosition = new Vector3(Core.GameController.Player.GetComponent<Positioned>().WorldPos, Core.GameController.Player.Pos.Z+50);

            var rightMove = new Vector3(range, 0, 0);
            var downMove = new Vector3(0, range, 0);

            var PlayerCoords = Core.GameController.Game.IngameState.Camera.WorldToScreen(playerPosition, Core.GameController.Player);

            Vector3.Add(ref playerPosition, ref rightMove, out Vector3 rightPos);
            Vector2 rightScreenPos = Core.GameController.Game.IngameState.Camera.WorldToScreen(rightPos, Core.GameController.Player);

            Vector3.Add(ref playerPosition, ref downMove, out Vector3 downPos);
            Vector2 downScreenPos = Core.GameController.Game.IngameState.Camera.WorldToScreen(rightPos, Core.GameController.Player);

            var sizeX = (PlayerCoords.X - rightScreenPos.X);
            var sizeY = (PlayerCoords.Y - rightScreenPos.Y);
            Core.Graphics.DrawFrame(new RectangleF(PlayerCoords.X - sizeX, PlayerCoords.Y - sizeY, sizeX * 2, sizeY * 2), 2, Color.Red);

        }
    }
}
