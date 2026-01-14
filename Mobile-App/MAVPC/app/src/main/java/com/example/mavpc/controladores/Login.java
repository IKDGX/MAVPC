package com.example.mavpc.controladores;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.example.mavpc.R;

public class Login extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login);

        Button btnLogin = (Button) findViewById(R.id.btnLogin);
        btnLogin.setOnClickListener(v -> login());
    }

    private void login() {
        TextView tvEmailLabel = (TextView) findViewById(R.id.tvEmailLabel);
        TextView tvPasswordLabel = (TextView) findViewById(R.id.tvPasswordLabel);

        String txtEmail = (String) tvEmailLabel.getText();
        String txtPass = (String) tvPasswordLabel.getText();

        if(true){
            Intent intent = new Intent(Login.this, Explorar.class);
            startActivity(intent);

            this.finish();
        } else{
            Toast.makeText(this, "email o contraseña incorrectos", Toast.LENGTH_SHORT).show();
        }
    }

}