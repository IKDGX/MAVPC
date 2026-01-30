package com.example.demo.servicios;

import java.net.HttpURLConnection;
import java.net.URI;
import java.net.URL;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import com.example.demo.daos.CamaraDao;
import com.example.demo.daos.IncidenciaDao;
import com.example.demo.modelos.Camara;
import com.example.demo.modelos.Incidencia;

import tools.jackson.core.type.TypeReference;
import tools.jackson.databind.JsonNode;
import tools.jackson.databind.ObjectMapper;

@Service
public class TraficoService {

    @Autowired
    private CamaraDao camaraDao;
    @Autowired
    private IncidenciaDao incidenciaDao;
    
    // Inyectamos el RestTemplate configurado en 'Configracion.java' (el que ignora SSL)
    @Autowired
    private RestTemplate restTemplate;
    
    private final String BASE_URL = "https://api.euskadi.eus/traffic/v1.0";

    /**
     * Obtiene un objeto genérico con todas las incidencias (endpoint básico).
     * Útil para pruebas rápidas de conexión.
     */
    public Object obtenerTodasIncidencias() {
        return restTemplate.getForObject(BASE_URL + "/incidences", Object.class);
    }
    
    /**
     * Descarga todas las incidencias de una fecha específica.
     * * COMPLEJIDAD: 
     * - Maneja la paginación de la API (bucle do-while).
     * - Deserializa manualmente el JSON para leer 'totalPages'.
     */
    public List<Incidencia> obtenerTodasIncidenciasDelDia(String anio, String mes, String dia) {
        List<Incidencia> todasLasDelDia = new ArrayList<>();
        int paginaActual = 1;
        int totalPaginas = 1;
        ObjectMapper mapper = new ObjectMapper();

        try {
            // Bucle para recorrer todas las páginas disponibles en la API
            do {
                String url = BASE_URL + "/incidences/byDate/" + anio + "/" + mes + "/" + dia + "?_page=" + paginaActual;
                
                // Obtenemos la respuesta como un Nodo JSON genérico para poder inspeccionarlo antes de convertir
                JsonNode root = restTemplate.getForObject(url, JsonNode.class);

                if (root != null && root.has("incidences")) {
                    // Solo en la primera vuelta leemos cuántas páginas hay en total
                    if (paginaActual == 1 && root.has("totalPages")) {
                        totalPaginas = root.get("totalPages").asInt();
                    }

                    // Extraemos el array "incidences" del JSON
                    JsonNode nodes = root.get("incidences");
                    
                    // Convertimos ese nodo JSON a una Lista de objetos Java 'Incidencia'
                    List<Incidencia> paginaLista = mapper.convertValue(
                        nodes, 
                        new TypeReference<List<Incidencia>>() {}
                    );

                    todasLasDelDia.addAll(paginaLista);
                    
                    paginaActual++;
                } else {
                    break; // Si no hay incidencias o respuesta, salimos
                }
            } while (paginaActual <= totalPaginas);

        } catch (Exception e) {
            System.err.println("Error al obtener incidencias del día paginadas: " + e.getMessage());
        }

        return todasLasDelDia;
    }

    /**
     * Obtiene una lista rápida de cámaras (primeras 1000).
     * Usa TypeReference para mapear directamente el JSON a lista de objetos.
     */
    public List<Camara> obtenerCamaras() {
        try {
            JsonNode root = restTemplate.getForObject("https://api.euskadi.eus/traffic/v1.0/cameras?_pageSize=1000", JsonNode.class);
            
            if (root != null && root.has("cameras")) {
                JsonNode camerasNode = root.get("cameras");
                ObjectMapper mapper = new ObjectMapper();
                
                // Magia de Jackson: Convierte el nodo JSON directamente a List<Camara>
                return mapper.readerFor(new TypeReference<List<Camara>>() {})
                             .readValue(camerasNode);
            }
        } catch (Exception e) {
            System.out.println("Error al deserializar: " + e.getMessage());
        }
        return Collections.emptyList();
    }
    
