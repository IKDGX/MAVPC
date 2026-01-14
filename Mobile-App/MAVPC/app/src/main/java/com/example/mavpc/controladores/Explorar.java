package com.example.mavpc.controladores;

import android.content.Intent;
import android.os.Bundle;
import android.preference.PreferenceManager; // Importante para osmdroid

import androidx.appcompat.app.AppCompatActivity;

import org.osmdroid.config.Configuration;
import org.osmdroid.tileprovider.tilesource.TileSourceFactory;
import org.osmdroid.util.GeoPoint;
import org.osmdroid.views.MapView;
import org.osmdroid.views.overlay.Marker;

import com.example.mavpc.R;
import com.google.android.material.bottomnavigation.BottomNavigationView;

public class Explorar extends AppCompatActivity {

    private MapView map;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // cargar configuración de osmdroid
        // esto evita que los servidores de mapas te bloqueen
        Configuration.getInstance().load(getApplicationContext(),
                PreferenceManager.getDefaultSharedPreferences(getApplicationContext()));

        setContentView(R.layout.explorar);

        // 2. Inicializar el Mapa
        map = findViewById(R.id.mapaOsm);
        map.setTileSource(TileSourceFactory.MAPNIK); // Estilo de mapa estándar
        map.setMultiTouchControls(true); // Permitir zoom con dos dedos

        // 3. Centrar el mapa en Irun
        // GeoPoint usa (Latitud, Longitud)
        GeoPoint startPoint = new GeoPoint(43.338, -1.789);
        map.getController().setZoom(15.0);
        map.getController().setCenter(startPoint);

        // 4. Añadir un marcador
        Marker startMarker = new Marker(map);
        startMarker.setPosition(startPoint);
        startMarker.setAnchor(Marker.ANCHOR_CENTER, Marker.ANCHOR_BOTTOM);
        startMarker.setTitle("Irun - Centro");
        startMarker.setSnippet("¡Aquí estamos sin pagar nada!");
        map.getOverlays().add(startMarker);

        // 5. Configurar Botones del Menú (Igual que antes)
        setupBottomNav();
    }

    private void setupBottomNav() {
        BottomNavigationView bottomNav = findViewById(R.id.bottomNav);
        bottomNav.setOnItemSelectedListener(item -> {
            int id = item.getItemId();
            if (id == R.id.nav_explorar) return true;
            if (id == R.id.nav_favoritos) {
                Intent intent = new Intent(Explorar.this, Favoritos.class);
                startActivity(intent);

                this.finish();
            }
            if (id == R.id.nav_incidencias) {
                Intent intent = new Intent(Explorar.this, Incidencias.class);
                startActivity(intent);

                this.finish();
            }
            return false;
        });
    }

    @Override
    public void onResume() {
        super.onResume();
        if (map != null) map.onResume(); // Necesario para el ciclo de vida de osmdroid
    }

    @Override
    public void onPause() {
        super.onPause();
        if (map != null) map.onPause(); // Necesario para pausar descargas
    }
}