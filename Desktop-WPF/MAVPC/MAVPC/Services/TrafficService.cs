using MAVPC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Importante para PostAsJsonAsync
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MAVPC.Services
{


    public class TrafficService : ITrafficService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "http://10.10.16.93:8080/api/camaras";

        public TrafficService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Aumentamos el timeout por si la red es lenta
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<Camara>> GetCamarasAsync()
        {
            try
            {
                // 1. Descargar JSON
                var jsonString = await _httpClient.GetStringAsync(BASE_URL);

                // 2. Configurar opciones (importante para leer números como strings si hace falta)
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                // 3. Deserializar la lista directa
                var datos = System.Text.Json.JsonSerializer.Deserialize<List<Camara>>(jsonString, options);

                return datos ?? new List<Camara>();
            }
            catch
            {
                // Si falla, lista vacía para no romper la app
                return new List<Camara>();
            }
        }

        public async Task<bool> AddCamaraAsync(Camara nuevaCamara)
        {
            try
            {
                // Envia los datos al servidor (POST)
                var response = await _httpClient.PostAsJsonAsync(BASE_URL, nuevaCamara);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Incidencia>> GetIncidenciasAsync()
        {
            string url = "http://10.10.16.93:8080/api/incidencias";
            try
            {
                var jsonString = await _httpClient.GetStringAsync(url);

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                // 1. Deserializamos usando la clase "Envoltorio" que hemos creado abajo
                var respuestaApi = System.Text.Json.JsonSerializer.Deserialize<IncidenciasResponse>(jsonString, options);

                // 2. Devolvemos solo la lista que hay dentro. Si es nula, devolvemos lista vacía.
                return respuestaApi?.Incidencias ?? new List<Incidencia>();
            }
            catch (Exception ex)
            {
                // Si falla, muestra el error por si acaso, pero ya no debería fallar.
                System.Diagnostics.Debug.WriteLine($"ERROR API: {ex.Message}");
                return new List<Incidencia>();
            }
        }
    } // <--- Cierre de la clase TrafficService

    // --- AÑADE ESTA CLASE AL FINAL DEL ARCHIVO (fuera de TrafficService pero dentro del namespace) ---

    public class IncidenciasResponse
    {
        // Esta propiedad debe llamarse igual que en el JSON ("incidences")
        [JsonPropertyName("incidences")]
        public List<Incidencia> Incidencias { get; set; }
    }
} // <--- Cierre del namespace

    
