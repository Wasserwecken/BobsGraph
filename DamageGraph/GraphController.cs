using System.Collections.Generic;
using System.Linq;
using BobsBuddy.Simulation;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace DamageGraph
{
    public class GraphController
    {
        private readonly GraphUI _graphUI;

        /// <summary>
        /// 
        /// </summary>
        public GraphController(GraphUI graphUI)
        {
            _graphUI = graphUI;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public void TurnStart(ActivePlayer player)
        {
            if (ShouldEvaluate(player) && BobsBuddyProvider.TryGetTestOutput(out var output))
            {
                _graphUI.Update(EvaluateSimulation(output));
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
            if (turnNumber < 2)
            {
                Log.Info("There is no simulation for the first turn");
                return false;
            }

            if (player == ActivePlayer.Player && turnNumber != 2)
            {
                Log.Info("Shooping turn, simulation has been already run");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private static List<(int, int)> EvaluateSimulation(TestOutput simulationOutput)
        {
            return simulationOutput.result
                .GroupBy(trace => trace.damage)
                .Select(group => (Damage: group.Key, Count: group.Count() / simulationOutput.result.Count))
                .ToList();
        }
    }
}
