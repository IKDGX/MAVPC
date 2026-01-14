using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using MAVPC.Models;

using MAVPC.MVVM.Views; // Necesario para abrir la ventana

using MAVPC.Services;

using System.Collections.ObjectModel;

using System.Threading.Tasks;

using System.Windows; // Para crear la ventana flotante



namespace MAVPC.MVVM.ViewModels

{

    public partial class DashboardViewModel : ObservableObject

    {

        private readonly ITrafficService _trafficService;



        // Lista de cámaras (Usamos la clase nueva 'Camara')

        [ObservableProperty] private ObservableCollection<Camara> _cameras;

        // Lista de incidencias

        [ObservableProperty] private ObservableCollection<Incidencia> _incidencias;



        // Propiedad para enlazar con el Formulario

        [ObservableProperty] private Camara _newCamera;



        [ObservableProperty] private int _activeIncidents;

        // KPIs

        [ObservableProperty] private int _totalCameras;

        [ObservableProperty] private string _systemStatus = "CONECTADO";

        [ObservableProperty] private bool _isLoading;



        // Ventana del formulario (para poder cerrarla desde código)

        private Window? _formWindow;



        public DashboardViewModel(ITrafficService trafficService)

        {

            _trafficService = trafficService;

            Cameras = new ObservableCollection<Camara>();

            Incidencias = new ObservableCollection<Incidencia>();

            NewCamera = new Camara(); // Inicializamos vacía



            LoadDataCommand.Execute(null);

        }



        [RelayCommand]

        private async Task LoadData()

        {

            IsLoading = true;

            SystemStatus = "SINCRONIZANDO...";



            // Cargar Cámaras

            var dataCam = await _trafficService.GetCamarasAsync();

            Cameras.Clear();

            foreach (var item in dataCam) Cameras.Add(item);



            // Cargar Incidencias (NUEVO)

            var dataInc = await _trafficService.GetIncidenciasAsync();

            Incidencias.Clear();

            foreach (var item in dataInc) Incidencias.Add(item);



            // Actualizar KPIs

            TotalCameras = Cameras.Count;

            ActiveIncidents = Incidencias.Count; // Usamos la cuenta real ahora



            SystemStatus = "EN LÍNEA";

            IsLoading = false;

        }



        // --- COMANDOS PARA AÑADIR ---



        [RelayCommand]

        private void OpenAddForm()

        {

            // Limpiamos el objeto para una nueva inserción

            NewCamera = new Camara();



            // Creamos una ventana flotante rápida

            _formWindow = new Window

            {

                Title = "Añadir Dispositivo",

                Content = new CameraFormView(), // Cargamos la vista que creamos antes

                DataContext = this, // Compartimos este ViewModel

                SizeToContent = SizeToContent.WidthAndHeight,

                WindowStyle = WindowStyle.None,

                AllowsTransparency = true,

                Background = System.Windows.Media.Brushes.Transparent,

                WindowStartupLocation = WindowStartupLocation.CenterScreen,

                ResizeMode = ResizeMode.NoResize

            };



            _formWindow.ShowDialog(); // Mostramos modal (bloquea la de atrás)

        }



        [RelayCommand]

        private async Task Save()

        {

            if (string.IsNullOrWhiteSpace(NewCamera.Id) || string.IsNullOrWhiteSpace(NewCamera.Nombre))

            {

                MessageBox.Show("El ID y el Nombre son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                return;

            }



            IsLoading = true;



            // Enviamos a la API

            bool success = await _trafficService.AddCamaraAsync(NewCamera);



            if (success)

            {

                // Cerramos ventana

                _formWindow?.Close();

                _formWindow = null;



                // Recargamos la tabla para ver el cambio

                await LoadData();

                MessageBox.Show("Cámara guardada correctamente en el servidor.", "Éxito");

            }

            else

            {

                MessageBox.Show("Error al conectar con 10.10.16.93. Verifica que el servidor está encendido.", "Error de Conexión");

            }



            IsLoading = false;

        }



        [RelayCommand]

        private void Cancel()

        {

            _formWindow?.Close();

        }



        [RelayCommand]

        private void ExportPdf()

        {

            MessageBox.Show("Función PDF pendiente.");

        }

    }

}