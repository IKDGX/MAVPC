package com.example.demo;

import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

@Service
public class TraficoService {

    // URL de ejemplo de incidencias de tráfico de Open Data Euskadi
	private final String URL_TRAFICO = "https://api.euskadi.eus/traffic/v1.0/incidences";

    public Object obtenerDatosTrafico() {
        RestTemplate restTemplate = new RestTemplate();
        // Hacemos la llamada GET y recibimos la respuesta como un Objeto (JSON automático)
        return restTemplate.getForObject(URL_TRAFICO, Object.class);
    }
}