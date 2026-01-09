package com.example.demo;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HolaController {

    @GetMapping("/hola")
    public String saludar() {
        return "¡Servidor Spring funcionando correctamente!";
    }
    
    @Autowired
    private TraficoService traficoService;

    @GetMapping("/trafico-euskadi")
    public Object verTrafico() {
        return traficoService.obtenerDatosTrafico();
    }
}