package com.example.demo.servicios;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.stereotype.Service;

import jakarta.mail.internet.MimeMessage;

@Service
public class EmailSchedulerService {

    @Autowired
    private JavaMailSender mailSender;


    public void enviarCorreoBienvenida(String email, String usuario) {
        try {
            MimeMessage message = mailSender.createMimeMessage();
            
            MimeMessageHelper helper = new MimeMessageHelper(message, "utf-8");
            
            helper.setFrom("Mavpc soporte <mavpc1459@gmail.com>");
            
            helper.setTo(email);
            helper.setSubject("Bienvenido a MAVPC - ¡Ya puedes empezar!");
            
            String textoCuerpo = "Hola, " + usuario + ":\r\n"
                    + "\r\n"
                    + "¡Es un placer tenerte con nosotros! Te has unido a la app de MAVPC, la plataforma diseñada para que puedas hacer un seguimiento del trafico en todo momento.\r\n"
                    + "\r\n"
                    + "Si necesitas ayuda en cualquier momento, solo tienes que responder a este mensaje. Nuestro equipo está a un clic de distancia.\r\n"
                    + "\r\n"
                    + "¡Nos vemos dentro!"
                    + "\r\n"
                    + "El equipo de MAVPC.";
            
            helper.setText(textoCuerpo);

            mailSender.send(message);
            System.out.println("Correo enviado con éxito.");
            
        } catch (Exception e) {
            System.err.println("Error al enviar el correo: " + e.getMessage());
        }
    }
}