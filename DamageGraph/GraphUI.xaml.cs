using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace DamageGraph
{
    /// <summary>
    /// Interaktionslogik für Graph.xaml
    /// </summary>
    public partial class GraphUI : UserControl
    {
        public double TieLinePosition { get; private set; }
        public Visibility TieLineVisibility { get; private set; }
        public PointCollection GraphPoints { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public GraphUI()
        {
            GraphPoints = new PointCollection();
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageDistribution"></param>
        public void Update(IEnumerable<(int, int)> damageDistribution)
        {
            damageDistribution.OrderBy(damage => damage.Item1);

            Log.Info($"Points: {damageDistribution.Count()}");

            var min = damageDistribution.First().Item1;
            var max = damageDistribution.Last().Item1;
            var diff = Math.Max(0.001d, max - min);

            TieLineVisibility = (min < 0 && max > 0) ? Visibility.Visible : Visibility.Hidden;
            TieLinePosition = max / diff * GraphGrid.Height;
            Log.Info($"Tie position: {TieLinePosition}");

            GraphPoints.Clear();
            double x = 0;
            foreach (var damage in damageDistribution)
            {
                double y = (1d - (damage.Item1 / diff)) * GraphGrid.Height;
                Log.Info($"Point x:{x * GraphGrid.Width}, y:{y}");
                GraphPoints.Add(new Point(x * GraphGrid.Width, y));

                x += damage.Item2;
                Log.Info($"Point x:{x * GraphGrid.Width}, y:{y}");
                GraphPoints.Add(new Point(x * GraphGrid.Width, y));
            }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            Canvas.SetTop(this, Core.OverlayWindow.Top);
            Canvas.SetRight(this, (Core.OverlayWindow.Width - GraphGrid.Width) / 2);
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }
    }
}