    /**
     * Proceso masivo para descargar, corregir y guardar cámaras en la BD.
     * * COMPLEJIDAD:
     * - Corrección de URLs: Reemplaza dominios antiguos por nuevos.
     * - Validación: Comprueba si la imagen existe (Ping) antes de guardar.
     */
    public void SubirCamaras() {
        int paginaActual = 1;
        int totalPaginas = 1;
        String URL_BASE_EUSKADI = "https://api.euskadi.eus/traffic/v1.0/cameras";

        String dominioAntiguo = "https://www.trafikoa.eus";
        String dominioNuevo = "https://apps.trafikoa.euskadi.eus";

        do {
            String urlConPagina = URL_BASE_EUSKADI + "?_page=" + paginaActual;
            
            try {
                JsonNode root = restTemplate.getForObject(urlConPagina, JsonNode.class);

                if (root != null && root.has("cameras")) {
                    if (paginaActual == 1) {
                        totalPaginas = root.get("totalPages").asInt();
                        System.out.println("Total de páginas detectadas: " + totalPaginas);
                    }

                    JsonNode camerasNode = root.get("cameras");
                    ObjectMapper mapper = new ObjectMapper();
                    
                    List<Camara> listaPagina = mapper.convertValue(
                        camerasNode, 
                        new TypeReference<List<Camara>>() {}
                    );

                    List<Camara> camarasParaGuardar = new ArrayList<>();

                    for (Camara camara : listaPagina) {
                        
                        // Reseteamos ID a null para que la BD genere uno nuevo (autoincrement)
                        camara.setId(null); 

                        // LÓGICA DE NEGOCIO: Actualizar dominios obsoletos
                        String urlCamara = camara.getUrlImage();
                        if (urlCamara != null && urlCamara.contains(dominioAntiguo)) {
                            String nuevaUrl = urlCamara.replace(dominioAntiguo, dominioNuevo);
                            camara.setUrlImage(nuevaUrl);
                            urlCamara = nuevaUrl;
                        }

                        // VALIDACIÓN: Solo guardamos si la URL responde (evita imágenes rotas en la app)
                        if (urlCamara != null && esUrlValida(urlCamara)) {
                            camarasParaGuardar.add(camara);
                        }
                    }

                    // Guardado por lotes (más eficiente que guardar una por una)
                    if (!camarasParaGuardar.isEmpty()) {
                        camaraDao.saveAll(camarasParaGuardar);
                        System.out.println("Guardada página " + paginaActual + ". Insertadas: " + camarasParaGuardar.size());
                    } else {
                        System.out.println("Página " + paginaActual + " procesada, pero ninguna cámara tenía imagen válida.");
                    }

                    paginaActual++;
                    
                } else {
                    break;
                }
            } catch (Exception e) {
                System.err.println("Error en la página " + paginaActual + ": " + e.getMessage());
            }

        } while (paginaActual <= totalPaginas);
        
        System.out.println("Sincronización finalizada con éxito.");
    }

    /**
     * Método auxiliar para validar si una URL de imagen es accesible.
     * * TRUCO:
     * - Usa el método HTTP "HEAD" en lugar de "GET".
     * - "HEAD" pide solo los encabezados (metadata) sin descargar la imagen entera.
     * - Esto hace la comprobación muchísimo más rápida y ahorra datos.
     */
    private boolean esUrlValida(String urlString) {
        try {
            URI uri = new URI(urlString);
            URL url = uri.toURL();
            
            HttpURLConnection huc = (HttpURLConnection) url.openConnection();
            
            huc.setRequestMethod("HEAD"); // Clave para rendimiento
            huc.setConnectTimeout(2000);  // Si tarda más de 2s, asumimos que está caída
            huc.setReadTimeout(2000);
            
            int responseCode = huc.getResponseCode();
            
            // Devuelve true solo si el servidor responde "200 OK"
            return responseCode == HttpURLConnection.HTTP_OK; 
            
        } catch (Exception e) {
            return false;
        }
    }
    
