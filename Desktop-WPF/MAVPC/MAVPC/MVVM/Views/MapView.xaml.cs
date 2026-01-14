using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation; // Necesario para GMapMarker
using MAVPC.MVVM.ViewModels;        // Necesario para ver el MapViewModel
using System.Collections.Specialized; // Necesario para detectar cambios en la lista
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MAVPC.MVVM.Views
{
    public partial class MapView : UserControl
    {
        public MapView()
        {
            InitializeComponent();
        }

        private void MainMap_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Configuración obligatoria
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = new PointLatLng(42.8467, -2.6716); // Vitoria
            MainMap.DragButton = MouseButton.Left;
            MainMap.ShowCenter = false;

            // 2. CONEXIÓN MANUAL CON EL VIEWMODEL
            if (DataContext is MapViewModel vm)
            {
                // Limpiamos por si acaso
                MainMap.Markers.Clear();

                // Añadimos los marcadores que ya tenga el ViewModel cargados
                foreach (var marker in vm.Markers)
                {
                    MainMap.Markers.Add(marker);
                }

                // 3. (Opcional pero recomendado) Si la lista cambia en el futuro, actualizamos el mapa
                vm.Markers.CollectionChanged += (s, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (GMapMarker m in args.NewItems!) MainMap.Markers.Add(m);
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Reset)
                    {
                        MainMap.Markers.Clear();
                    }
                };
            }
        }
    }
}