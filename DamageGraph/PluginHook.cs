using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;

namespace BobsGraphPlugin
{
    public class PluginHook : IPlugin
    {
        public string Name => "Damage graph";
        public string Description => "Shows the possible damage distribution of the simulation";
        public string ButtonText => "";
        public string Author => "Wasserwecken";
        public Version Version => new Version(0, 0, 1);
        public MenuItem MenuItem => null;

        private BobsGraphUI _graphUI;
        private BobyGraphController _graphController;

        /// <summary>
        /// Triggered upon startup and when the user ticks the plugin on
        /// </summary>
        public void OnLoad()
        {
            GameEvents.OnGameStart.Add(PrepareIfBattleGrounds);
        }

        /// <summary>
        /// Triggered when the user unticks the plugin, however, HDT does not completely unload the plugin.
        /// see https://git.io/vxEcH
        /// </summary>
        public void OnUnload()
        {
            if (_graphUI != null) Core.OverlayCanvas.Children.Remove(_graphUI);
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
        /// Prepares the plugin ind UI for the turns
        /// </summary>
        private void PrepareIfBattleGrounds()
        {
            if (Core.Game.IsBattlegroundsMatch)
            {
                _graphUI = new BobsGraphUI();
                _graphController = new BobyGraphController(_graphUI);

                Core.OverlayCanvas.Children.Add(_graphUI);
                GameEvents.OnTurnStart.Add(_graphController.TurnStart);
            }
        }
    }
}
