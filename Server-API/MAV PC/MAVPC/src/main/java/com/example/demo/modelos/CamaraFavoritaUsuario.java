package com.example.demo.modelos;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

@Entity
@Table(name = "camarasFavoritas_usuarios")
@JsonIgnoreProperties(ignoreUnknown = true)
public class CamaraFavoritaUsuario {
	
	@Id
	private int id;
	
	private int idCamara;
	
	private int idUsuario;

	public CamaraFavoritaUsuario() {
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getIdCamara() {
		return idCamara;
	}

	public void setIdCamara(int idCamara) {
		this.idCamara = idCamara;
	}

	public int getIdUsuario() {
		return idUsuario;
	}

	public void setIdUsuario(int idUsuario) {
		this.idUsuario = idUsuario;
	}

}
