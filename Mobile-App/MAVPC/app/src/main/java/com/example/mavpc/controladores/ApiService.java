package com.example.mavpc.controladores;

// modelos
import com.example.mavpc.modelos.Camara;
import com.example.mavpc.modelos.Incidencia;
import com.example.mavpc.modelos.Usuario;

import java.util.List;

import retrofit2.Call;
import retrofit2.http.GET;
import retrofit2.http.Query;

public interface ApiService {
    // solo se pone la parte final de la url de la API
    @GET("incidencias/listarActual")
    Call<List<Incidencia>> obtenerIncidenciasHoy();

    @GET("camaras")
    Call<List<Camara>> obtenerCamaras();

    @GET("usuarios/comprobarUsuario")
    Call<Usuario> comprobarUsuarioLogin(
            @Query("usuario") String nombreUsuario,   // "usuario" es el nombre del parámetro en la API
            @Query("contrasena") String password      // "contrasena" es el nombre del parámetro en la API
    );
}