package com.example.mavpc.controladores;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.example.mavpc.R;
import com.example.mavpc.modelos.Usuario;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class Registro extends BaseActivity {

    EditText etEmail, etUsername, etPassword;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.registro);

        etEmail = findViewById(R.id.etEmail);
        etUsername = findViewById(R.id.etUsername);
        etPassword = findViewById(R.id.etPassword);

        Button btnRegister = (Button) findViewById(R.id.btnRegister);
        btnRegister.setOnClickListener(v -> validarInputs());

        TextView tvRegister = (TextView) findViewById(R.id.tvLogin);
        tvRegister.setOnClickListener(v -> login());
    }

    private void validarInputs() {
        String txtEmail = etEmail.getText().toString();
        String txtUsername = etUsername.getText().toString();
        String txtPassword = etPassword.getText().toString();

        if (txtEmail.isEmpty() || txtUsername.isEmpty() || txtPassword.isEmpty()) {
            Toast.makeText(this, "Por favor, rellena todos los campos", Toast.LENGTH_SHORT).show();
            return;
        }

        if (!txtEmail.contains("@")) {
            Toast.makeText(this, "Introduce un email válido", Toast.LENGTH_SHORT).show();
            return;
        }

        comprobarDisponibilidad(txtUsername, txtEmail, txtPassword);
    }

    private void comprobarDisponibilidad(String usuario, String email, String password) {
        String BASE_URL = "http://10.10.16.93:8080/api/";

        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(BASE_URL)
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        ApiService service = retrofit.create(ApiService.class);

        // llamada a la api
        Call<Boolean> call = service.comprobarUsuarioRegistro(usuario, email);

        call.enqueue(new Callback<Boolean>() {
            @Override
            public void onResponse(Call<Boolean> call, Response<Boolean> response) {
                if (response.isSuccessful() && response.body() != null) {
                    boolean yaExiste = response.body();

                    if (yaExiste) {
                        Toast.makeText(Registro.this, "Este usuario o email ya están registrados", Toast.LENGTH_LONG).show();
                    } else {
                        registrarUsuaio(service, usuario, email, password);
                    }
                } else {
                    Toast.makeText(Registro.this, "Error comprobando datos", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Boolean> call, Throwable t) {
                Toast.makeText(Registro.this, "Fallo de conexión: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void registrarUsuaio(ApiService service, String usuario, String email, String password) {
        Usuario nuevoUsuario = new Usuario();
        nuevoUsuario.setUsername(usuario);
        nuevoUsuario.setEmail(email);
        nuevoUsuario.setPassword(password);
        nuevoUsuario.setPfpUrl(null);

        Call<Void> callRegistro = service.registrarUsuario(nuevoUsuario);

        callRegistro.enqueue(new Callback<Void>() {
            @Override
            public void onResponse(Call<Void> call, Response<Void> response) {
                // solo comprobamos el código HTTP (200-299)
                if (response.isSuccessful()) {
                    Toast.makeText(Registro.this, "¡Registro completado!", Toast.LENGTH_LONG).show();

                    Intent intent = new Intent(Registro.this, Explorar.class);
                    startActivity(intent);

                    finish();
                } else {
                    // error del servidor (500 o 400)
                    Toast.makeText(Registro.this, "Error al crear usuario: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Void> call, Throwable t) {
                Toast.makeText(Registro.this, "Error de red al registrar", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void login() {
        Intent intent = new Intent(Registro.this, Login.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        startActivity(intent);
        overridePendingTransition(0, 0);

        finish();
    }

}