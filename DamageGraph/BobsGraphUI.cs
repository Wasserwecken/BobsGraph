using BobsBuddy.Simulation;
using BobsGraph;
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
        /// <summary>
        /// 
        /// </summary>
        public BobsGraphUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simulationOutput"></param>
        public void Update(TestOutput result)
        {
            var viewModel = new BobsGraphViewModel();
            viewModel.UpdateSize(
                (float)GraphArea.ActualWidth,
                (float)GraphArea.ActualHeight,
                2);
            viewModel.UpdateData(result);

            DataContext = viewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            Visibility = Visibility.Visible;

            //Canvas.SetTop(this, Core.OverlayWindow.Top);
            //Canvas.SetRight(this, (Core.OverlayWindow.Width - GraphGrid.Width) / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }
    }
}
