package com.example.demo.servicios;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

@Service
public class EmailSchedulerService {

    @Autowired
    private JavaMailSender mailSender;


    public void enviarCorreoBienvenida(String email, String usuario) {
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(email);
        message.setSubject("Bienvenido a MAVPC - ¡Ya puedes empezar!");
        message.setText("Hola, "+ usuario +":\r\n"
        		+ "\r\n"
        		+ "¡Es un placer tenerte con nosotros! Te has unido a la app de MAVPC, la plataforma diseñada para que puedas hacer un seguimiento del trafico en todo momento.\r\n"
        		+ "\r\n"
        		+ "Si necesitas ayuda en cualquier momento, solo tienes que responder a este mensaje. Nuestro equipo está a un clic de distancia.\r\n"
        		+ "\r\n"
        		+ "¡Nos vemos dentro! El equipo de MAVPC.");

        mailSender.send(message);
        System.out.println("Correo enviado con éxito.");
    }
}