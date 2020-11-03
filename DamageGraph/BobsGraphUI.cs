using BobsBuddy.Simulation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BobsGraphPlugin
{
    /// <summary>
    /// Interaktionslogik für Graph.xaml
    /// </summary>
    public partial class BobsGraphUI : UserControl
    {
        private BobsGraphViewModel _viewModel;

        /// <summary>
        /// 
        /// </summary>
        public BobsGraphUI()
        {
            InitializeComponent();

            _viewModel = new BobsGraphViewModel();
            DataContext = _viewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationOutput"></param>
        public void Update(TestOutput simulationOutput)
        {
            _viewModel.Prepare(simulationOutput);
            _viewModel.Show(
                (float)GraphArea.ActualWidth,
                (float)GraphArea.ActualHeight,
                2);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            //Canvas.SetTop(this, Core.OverlayWindow.Top);
            //Canvas.SetRight(this, (Core.OverlayWindow.Width - GraphGrid.Width) / 2);
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        private void SwitchDiagram(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.ShowNextDiagram(
                (float)GraphArea.ActualWidth,
                (float)GraphArea.ActualHeight,
                2);

            var foo = _viewModel;
            DataContext = null;
            DataContext = foo;
        }
    }
}
