using BobsBuddy.Simulation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BobsGraphPlugin
{
    /// <summary>
    /// Interaktionslogik für Graph.xaml
    /// </summary>
    public partial class BobsGraphUI : UserControl
    {
        private bool _isDragging;
        private Point _initialMousePosition;


        /// <summary>
        /// 
        /// </summary>
        public BobsGraphUI()
        {
            InitializeComponent();

            Canvas.SetTop(this, 0);
            Canvas.SetLeft(this, 0);
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
        public void Clear()
        {
            DataContext = new BobsGraphViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginDrag(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _isDragging = true;
            _initialMousePosition = e.GetPosition(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndDrag(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _isDragging = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Drag(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var newMousePosition = e.GetPosition(this);

                Canvas.SetLeft(this, Canvas.GetLeft(this) + (newMousePosition - _initialMousePosition).X);
                Canvas.SetTop(this, Canvas.GetTop(this) + (newMousePosition - _initialMousePosition).Y);
            }
        }
    }
}
