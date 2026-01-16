using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Specialized;
using System.ComponentModel;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MAVPC.Models;
using MAVPC.MVVM.ViewModels;

namespace MAVPC.MVVM.Views
{
    public partial class MapView : UserControl
    {
        private MapViewModel? _viewModel;

        public MapView()
        {
            InitializeComponent();
            this.Unloaded += MapView_Unloaded;

            // --- OPTIMIZACIÓN DE ARRASTRE ---
            // "ShowMarkers" no existe en WPF, usamos nuestra función ToggleMarkers
            MainMap.MouseLeftButtonDown += (s, e) => ToggleMarkers(false);
            MainMap.MouseLeftButtonUp += (s, e) => ToggleMarkers(true);

            // Soporte para botón derecho
            MainMap.MouseRightButtonDown += (s, e) => ToggleMarkers(false);
            MainMap.MouseRightButtonUp += (s, e) => ToggleMarkers(true);

            // Evento para el Zoom
            MainMap.OnMapZoomChanged += MainMap_OnMapZoomChanged;
        }

        private void MainMap_Loaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            MainMap.MapProvider = GMapProviders.ArcGIS_World_Street_Map;
            MainMap.Position = new PointLatLng(42.8467, -2.6716);
            MainMap.DragButton = MouseButton.Left;
            MainMap.ShowCenter = false;

            if (DataContext is MapViewModel vm)
            {
                _viewModel = vm;
                ActualizarMapaCompleto();

                // Suscripciones
                _viewModel.Markers.CollectionChanged += OnMarkersCollectionChanged;
                _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        // Detectar selección en el buscador
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapViewModel.SelectedResult))
            {
                var result = _viewModel?.SelectedResult;
                if (result != null)
                {
                    // 1. Movemos el mapa
                    MainMap.Position = new PointLatLng(result.Lat, result.Lon);
                    MainMap.Zoom = 15; // Ajustado a un zoom más cercano para ver detalle

                    // 2. Abrimos la ventana correspondiente
                    if (result.DataObject is Camara cam) new CameraWindow(cam).Show();
                    else if (result.DataObject is Incidencia inc) new IncidentWindow(inc).Show();
                }
            }
        }

        private void MapView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Markers.CollectionChanged -= OnMarkersCollectionChanged;
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
                MainMap.Dispose();
            }
        }

        private void OnMarkersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (GMapMarker m in e.NewItems) { MainMap.Markers.Add(m); ConfigurarClickMarcador(m); }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                MainMap.Markers.Clear();
            }
        }

        private void ActualizarMapaCompleto()
        {
            MainMap.Markers.Clear();
            if (_viewModel != null)
            {
                foreach (GMapMarker marker in _viewModel.Markers) { MainMap.Markers.Add(marker); ConfigurarClickMarcador(marker); }
            }
        }

        private void ConfigurarClickMarcador(GMapMarker marker)
        {
            if (marker.Shape is UIElement shape)
            {
                shape.MouseLeftButtonDown += (s, e) =>
                {
                    if (marker.Tag is Camara camaraSeleccionada)
                    {
                        new CameraWindow(camaraSeleccionada).Show();
                        e.Handled = true;
                    }
                    else if (marker.Tag is Incidencia incidenciaSeleccionada)
                    {
                        new IncidentWindow(incidenciaSeleccionada).Show();
                        e.Handled = true;
                    }
                };
            }
        }

        // --- LÓGICA DE OPTIMIZACIÓN (Reemplaza a ShowMarkers) ---

        private void MainMap_OnMapZoomChanged()
        {
            ToggleMarkers(false); // Ocultar

            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(300); // Esperar a que renderice
                ToggleMarkers(true);   // Mostrar
            });
        }

        // Esta función hace el trabajo sucio de ocultar los iconos uno a uno
        private void ToggleMarkers(bool visible)
        {
            var visibility = visible ? Visibility.Visible : Visibility.Collapsed;

            foreach (var marker in MainMap.Markers)
            {
                if (marker.Shape != null)
                {
                    marker.Shape.Visibility = visibility;
                }
            }
        }
    }
}