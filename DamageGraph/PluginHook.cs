using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace BobsGraphPlugin
{
    public class PluginHook : IPlugin
    {
        public string Name => "Bobs graph";
        public string Description => "Shows the possible damage distribution of the simulation";
        public string ButtonText => "";
        public string Author => "Wasserwecken";
        public Version Version => new Version(0, 0, 1);
        public MenuItem MenuItem => null;

        private BobsGraphUI _graphUI;

        /// <summary>
        /// Triggered upon startup and when the user ticks the plugin on
        /// </summary>
        public void OnLoad()
        {
            GameEvents.OnGameStart.Add(PrepareIfBattleGrounds);
            GameEvents.OnInMenu.Add(Deactivate);

            PrepareIfBattleGrounds();
        }

        /// <summary>
        /// Triggered when the user unticks the plugin, however, HDT does not completely unload the plugin.
        /// see https://git.io/vxEcH
        /// </summary>
        public void OnUnload()
        {
            Deactivate();
        }

        /// <summary>
        /// Triggered when the user clicks your button in the plugin list
        /// </summary>
        public void OnButtonPress() { }

        /// <summary>
        /// called every ~100ms
        /// </summary>
        public void OnUpdate() { }


        /// <summary>
        /// Prepares the plugin and UI for the turns
        /// </summary>
        private void PrepareIfBattleGrounds()
        {
            if (Core.Game.IsBattlegroundsMatch)
            {
                if (_graphUI == null)
                {
                    _graphUI = new BobsGraphUI();
                }

                Core.OverlayCanvas.Children.Add(_graphUI);
                GameEvents.OnTurnStart.Add(HandleTurnStart);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Deactivate()
        {
            if (_graphUI != null)
            {
                Core.OverlayCanvas.Children.Remove(_graphUI);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void HandleTurnStart(ActivePlayer player)
        {
            var turn = Core.Game.GetTurnNumber() - player == ActivePlayer.Player ? 1 : 0;

            if (BobsBuddyProvider.TryGetTestOutput(turn, out var result))
            {
                Log.Info("Showing simulaion result in graph");
                _graphUI.Update(result);
            }
            else
            {
                Log.Info("Unable to get simulation result for graph");
            }
        }
    }
}
