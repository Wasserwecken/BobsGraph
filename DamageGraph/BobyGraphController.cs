﻿using System.Collections.Generic;
using System.Linq;
using BobsBuddy.Simulation;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace BobsGraphPlugin
{
    public class BobyGraphController
    {
        private readonly BobsGraphUI _graphUI;

        /// <summary>
        /// 
        /// </summary>
        public BobyGraphController(BobsGraphUI graphUI)
        {
            _graphUI = graphUI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void TurnStart(ActivePlayer player)
        {
            var turn = Core.Game.GetTurnNumber();
            if (ShouldEvaluate(player) && BobsBuddyProvider.TryGetTestOutput(turn, out var output))
            {
                _graphUI.Update(output);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool ShouldEvaluate(ActivePlayer player)
        {
            var turnNumber = Core.Game.GetTurnNumber();
            if (turnNumber < 1)
            {
                Log.Info("There is no simulation for the first turn");
                return false;
            }

            return true;
        }
    }
}
