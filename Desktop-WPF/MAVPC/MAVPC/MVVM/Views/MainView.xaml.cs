using System.Windows;
using System.Windows.Input;

namespace MAVPC.MVVM.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                GMap.NET.GMaps.Instance.CancelTileCaching();
            }
            catch { /* Ignorar si falla */ }

            // Mata la aplicación completa inmediatamente
            Environment.Exit(0);
        }
    }
}