    /**
     * Tarea programada (Cron Job) que se ejecuta automáticamente.
     * Se conecta a la API, comprueba duplicados y guarda nuevas incidencias.
     * * COMPLEJIDAD:
     * - Configuración de fecha dinámica (siempre busca "hoy").
     * - Prevención de duplicados consultando a la BD antes de insertar.
     * - Control de flujo (Thread.sleep) para no saturar la API externa.
     */
    @Scheduled(fixedRate = 900000) // Se ejecuta cada 900,000 ms (15 minutos)
    public void SubirIncidenciasDelDia() {
        // Cálculo de la fecha de hoy para construir la URL dinámica
        LocalDate hoy = LocalDate.now();
        String anio = String.valueOf(hoy.getYear());
        String mes = String.format("%02d", hoy.getMonthValue());
        String dia = String.format("%02d", hoy.getDayOfMonth());
        
        int paginaActual = 1;
        int totalPaginas = 1;
        int nuevasIncidencias = 0;
        int duplicadasTotal = 0;
        
        String URL_BASE = "https://api.euskadi.eus/traffic/v1.0/incidences/byDate/" 
                          + anio + "/" + mes + "/" + dia;

        ObjectMapper mapper = new ObjectMapper(); 

        do {
            String urlConPagina = URL_BASE + "?_page=" + paginaActual;
            
            try {
                // Llamada a la API usando el RestTemplate seguro (SSL bypass)
                JsonNode root = restTemplate.getForObject(urlConPagina, JsonNode.class);

                if (root != null && root.has("incidences")) {
                    // Detectar total de páginas solo en la primera iteración
                    if (paginaActual == 1) {
                        totalPaginas = root.get("totalPages").asInt();
                        System.out.println("Sincronizando: " + dia + "/" + mes + "/" + anio);
                    }

                    JsonNode incidenciasNode = root.get("incidences");
                    List<Incidencia> listaPagina = mapper.convertValue(
                        incidenciasNode,
                        new TypeReference<List<Incidencia>>() {}
                    );

                    List<Incidencia> incidenciasAInsertar = new ArrayList<>();
                    int duplicadasEnPagina = 0;

                    // FILTRADO DE DUPLICADOS:
                    // Iteramos lo que viene de la API y preguntamos a la base de datos
                    // si ya tiene ese ID específico.
                    for (Incidencia incidencia : listaPagina) {
                        // IMPORTANTE: 'existsByIncidenceId' evita errores de Primary Key duplicada
                        if (!incidenciaDao.existsByIncidenceId(incidencia.getIncidenceId())) {
                            incidenciasAInsertar.add(incidencia);
                        } else {
                            duplicadasEnPagina++;
                        }
                    }

                    // Solo llamamos a la base de datos si hay algo nuevo que guardar
                    if (!incidenciasAInsertar.isEmpty()) {
                        incidenciaDao.saveAll(incidenciasAInsertar);
                        nuevasIncidencias += incidenciasAInsertar.size();
                    }
                    
                    duplicadasTotal += duplicadasEnPagina;
                    System.out.println("Página " + paginaActual + "/" + totalPaginas 
                                       + " -> Nuevas: " + incidenciasAInsertar.size() 
                                       + " | Omitidas: " + duplicadasEnPagina);

                    paginaActual++;
                    Thread.sleep(100); // Pausa de 100ms para ser "educados" con la API

                } else {
                    break;
                }
            } catch (Exception e) {
                System.err.println("Error en página " + paginaActual + ": " + e.getMessage());
                break; // Si falla una página, rompemos el bucle para evitar bucles infinitos de error
            }

        } while (paginaActual <= totalPaginas);

        System.out.println("Sincronización finalizada. Nuevas: " + nuevasIncidencias + " | Duplicadas: " + duplicadasTotal);
    }
}