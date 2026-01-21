package com.example.demo.configuracion;

import org.modelmapper.ModelMapper;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import com.example.demo.modelos.Incidencia;
import com.example.demo.modelos.IncidenciaCreada;

@Configuration
public class Configracion {
    @Bean
    public ModelMapper modelMapper() {
        ModelMapper modelMapper = new ModelMapper();

        // Regla: De Incidencia -> a IncidenciaCreada
        modelMapper.typeMap(Incidencia.class, IncidenciaCreada.class).addMappings(mapper -> {
            mapper.map(src -> src.getId(), IncidenciaCreada::setId);
            mapper.map(src -> src.getTipo(), IncidenciaCreada::setTipo);
        });

        return modelMapper;
    }
}