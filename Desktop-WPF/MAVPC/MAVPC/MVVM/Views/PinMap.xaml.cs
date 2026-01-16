using System.Windows.Controls;
using System.Windows.Media;

namespace MAVPC.MVVM.Views.Controls // Ajusta el namespace si lo creaste en otra carpeta
{
    public partial class PinMap : UserControl
    {
        public PinMap()
        {
            InitializeComponent();
        }

        // Método rápido para configurar el pin desde el ViewModel
        public void Configurar(string tipo)
        {
            switch (tipo.ToLower())
            {
                case "camara":
                    FondoPin.Fill = (Brush)new BrushConverter().ConvertFrom("#00D4FF"); // Cyan Neon
                    IconoPin.Kind = MaterialDesignThemes.Wpf.PackIconKind.Cctv;
                    break;

                case "incidencia":
                case "accidente":
                    FondoPin.Fill = (Brush)new BrushConverter().ConvertFrom("#FF5252"); // Rojo
                    IconoPin.Kind = MaterialDesignThemes.Wpf.PackIconKind.AlertCircle;
                    break;

                case "obra":
                case "mantenimiento":
                    FondoPin.Fill = (Brush)new BrushConverter().ConvertFrom("#FFAB40"); // Naranja
                    IconoPin.Kind = MaterialDesignThemes.Wpf.PackIconKind.Construction;
                    break;

                default:
                    FondoPin.Fill = Brushes.Gray;
                    IconoPin.Kind = MaterialDesignThemes.Wpf.PackIconKind.MapMarker;
                    break;
            }
        }
    }
}