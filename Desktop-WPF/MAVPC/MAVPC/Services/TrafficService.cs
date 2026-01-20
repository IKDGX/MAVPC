using MAVPC.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace MAVPC.Services
{
    public class TrafficService : ITrafficService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TrafficService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                // Esto es vital para que [JsonPropertyName] funcione al enviar
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<bool> AddCamaraAsync(Camara camara)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://10.10.16.93:8080/api/camaras/guardar", camara, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Windows.MessageBox.Show($"Error Servidor (Cámara):\n{error}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddIncidenciaAsync(Incidencia incidencia)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://10.10.16.93:8080/api/incidencias/guardar", incidencia, _jsonOptions);
                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    System.Windows.MessageBox.Show($"Error Servidor (Incidencia):\n{error}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Camara>> GetCamarasAsync() =>
            await _httpClient.GetFromJsonAsync<List<Camara>>("http://10.10.16.93:8080/api/camaras", _jsonOptions) ?? new();

        public async Task<List<Incidencia>> GetIncidenciasAsync() =>
            await _httpClient.GetFromJsonAsync<List<Incidencia>>("http://10.10.16.93:8080/api/incidencias/listarActual", _jsonOptions) ?? new();

        public async Task<bool> DeleteCamaraAsync(string id)
        {
            try
            {
                // 1. Construimos la URL con el parámetro 'id' (porque en Java es @RequestParam)
                string url = $"http://10.10.16.93:8080/api/camaras/eliminar?id={id}";

                // 2. ¡IMPORTANTE! Usamos GetAsync porque el servidor tiene @GetMapping
                var response = await _httpClient.GetAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Error de conexión: {ex.Message}");
                return false;
            }
        }
    }
}