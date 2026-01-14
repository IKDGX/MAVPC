using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using MAVPC.Services;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MAVPC.MVVM.ViewModels
{
    public partial class StatsViewModel : ObservableObject
    {
        private readonly ITrafficService _trafficService;

        // Propiedades de Datos
        [ObservableProperty] private SeriesCollection _incidenciasSeries;
        [ObservableProperty] private SeriesCollection _camarasSeries;
        [ObservableProperty] private SeriesCollection _carreterasSeries; // <--- NUEVO

        [ObservableProperty] private string[] _labels;
        [ObservableProperty] private string[] _carreterasLabels; // <--- NUEVO

        [ObservableProperty] private Func<double, string> _formatter; // Para quitar decimales

        public StatsViewModel(ITrafficService trafficService)
        {
            _trafficService = trafficService;

            // Inicializar para evitar nulos
            IncidenciasSeries = new SeriesCollection();
            CamarasSeries = new SeriesCollection();
            CarreterasSeries = new SeriesCollection();

            // Formateador: Convierte 10.0 en "10"
            Formatter = value => value.ToString("N0");

            LoadStats();
        }

        private async void LoadStats()
        {
            try
            {
                var incidencias = await _trafficService.GetIncidenciasAsync();
                var camaras = await _trafficService.GetCamarasAsync();

                if (incidencias == null || camaras == null) return;

                // 1. GRÁFICO CIRCULAR (Tipos)
                var incidenciasPorTipo = incidencias
                    .GroupBy(x => x.Tipo)
                    .Select(g => new { Tipo = g.Key, Cantidad = g.Count() });

                var pieSeries = new SeriesCollection();
                foreach (var item in incidenciasPorTipo)
                {
                    pieSeries.Add(new PieSeries
                    {
                        Title = item.Tipo,
                        Values = new ChartValues<int> { item.Cantidad },
                        DataLabels = true,
                        LabelPoint = chartPoint => string.Format("{0} ({1:P})", chartPoint.SeriesView.Title, chartPoint.Participation)
                    });
                }
                IncidenciasSeries = pieSeries;

                // 2. GRÁFICO COLUMNAS (Totales)
                CamarasSeries = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Total",
                        Values = new ChartValues<int> { camaras.Count, incidencias.Count },
                        Fill = System.Windows.Media.Brushes.OrangeRed
                    }
                };
                Labels = new[] { "Cámaras", "Incidencias" };

                // 3. GRÁFICO BARRAS HORIZONTALES (Top 5 Carreteras) - NUEVO
                // Agrupamos por nombre de carretera, ordenamos descendente y cogemos 5
                var topRoads = incidencias
                    .Where(x => !string.IsNullOrEmpty(x.Carretera)) // Filtramos vacíos
                    .GroupBy(x => x.Carretera)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { Carretera = g.Key, Cantidad = g.Count() })
                    .ToList();

                CarreterasSeries = new SeriesCollection
                {
                    new RowSeries // RowSeries hace barras horizontales
                    {
                        Title = "Incidencias",
                        Values = new ChartValues<int>(topRoads.Select(x => x.Cantidad)),
                        Fill = System.Windows.Media.Brushes.DodgerBlue
                    }
                };

                CarreterasLabels = topRoads.Select(x => x.Carretera).ToArray();
            }
            catch (Exception)
            {
                // Manejo de errores
            }
        }
    }
}