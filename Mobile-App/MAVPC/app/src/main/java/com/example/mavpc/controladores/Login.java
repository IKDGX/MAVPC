package com.example.mavpc.controladores;

import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.example.mavpc.R;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class Login extends BaseActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);

        Button btnLogin = (Button) findViewById(R.id.btnLogin);
        btnLogin.setOnClickListener(v -> login());

        TextView tvRegister = (TextView) findViewById(R.id.tvRegister);
        tvRegister.setOnClickListener(v -> register());
    }

    private void login() {
        TextView etUsername = findViewById(R.id.etUsername);
        TextView etPassword = findViewById(R.id.etPassword);

        String txtUsername = etUsername.getText().toString();
        String txtPassword = etPassword.getText().toString();

        if (txtUsername.isEmpty() || txtPassword.isEmpty()) {
            Toast.makeText(this, "Por favor rellena los campos", Toast.LENGTH_SHORT).show();
            return;
        }

        // configuracion retrofit
        String BASE_URL = "http://10.10.16.93:8080/api/";
        Retrofit retrofit = new Retrofit.Builder()
                .baseUrl(BASE_URL)
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        ApiService service = retrofit.create(ApiService.class);

        // llamada a la api
        Call<Boolean> call = service.comprobarUsuarioLogin(txtUsername, txtPassword);

        call.enqueue(new Callback<Boolean>() {
            @Override
            public void onResponse(Call<Boolean> call, Response<Boolean> response) {
                // verificamos que la conexión fue bien y el cuerpo no es nulo
                if (response.isSuccessful() && response.body() != null) {

                    boolean usuarioCorrecto = response.body();

                    if (usuarioCorrecto) {
                        Intent intent = new Intent(Login.this, Explorar.class);
                        intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
                        startActivity(intent);
                        overridePendingTransition(0, 0);

                        finish();
                    } else {
                        Toast.makeText(Login.this, "Usuario o contraseña incorrectos", Toast.LENGTH_SHORT).show();
                    }

                } else {
                    Toast.makeText(Login.this, "Error en el servidor: " + response.code(), Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Boolean> call, Throwable t) {
                // error de conexión (no hay internet/ip incorrecta/timeout)
                Toast.makeText(Login.this, "Fallo de conexión: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                Log.e("LOGIN", "Error: " + t.getMessage());
            }
        });
    }

    private void register() {
        Intent intent = new Intent(Login.this, Registro.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        startActivity(intent);
        overridePendingTransition(0, 0);

        finish();
    }
}