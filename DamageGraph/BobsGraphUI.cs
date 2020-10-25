using BobsBuddy.Simulation;
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
        public void Update(TestOutput simulationOutput)
        {
            DataContext = new BobsGraphViewModel(
                simulationOutput,
                (float)GraphArea.ActualWidth,
                (float)GraphArea.ActualHeight,
                2
            );
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
    }
}
