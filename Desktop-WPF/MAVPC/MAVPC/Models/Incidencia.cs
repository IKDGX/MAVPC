using System;
using System.Text.Json.Serialization;

namespace MAVPC.Models
{
    public class Incidencia
    {
        // El JSON envía "328096" (String), así que lo recogemos como string para evitar errores
        [JsonPropertyName("incidenceId")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("incidenceType")]
        public string Tipo { get; set; } = string.Empty; // Ej: Obras, Accidente

        [JsonPropertyName("cause")]
        public string Causa { get; set; } = string.Empty; // Ej: Obras

        [JsonPropertyName("incidenceLevel")]
        public string Nivel { get; set; } = string.Empty; // Ej: Blanco, Amarillo, Rojo

        [JsonPropertyName("road")]
        public string Carretera { get; set; } = string.Empty; // Ej: AP-1

        [JsonPropertyName("pkStart")]
        public string Kilometro { get; set; } = string.Empty; // Ej: "112.0"

        [JsonPropertyName("cityTown")]
        public string Municipio { get; set; } = string.Empty; // Ej: Legutiano

        [JsonPropertyName("province")]
        public string Provincia { get; set; } = string.Empty; // Ej: ARABA

        [JsonPropertyName("direction")]
        public string Direccion { get; set; } = string.Empty; // Ej: Madrid

        [JsonPropertyName("startDate")]
        public DateTime FechaInicio { get; set; }

        // Las coordenadas vienen como string en tu JSON ("42.96046")
        // Es mejor guardarlas como string y convertirlas solo si necesitas pintar un mapa.
        [JsonPropertyName("latitude")]
        public string Latitud { get; set; } = string.Empty;

        [JsonPropertyName("longitude")]
        public string Longitud { get; set; } = string.Empty;


        // --- EXTRAS VISUALES (Calculados automáticamente) ---

        // Icono dinámico: Mira el TIPO de incidencia
        [JsonIgnore]
        public string IconKind
        {
            get
            {
                var t = Tipo?.ToLower() ?? "";
                var c = Causa?.ToLower() ?? "";

                if (t.Contains("accidente") || c.Contains("accidente")) return "CarCrash";
                if (t.Contains("obra") || c.Contains("obra")) return "Cone";
                if (t.Contains("nieve") || t.Contains("hielo") || c.Contains("meteorolo")) return "Snowflake";
                if (t.Contains("retención") || t.Contains("retencion")) return "CarSide";

                return "AlertCircle"; // Icono por defecto
            }
        }

        // Color dinámico: Ahora mira el NIVEL (Amarillo, Rojo...) que es más preciso
        [JsonIgnore]
        public string StatusColor
        {
            get
            {
                var n = Nivel?.ToLower() ?? "";

                if (n.Contains("rojo")) return "#FF003C";     // Rojo Neón (Crítico)
                if (n.Contains("negro")) return "#000000";    // Negro (Corte total) - Podríamos poner borde rojo
                if (n.Contains("amarillo")) return "#FFA500"; // Naranja (Precaución)
                if (n.Contains("blanco")) return "#94F2E2";   // Cian/Blanco (Fluido/Leve) #00FFCC

                // Si no hay nivel, miramos el tipo por si acaso
                var t = Tipo?.ToLower() ?? "";
                if (t.Contains("accidente")) return "#FF003C";

                return "#CCCCCC"; // Gris por defecto
            }
        }

        // Propiedad extra para mostrar ubicación completa en la lista
        [JsonIgnore]
        public string UbicacionCompleta => $"{Carretera} (Km {Kilometro}) - {Municipio}";
    }
}