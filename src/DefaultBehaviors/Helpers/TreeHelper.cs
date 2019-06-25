using System;

namespace TreeRoutine.DefaultBehaviors.Helpers
{
    public class TreeHelper<TSettings, TCache>
        where TSettings : BaseTreeSettings, new()
        where TCache : BaseTreeCache, new()
    {
        public BaseTreeRoutinePlugin<TSettings, TCache> Core { get; set; }

        public Boolean CanTick()
        {
            if (Core.GameController.IsLoading)
            {
                //TreeRoutine.LogMessage("Game is loading...", 0.2f);
                return false;
            }
            if (!Core.GameController.Game.IngameState.ServerData.IsInGame)
            {
                //TreeRoutine.LogMessage("Currently not in the game (Charactor selection maybe).", 0.2f);
                return false;
            }
            else if (Core.GameController.Player == null || Core.GameController.Player.Address == 0 || !Core.GameController.Player.IsValid)
            {
                //TreeRoutine.LogMessage("Cannot find player info.", 0.2f);
                return false;
            }
            else if (!Core.GameController.Window.IsForeground())
            {
                //TreeRoutine.LogMessage("Poe is minimized.", 0.2f);
                return false;
            }
            else if (Core.Cache.InTown)
            {
                //TreeRoutine.LogMessage("Player is in town.", 0.2f);
                return false;
            }
            return true;
        }
    }
}
