using BobsBuddy;
using BobsBuddy.Simulation;
using BobsGraphPlugin;
using System;
using System.Windows;

namespace GraphTest
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random _rand;
        private BobsGraphUI _graphUI;

        public MainWindow()
        {
            InitializeComponent();

            _rand = new Random();
            _graphUI = new BobsGraphUI();
            TestCanvas.Children.Add(_graphUI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate(object sender, RoutedEventArgs e)
        {
            var min = _rand.Next(-10, 5);
            var max = Math.Max(min, _rand.Next(-5, 10));

            var fakeResult = new TestOutput();
            for (int i = 0; i < 100; i++)
            {
                fakeResult.result.Add(new FightTrace()
                {
                    damage = _rand.Next(min, max)
                });
            }

            _graphUI.Update(fakeResult);
        }
    }
}
