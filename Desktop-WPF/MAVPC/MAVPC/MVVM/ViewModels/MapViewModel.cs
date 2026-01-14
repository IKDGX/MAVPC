using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using MAVPC.Models;
using MAVPC.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization; // Necesario para los decimales (puntos vs comas)
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MAVPC.MVVM.ViewModels
{
    public partial class MapViewModel : ObservableObject
    {
        private readonly ITrafficService _trafficService;

        [ObservableProperty]
        private ObservableCollection<GMapMarker> _markers = new();

        public MapViewModel(ITrafficService trafficService)
        {
            _trafficService = trafficService;
            LoadMapDataCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadMapData()
        {
            Markers.Clear();

            try
            {
                // Usamos InvariantCulture para asegurar que "42.5" se lee como decimal aunque tu PC esté en español
                var culture = CultureInfo.InvariantCulture;

                // 1. INCIDENCIAS (Rojo)
                var incidencias = await _trafficService.GetIncidenciasAsync();
                if (incidencias != null)
                {
                    foreach (var inc in incidencias)
                    {
                        if (double.TryParse(inc.Latitud, NumberStyles.Any, culture, out double lat) &&
                            double.TryParse(inc.Longitud, NumberStyles.Any, culture, out double lon))
                        {
                            // Filtramos coordenadas 0,0 o inválidas
                            if (lat != 0 && lon != 0)
                            {
                                AddMarker(lat, lon, Brushes.Red, $"Incidencia: {inc.Tipo}\n{inc.Carretera}", 15);
                            }
                        }
                    }
                }

                // 2. CÁMARAS (Azul) - Vienen en UTM
                var camaras = await _trafficService.GetCamarasAsync();
                if (camaras != null)
                {
                    foreach (var cam in camaras)
                    {
                        if (double.TryParse(cam.Latitud, NumberStyles.Any, culture, out double utmY) &&
                            double.TryParse(cam.Longitud, NumberStyles.Any, culture, out double utmX))
                        {
                            // Zona 30 Norte es la estándar para Euskadi
                            var (lat, lon) = UtmToLatLon(utmX, utmY, 30, true);

                            // Solo pintamos si la conversión dio un resultado válido (Euskadi está aprox entre Lat 42 y 44)
                            if (lat > 40 && lat < 45 && lon > -5 && lon < -1)
                            {
                                AddMarker(lat, lon, Brushes.Blue, $"Cámara: {cam.Nombre}", 10);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log silencioso o MessageBox si es crítico
                System.Diagnostics.Debug.WriteLine($"Error mapa: {ex.Message}");
            }
        }

        private void AddMarker(double lat, double lon, Brush color, string tooltip, double size)
        {
            GMapMarker marker = new GMapMarker(new PointLatLng(lat, lon));

            marker.Shape = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = color,
                Stroke = Brushes.White,
                StrokeThickness = 2,
                ToolTip = tooltip
            };

            marker.Offset = new Point(-size / 2, -size / 2);
            Markers.Add(marker);
        }

        // --- MATEMÁTICAS: Conversión UTM a Lat/Lon (WGS84) ---
        private (double Lat, double Lon) UtmToLatLon(double x, double y, int zone, bool north)
        {
            try
            {
                const double a = 6378137; // Radio mayor WGS84
                const double e = 0.081819191; // Excentricidad
                const double k0 = 0.9996; // Factor de escala

                double m0 = north ? 0 : 10000000;
                double arc = y - m0; // Distancia meridional

                double mu = arc / (a * (1 - Math.Pow(e, 2) / 4.0 - 3 * Math.Pow(e, 4) / 64.0 - 5 * Math.Pow(e, 6) / 256.0) * k0);

                double e1 = (1 - Math.Sqrt(1 - Math.Pow(e, 2))) / (1 + Math.Sqrt(1 - Math.Pow(e, 2)));

                double J1 = (3 * e1 / 2 - 27 * Math.Pow(e1, 3) / 32);
                double J2 = (21 * Math.Pow(e1, 2) / 16 - 55 * Math.Pow(e1, 4) / 32);
                double J3 = (151 * Math.Pow(e1, 3) / 96);
                double J4 = (1097 * Math.Pow(e1, 4) / 512);

                double fp = mu + J1 * Math.Sin(2 * mu) + J2 * Math.Sin(4 * mu) + J3 * Math.Sin(6 * mu) + J4 * Math.Sin(8 * mu);

                double C1 = Math.Pow(e, 2) * Math.Pow(Math.Cos(fp), 2) / (1 - Math.Pow(e, 2));
                double T1 = Math.Pow(Math.Tan(fp), 2);
                double R1 = a * (1 - Math.Pow(e, 2)) / Math.Pow(1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2), 1.5);
                double N1 = a / Math.Sqrt(1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(fp), 2));
                double D = (x - 500000) / (N1 * k0);

                double Q1 = N1 * Math.Tan(fp) / R1;
                double Q2 = D * D / 2;
                double Q3 = (5 + 3 * T1 + 10 * C1 - 4 * C1 * C1 - 9 * Math.Pow(e, 2)) * Math.Pow(D, 4) / 24;
                double Q4 = (61 + 90 * T1 + 298 * C1 + 45 * T1 * T1 - 252 * Math.Pow(e, 2) - 3 * C1 * C1) * Math.Pow(D, 6) / 720;

                double latRad = fp - Q1 * (Q2 - Q3 + Q4);

                double Q5 = D;
                double Q6 = (1 + 2 * T1 + C1) * Math.Pow(D, 3) / 6;
                double Q7 = (5 - 2 * C1 + 28 * T1 - 3 * Math.Pow(C1, 2) + 8 * Math.Pow(e, 2) + 24 * Math.Pow(T1, 2)) * Math.Pow(D, 5) / 120;

                double lonRad = (Q5 - Q6 + Q7) / Math.Cos(fp);
                double lonOrigin = (zone * 6 - 183) * Math.PI / 180;

                double lat = latRad * 180 / Math.PI;
                double lon = (lonRad + lonOrigin) * 180 / Math.PI;

                return (lat, lon);
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}