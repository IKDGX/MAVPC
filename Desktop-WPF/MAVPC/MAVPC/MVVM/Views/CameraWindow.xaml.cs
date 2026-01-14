using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using MAVPC.Models; // Asegúrate de que esto apunta a tus modelos

namespace MAVPC
{
    public partial class CameraWindow : Window
    {
        public CameraWindow(Camara camara)
        {
            InitializeComponent();
            CargarDatos(camara);
        }

        private void CargarDatos(Camara cam)
        {
            TxtTitulo.Text = cam.Nombre;
            TxtUbicacion.Text = $"{cam.Carretera} - {cam.Direccion}";
            TxtPk.Text = $"PK {cam.Kilometro}";
            TxtCoordenadas.Text = $"X: {cam.Latitud} | Y: {cam.Longitud}";

            if (!string.IsNullOrEmpty(cam.UrlImagen))
            {
                // Cargar imagen
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(cam.UrlImagen);
                bitmap.EndInit();
                ImgMonitor.Source = bitmap;
            }
            else
            {
                // Si es nula, cambiamos el badge a "SIN SEÑAL"
                BadgeEstado.Background = new SolidColorBrush(Color.FromArgb(30, 255, 0, 0)); // Rojo translúcido
                BadgeEstado.BorderBrush = Brushes.Red;
                ((System.Windows.Controls.TextBlock)BadgeEstado.Child).Text = "OFFLINE";
                ((System.Windows.Controls.TextBlock)BadgeEstado.Child).Foreground = Brushes.Red;
            }
        }

        // Permite arrastrar la ventana
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}